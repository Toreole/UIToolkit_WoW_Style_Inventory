using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace WoW_Inventory
{
    public static class CursorInfo 
    {
        
        public static VisualElement Cursor {get; internal set;}
        public static Image Image {get; internal set;}
        public static Sprite DefaultSprite {get; internal set;}
        public static CursorState CurrentState = CursorState.Default;

        ///<summary>
        ///Tell the cursor to "hold" an item(stack).
        ///Returns true if the cursor was able to pick it up.
        ///</summary>
        private static bool HoldItemStack(IInventory inventory, int index, VisualElement slot)
        {
            //The cursor can only start holding an item if its not doing anything else.
            if(CurrentState is not CursorState.Default)
            {
                return false;
            }
            else
            {
                var itemInfo = inventory.GetItemStackAtIndex(index);
                //check if its empty first.
                if(itemInfo is null)
                {
                    return false;
                }
                else
                {
                    Image.sprite = itemInfo.Item.Sprite;
                    itemCarry = new FullItemStackPayload(inventory, index);
                    lastSlot = slot;
                    slot.SetEnabled(false); //disable the slot for now.
                    CurrentState = CursorState.HoldingItem;
                    return true;
                }
            }
        }

        public static void HoldItemStack(IInventory inventory, int index)
        {
            var stack = inventory.GetItemStackAtIndex(index);
            Image.sprite = stack.Item.Sprite;

            itemCarry = new FullItemStackPayload(inventory, index);
            CurrentState = CursorState.HoldingItem;
        }

        public static void HoldPartialStackAmount(IInventory inventory, int index, int amount)
        {
            if(amount == 0)
            {
                CurrentState = CursorState.Default;
                Image.sprite = DefaultSprite;
                lastSlot.SetEnabled(true);
                return;
            }
            var stack = inventory.GetItemStackAtIndex(index);
            Image.sprite = stack.Item.Sprite;

            itemCarry = new PartialItemStackPayload(inventory, index, amount);
            CurrentState = CursorState.HoldingItem;
        }

        ///<summary>
        ///Places the held item into the index in the bag.
        ///See: InventoryUtility.MoveStacks
        ///</summary>
        public static void PlaceItem(IInventory targetInventory, int index) 
        {               //IDEA: rework to IItemReceiver instead of IInventory/index. Item receiver has slot data and everything.
            if(itemCarry.TryDeliver(targetInventory, targetSlot: index))
            {
                lastSlot.SetEnabled(true); //Re-enable the slot.
                //Reset the cursor sprite.
                Image.sprite = DefaultSprite;
                CurrentState = CursorState.Default;
            }
        }

        private static IItemPayload itemCarry;
        private static VisualElement lastSlot;

        //AmountThingy 
        // MovePartOfItemStack vs MoveEntireStack
        // Moving part requires the destination(target) to be a stack of the same item, OR be an empty slot.
        // moving an entire stack is easy we did that already.
        // 
        // When picking up a part of a stack: 
        // - open the amount menu thingy (number input)
        //   - max amount = original stack.Count
        // - Disable interaction with other uielements? other interactions cause abort of the action
        // ->> when confirmed amount to move:
            // - set Image.sprite to the item.Sprite
            // - create a new ItemStack with Item and confirmed amount
            // - OR keep reference to original bag/index + int amountToMove?
        // - If confirmed amount == itemstack.Count => FullStackMove.

        ///<summary>
        ///Handles all inventory clicks pretty much.
        ///</summary>
        public static void HandleInventorySlotMouseDown(MouseDownEvent e, IInventory inventory, int slotIndex, VisualElement slot)
        {
            var stack = inventory.GetItemStackAtIndex(slotIndex);
            switch(CurrentState)
            {
                case CursorState.Default:
                    if(stack is null) //no item, nothing to do :cool:
                        return;
                    if(e.shiftKey && stack.Count > 1)
                    {
                        //picking up a partial stack starts here
                        CurrentState = CursorState.PreparingPartialStack;
                        //pre-disable slot and remember it
                        slot.SetEnabled(false);
                        lastSlot = slot;
                        //Enable the stack amount thingy for the slot.
                        UIManager.AmountPopup.Show(inventory, slotIndex);
                    }
                    else //otherwise hold the entire stack.
                        HoldItemStack(inventory, slotIndex, slot);
                    break;
                case CursorState.HoldingItem:
                    PlaceItem(inventory, slotIndex);
                    break;
                case CursorState.PreparingPartialStack: 
                    //abort preparing partial stack yay. 
                    //also happens when pressing esc. so this probably has to be a public method or something idk.
                    break;
            }
        }


    }

    public enum CursorState
    {
        Default = 0,
        Error = 666,
        HoldingItem = 1,
        PreparingPartialStack = 2
    }
}