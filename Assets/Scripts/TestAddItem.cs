using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WoW_Inventory;

public class TestAddItem : MonoBehaviour
{
    public InventoryUI uI;

    public Item itemToAdd;
    public Item secondItemToAdd;
    public int amountToAdd;
    
    IEnumerator Start() 
    {
        yield return new WaitForSeconds(0.7f);
        if(uI.TargetBag.TryAddFull(new ItemStack(){Item = itemToAdd, Count = amountToAdd}))
            Debug.Log("Added Item");
        yield return new WaitForSeconds(0.7f);
        uI.TargetBag.TryAddFull(new ItemStack(){Item = secondItemToAdd, Count = amountToAdd});
        yield return new WaitForSeconds(1f);
        uI.TargetBag.MoveStacks(0, 1);
        yield return new WaitForSeconds(1f);
        uI.TargetBag.MoveStacks(0, 5);
        yield return new WaitForSeconds(1f);
        uI.TargetBag.MoveStacks(5, 2);
        yield return new WaitForSeconds(1f);
        uI.TargetBag.MoveStacks(2, 7);
        yield return new WaitForSeconds(1f);
        uI.TargetBag.MoveStacks(1, 5);
    }
    
}
