using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WoW_Inventory
{
    public class ItemStack
    {
        public Item Item {get; internal set;} = null;
        public int Count {get; internal set;} = 0;

    }
}