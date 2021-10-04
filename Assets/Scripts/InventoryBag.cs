using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WoW_Inventory
{
    public class InventoryBag
    {
        private ItemStack[] stacks;
        private int size;
        public int ItemCount {get; private set;}

        public InventoryBag() : this(1) {}
        public InventoryBag(int size)
        {
            stacks = new ItemStack[size];
            this.size = size;
            ItemCount = 0;
        }

        ///<summary>
        ///Tries adding the full stack at once. Requires an empty slot.
        ///O(n)
        ///</summary>
        public bool TryAddFull(ItemStack stack)
        {
            for(int i = 0; i < size; i++)
            {
                if(stacks[i] is null)
                {
                    stacks[i] = stack;
                    return true;
                }
            }
            return false;
        }

        ///<summary>
        ///Adds all items from the stack that fit in this bag. Fills up existing stacks with priority.
        ///Returns true if all items fit. O(2n+m)
        ///</summary>
        public bool TryAddAllFitting(ItemStack stack)
        {
            var item = stack.Item;
            var existingStacks = QueryStacks(item);
            if(existingStacks.Count > 0)
            {
                for(int i = 0; i < existingStacks.Count; i++)
                {
                    if(existingStacks[i].Count == item.StackSize)
                        continue;
                    //the maximum that can be added to the existing stack.
                    int maxAdd = item.StackSize - existingStacks[i].Count;
                    //the actual amount to add. the smallest of the maxAdd and stack count (cant add more than we have right)
                    int actualAdd = Mathf.Min(maxAdd, stack.Count);
                    //remove the added items from the source stack, and add them to the inventory's existing stack.
                    stack.Count -= actualAdd;
                    existingStacks[i].Count += actualAdd;
                    //if the stack no longer has items, we can end this right here.
                    if(stack.Count == 0)
                        return true;
                }
            }
            //add the remainder.
            return TryAddFull(stack);
        }

        ///<summary>
        ///Search for all ItemStacks of a given item.
        ///O(n)
        ///</summary>
        private CountArray<ItemStack> QueryStacks(Item item)
        {
            CountArray<ItemStack> outArray = new(size);
            for(int i = 0; i < size; i++)
                if(stacks[i] is not null && stacks[i].Item == item)
                    outArray.Add(stacks[i]);
            //outList.TrimExcess(); //Trimming excess might be somewhat unnecessary.
            return outArray;
        }
        
        ///<summary>
        ///Move two stacks to change places.
        ///O(1)
        ///</summary>
        public void MoveStacks(int firstIndex, int secondIndex)
        {
            var buffer = stacks[firstIndex];
            stacks[firstIndex] = stacks[secondIndex];
            stacks[secondIndex] = buffer;
        }

        ///<summary>
        ///Removes all occurences of an item.
        ///O(n)
        ///</summary>
        public void RemoveAll(Item item)
        {
            for(int i = 0; i < size; i ++)
                if(stacks[i].Item == item)
                    stacks[i] = null;
        }

        ///<summary>
        ///Checks whether this bag contains total amount of item or more.
        ///Should not allocate memory.
        ///O(n)
        ///</summary>
        public bool HasItemAmount(Item item, int total)
        {
            int count = 0;
            for(int i = 0; i < size; i++)
                if(stacks[i].Item == item)
                    count += stacks[i].Count;
            return count >= total;
        }

        ///<summary>
        ///Removes a given amount of a specified item.
        ///O(n)
        ///</summary>
        public void RemoveItemAmount(Item item, int amount)
        {
            for(int i = 0; i < size; i++)
            {
                var stack = stacks[i];
                if(stack.Item == item)
                {
                    int remove = Mathf.Min(stack.Count, amount);
                    stack.Count -= remove;
                    if(stack.Count == 0)
                        stacks[i] = null; //remove itemStack at this index.
                    
                    amount -= remove;
                    if(amount == 0)
                        return;
                }
            }
            if(amount > 0)
                Debug.LogWarning("RemoveItemAmount paramater 'amount' greater than the avialable item count!");
        }


    }
}