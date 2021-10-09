using UnityEngine;
using System;

namespace WoW_Inventory
{
    ///<summary>
    ///Moves a full stack. Juse uses InventoryUtility.MoveStacks for convenience.
    ///</summary>
    internal struct FullItemStackPayload : IItemPayload
    {
        private IInventory origin;
        private int originIndex;

        internal FullItemStackPayload(IInventory o, int i)
        {
            origin = o;
            originIndex = i;
        }

        public bool TryDeliver(IInventory target, int targetSlot)
        {
            InventoryUtility.MoveStacks(origin, originIndex, target, targetSlot);
            return true;
        }
    }

    ///<summary>
    ///Moves a partial stack from one slot to another. 
    ///Only succeeds when the targetted slot is empty.
    ///</summary>
    internal struct PartialItemStackPayload : IItemPayload
    {
        private IInventory origin;
        private int originIndex;
        private int amount;

        internal PartialItemStackPayload(IInventory o, int i, int n)
        {
            origin = o;
            originIndex = i;
            amount = n;
        }

        public bool TryDeliver(IInventory target, int targetSlot)
        {
            if(target.GetItemStackAtIndex(targetSlot) is not null)
                return false;

            var stack = origin.GetItemStackAtIndex(originIndex);
            target.SetItemStackAtIndex(new ItemStack(stack.Item, amount), targetSlot);
            stack.Count -= amount;
            if(stack.Count == 0)
                origin.SetItemStackAtIndex(null, originIndex);
            origin.NotifyChange(originIndex);
            return true;
        }
    }
}