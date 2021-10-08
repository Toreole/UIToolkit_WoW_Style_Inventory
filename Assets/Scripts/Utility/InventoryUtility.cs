using UnityEngine;

namespace WoW_Inventory
{
    public static class InventoryUtility
    {
        ///<summary>
        ///Moves items between inventories and stacks. Combines stacks of matching items if possible.
        ///</summary>
        internal static void MoveStacks(IInventory origin, int oIndex, IInventory destination, int dIndex)
        {
            //slightly more convoluted item swapping.
            ItemStack oBuffer = origin.GetItemStackAtIndex(oIndex);
            ItemStack dBuffer = destination.GetItemStackAtIndex(dIndex);
            if(oBuffer is null || dBuffer is null)//one of them is null, just swap them right away.
            {
                Swap();
                return;
            }
            if(oBuffer.Item == dBuffer.Item) //both stacks are of the same item. combine them
            {
                int maxStack = dBuffer.Item.StackSize;
                //the maximum amount of items we can add to the destination stack.
                int add = Mathf.Min(oBuffer.Count, maxStack-dBuffer.Count);
                //change the item counts on the stacks.
                oBuffer.Count -= add;
                dBuffer.Count += add;
                //check if the origin stack is depleted.
                if(oBuffer.Count <= 0)
                    origin.SetItemStackAtIndex(null, oIndex);
                else
                    origin.NotifyChange(oIndex);
                destination.NotifyChange(dIndex);
                return;
            }
            Swap();

            void Swap()
            {
                origin.SetItemStackAtIndex(dBuffer, oIndex);
                destination.SetItemStackAtIndex(oBuffer, dIndex);
            }
        }

        ///<summary>
        ///Naively swap two stacks in different/equal inventories.
        ///</summary>
        internal static void MoveStacksSimple(IInventory origin, int oIndex, IInventory destination, int dIndex)
        {
            var buff = origin.GetItemStackAtIndex(oIndex);
            origin.SetItemStackAtIndex(destination.GetItemStackAtIndex(dIndex), oIndex);
            destination.SetItemStackAtIndex(buff, dIndex);
        }
    }
}