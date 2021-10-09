using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WoW_Inventory
{
    public interface IItemPayload 
    {
        bool TryDeliver(IInventory target, int targetSlot);
    }
}