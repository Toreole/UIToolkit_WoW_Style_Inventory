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
        private static bool HoldItem(IInventory inventory, int index, VisualElement slot)
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
                    lastInventory = inventory;
                    lastItemIndex = index;
                    lastSlot = slot;
                    slot.SetEnabled(false); //disable the slot for now.
                    CurrentState = CursorState.HoldingItem;
                    return true;
                }
            }
        }

        ///<summary>
        ///Places the held item into the index in the bag.
        ///See: InventoryUtility.MoveStacks
        ///</summary>
        public static void PlaceItem(IInventory targetInventory, int index)
        {
            lastSlot.SetEnabled(true); //Re-enable the slot.
            InventoryUtility.MoveStacks(lastInventory, lastItemIndex, targetInventory, index);
            //Reset the cursor sprite.
            Image.sprite = DefaultSprite;
            CurrentState = CursorState.Default;
        }

        private static IInventory lastInventory;
        private static int lastItemIndex;
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
        // - New CursorState for this probably.

        public static void HandleInventorySlotMouseDown(MouseDownEvent e, IInventory inventory, int slotIndex, VisualElement slot)
        {
            var stack = inventory.GetItemStackAtIndex(slotIndex);
            switch(CurrentState)
            {
                case CursorState.Default:
                    if(stack is null)
                        return;
                    HoldItem(inventory, slotIndex, slot);
                    break;
                case CursorState.HoldingItem:
                    PlaceItem(inventory, slotIndex);
                    break;
            }
        }


    }

    public enum CursorState
    {
        Default = 0,
        Error = 666,
        HoldingItem = 1,
        HoldingPartialStack = 2,
        PreparingPartialStack = 3
    }

}
