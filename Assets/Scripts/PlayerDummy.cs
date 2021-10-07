using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WoW_Inventory;

///<summary>A class to test functionality of the PlayerInventory. Needs to be a MonoBehaviour to live in the scene.</summary>
public class PlayerDummy : MonoBehaviour
{
    private PlayerInventory inventory;

    public Item itemA, itemB;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1); //wait right at the start to ensure this runs last.
        
        //60 item slots in total.
        var bags = new InventoryBag[5] {new InventoryBag(12), new InventoryBag(12), new InventoryBag(12), new InventoryBag(12), new InventoryBag(12)};
        inventory = new PlayerInventory(bags);
        inventory.InitGUI();
        inventory.OpenAllBags();

        inventory.TryAddFull(new ItemStack(itemA));
        inventory.TryAddFull(new ItemStack(itemB));

        inventory.SetItemStackAtIndex(new ItemStack(itemB), 50); //50 should be within limits

        yield return new WaitForSeconds(1);
        inventory.MoveStacks(1, 3);
        yield return new WaitForSeconds(1);
        inventory.MoveStacks(2, 50);
        yield return new WaitForSeconds(1);
        inventory.CloseAllBags();
        yield return new WaitForSeconds(1);
        inventory.OpenAllBags();
    }
}
