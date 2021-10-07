using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;

namespace WoW_Inventory
{
    ///<summary>
    ///The PlayerInventory is made up of multiple bags. More or less just a "utility" class.
    ///</summary>
    public class PlayerInventory : IInventory
    {
        InventoryBag[] bags = new InventoryBag[5];   //up to 5 bags.
        InventoryUI[] bagWindows = new InventoryUI[5];

        public int TotalSize {get; private set;}

        public void RecalculateSize()
        {
            TotalSize = 0;
            foreach(var b in bags)
                if(b is not null)
                    TotalSize += b.Size;
        }

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
        ///Search for all ItemStacks of a given item.
        ///O(5n)
        ///</summary>
        internal CountArray<ItemStack> QueryStacks(Item item)
        {
            CountArray<ItemStack> outArray = new(TotalSize);
            for(int i = 0; i < bags.Length; i++)
                if(bags[i] is not null)
                    bags[i].QueryStacks(item, outArray);
            return outArray;
        }

        ///<summary>
        ///Checks whether this inventory contains total amount of item or more.
        ///O(5n)
        ///</summary>
        public bool HasItemAmount(Item item, int total)
        {
            int count = 0;
            for(int i = 0; i < bags.Length; i++)
                if(bags[i] is not null)
                    for(int j = 0; j < bags[i].Size; j++)
                    {
                        var stack = bags[i].GetItemStackAtIndex(j);
                        if(stack is not null && stack.Item == item)
                            count += stack.Count;
                    }
            return count >= total;
        }

        ///<summary>
        ///Moves the stacks at the indices.
        ///Not recommended to use. Use InventoryUtility.MoveStacks directly on the target bags.
        ///</summary>
        public void MoveStacks(int firstIndex, int secondIndex)
        {
            Debug.LogWarning("I don't recommend the use of PlayerInventory.MoveStacks. Use the interface handles that have direct access to the bags and their indices.");
            if(firstIndex >= TotalSize || secondIndex >= TotalSize)
                throw new System.IndexOutOfRangeException($"Index out of range: {firstIndex} / {secondIndex} -- size={TotalSize}");
            
            InventoryBag a = null, b = null;
            int indexA = -1, indexB = -1;
            //This could be handled with using GetBagAndIndex() twice, but this should perform better in theory.
            for(int i = 0; i < bags.Length; i++)
            {
                int s = bags[i].Size;
                if(bags[i] is null)
                    continue;
                //Find the bags.
                if(firstIndex < s)
                {
                    a = bags[i];
                    indexA = firstIndex;
                    firstIndex = int.MaxValue;
                }
                else
                    firstIndex -= s;
                if(secondIndex < s)
                {
                    b = bags[i];
                    indexB = secondIndex;
                    secondIndex = int.MaxValue;
                }
                else
                    secondIndex -= s;
            }
            InventoryUtility.MoveStacks(a, indexA, b, indexB);
        }

        public void SetItemStackAtIndex(ItemStack stack, int index)
        => GetBagAndIndex(ref index).SetItemStackAtIndex(stack, index);

        public void RemoveItemAmount(Item item, int amount)
        {
            for(int i = 0; i < bags.Length; i++)
                if(bags[i] is not null)
                {
                    bags[i].RemoveItemAmount(item, ref amount);
                    if(amount == 0)
                        return;
                }
            Debug.LogWarning("RemoveItemAmount amount greater than actual amount of available items");
        }

        public ItemStack GetItemStackAtIndex(int index)
        => GetBagAndIndex(ref index).GetItemStackAtIndex(index);

        ///<summary>
        ///Figures out the correct InventoryBag and corresponding relative index from a 0->TotalSize-1 index.
        ///</summary>
        private InventoryBag GetBagAndIndex(ref int index)
        {
            for(int i = 0; i < bags.Length; i++)
                if(bags[i] is not null)
                    if(index < bags[i].Size)
                        return bags[i];
                    else 
                        index -= bags[i].Size;
            throw new System.IndexOutOfRangeException($"Index not inside inventory bounds. index={index}, size={TotalSize}");
        }
    
        ///<summary>
        ///Create the UI windows for the bags.
        ///</summary>
        public void InitGUI()
        {
            //Calling an async mathod thats not run async :)
            VisualTreeAsset tree = Addressables.LoadAssetAsync<VisualTreeAsset>("Assets/UI_XML/xml_window_InventoryBagWindow.uxml").WaitForCompletion();
            bagWindows = new InventoryUI[bags.Length];
            for(int i = 0; i < bags.Length; i++)
            {
                var window = tree.CloneTree().Q<InventoryUI>();
                bagWindows[i] = window;
                if(bags[i] is not null)
                    window.Initialize(bags[i]);
                //UIManager.InventoryGroup.Add(window); -- this would be to have them open by default
            }
        }
    }
}