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
        public static bool HoldItem(InventoryBag bag, int index, VisualElement slot)
        {
            //The cursor can only start holding an item if its not doing anything else.
            if(CurrentState is not CursorState.Default)
            {
                return false;
            }
            else
            {
                var itemInfo = bag.GetStackInfo(index);
                //check if its empty first.
                if(itemInfo.IsEmpty())
                {
                    return false;
                }
                else
                {
                    Image.sprite = itemInfo.item.Sprite;
                    lastBag = bag;
                    lastItemIndex = index;
                    lastSlot = slot;
                    slot.SetEnabled(false); //disable the slot for now.
                    CurrentState = CursorState.HoldingItem;
                    return true;
                }
            }
        }

        public static void PlaceItem(InventoryBag bag, int index)
        {
            lastSlot.SetEnabled(true); //Re-enable the slot.
            InventoryUtility.MoveStacks(lastBag, lastItemIndex, bag, index);
            //Reset the cursor sprite.
            Image.sprite = DefaultSprite;
            CurrentState = CursorState.Default;
        }

        private static InventoryBag lastBag;
        private static int lastItemIndex;
        private static VisualElement lastSlot;
    }

    public enum CursorState
    {
        Default = 0,
        Error = 666,
        HoldingItem = 1,
    }

}
