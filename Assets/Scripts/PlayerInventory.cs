using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WoW_Inventory
{
    ///<summary>
    ///The PlayerInventory is made up of multiple bags. More or less just a "utility" class.
    ///</summary>
    public class PlayerInventory
    {
        InventoryBag[] bags = new InventoryBag[5];   //up to 5 bags.

        public bool TryAddAllFitting(ItemStack stack)
        {
            for(int i = 0; i < bags.Length; i++)
                if(bags[i] is not null)
                    if (bags[i].TryAddAllFitting(stack))
                        return true;
            return false;
        }

        public bool TryAddFull(ItemStack stack)
        {
            for(int i = 0; i < bags.Length; i++)
                if(bags[i] is not null)
                    if (bags[i].TryAddFull(stack))
                        return true;
            return false;
        }

        ///<summary>
        ///Moves a stack from one bag to another.
        ///bagA, index of item in bagA, bagB, index of item in bagB
        ///</summary>
        public void MoveStack(IInventory origin, int oIndex, IInventory destination, int dIndex)
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