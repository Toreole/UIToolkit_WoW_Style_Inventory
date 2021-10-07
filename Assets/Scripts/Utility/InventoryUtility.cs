namespace WoW_Inventory
{
    public static class InventoryUtility
    {
        ///<summary>
        ///Moves a stack from one bag to another.
        ///bagA, index of item in bagA, bagB, index of item in bagB
        ///</summary>
        public static void MoveStacks(IInventory origin, int oIndex, IInventory destination, int dIndex)
        {
            if(origin == destination)
            {
                origin.MoveStacks(oIndex, dIndex);
                return;
            }
            //slightly more convoluted item swapping.
            ItemStack oBuffer = origin.GetItemStackAtIndex(oIndex);
            origin.SetItemStackAtIndex(destination.GetItemStackAtIndex(dIndex), oIndex);
            destination.SetItemStackAtIndex(oBuffer, dIndex);
        }
    }
}