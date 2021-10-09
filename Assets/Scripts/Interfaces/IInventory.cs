using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WoW_Inventory
{
    public interface IInventory : IDepositInventory, IWithdrawInventory //funny, maybe useful at some point.
    {
        void MoveStacks(int firstIndex, int secondIndex);

        void NotifyChange(int changedIndex);
    }

    public interface IDepositInventory
    {
        bool TryAddAllFitting(ItemStack stack);
        void SetItemStackAtIndex(ItemStack stack, int index);
    }

    public interface IWithdrawInventory
    {
        bool HasItemAmount(Item item, int amount);
        void RemoveItemAmount(Item item, int amount);
        ItemStack GetItemStackAtIndex(int index);
    }
}