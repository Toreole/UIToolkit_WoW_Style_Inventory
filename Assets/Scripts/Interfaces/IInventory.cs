using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WoW_Inventory
{
    public interface IInventory : IDepositInterface, IWithdrawInterface //funny, maybe useful at some point.
    {
        void MoveStacks(int firstIndex, int secondIndex);
    }

    public interface IDepositInterface
    {
        bool TryAddAllFitting(ItemStack stack);
        void SetItemStackAtIndex(ItemStack stack, int index);
    }

    public interface IWithdrawInterface
    {
        bool HasItemAmount(Item item, int amount);
        void RemoveItemAmount(Item item, int amount);
        ItemStack GetItemStackAtIndex(int index);
    }
}