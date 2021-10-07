using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WoW_Inventory
{
    ///<summary>
    ///An Item Stack in an inventory. Just a reference to an item and an amount.
    ///!!As class, it's always a reference!!! So be careful.
    ///</summary>
    public class ItemStack
    {
        public Item Item {get; internal set;} = null;
        public int Count {get; internal set;} = 0;

        public ItemStack(){}
        public ItemStack(Item item) : this(item, 1)
        {
        }

        public ItemStack(Item item, int amount)
        {
            Item = item;
            Count = amount;
        }

    }
}