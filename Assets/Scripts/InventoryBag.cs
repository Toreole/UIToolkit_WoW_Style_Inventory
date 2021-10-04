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

        public int Size => size;
        private CountArray<int> changedIndices;

        public InventoryBag() : this(1) {}
        public InventoryBag(int size)
        {
            stacks = new ItemStack[size];
            this.size = size;
            ItemCount = 0;
            changedIndices = new CountArray<int>(size);
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
                    //mark the index as changed.
                    changedIndices.Add(i);
                    FireBagChangedAndClearChanged();
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
                    //mark the index as changed.
                    changedIndices.Add(i);
                    //if the stack no longer has items, we can end this right here.
                    if(stack.Count == 0)
                    {
                        FireBagChangedAndClearChanged();
                        return true;
                    }
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
            return outArray;
        }
        
        ///<summary>
        ///Move two stacks to change places.
        ///O(1)
        ///</summary>
        public void MoveStacks(int firstIndex, int secondIndex)
        {
            if(firstIndex >= size || firstIndex < 0 || secondIndex >= size || secondIndex < 0)
                throw new System.IndexOutOfRangeException($"SetStackAtIndex: bad index. indices={firstIndex}, {secondIndex} | size={size}");
            var buffer = stacks[firstIndex];
            stacks[firstIndex] = stacks[secondIndex];
            stacks[secondIndex] = buffer;
            //handle change.
            changedIndices.Add(firstIndex); 
            changedIndices.Add(secondIndex);
            FireBagChangedAndClearChanged();
        }

        ///<summary>
        ///Removes all occurences of an item.
        ///O(n)
        ///</summary>
        public void RemoveAll(Item item)
        {
            for(int i = 0; i < size; i ++)
                if(stacks[i].Item == item)
                {
                    changedIndices.Add(i);
                    stacks[i] = null;
                }
            if(changedIndices.Count > 0) //make sure the event is only firing when actually needed.
                FireBagChangedAndClearChanged();
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
                    //mark changed index.
                    changedIndices.Add(i);
                    amount -= remove;
                    if(amount == 0)
                    {
                        FireBagChangedAndClearChanged();
                        return;
                    }
                }
            }
            //fire the event even if not all items could be removed.
            FireBagChangedAndClearChanged();
            if(amount > 0)
                Debug.LogWarning("RemoveItemAmount paramater 'amount' greater than the avialable item count!");
        }

        ///<summary>
        ///Sets the stack stored at the given index.
        ///Use with care. O(1)
        ///</summary>
        public void SetStackAtIndex(ItemStack stack, int index)
        {
            if(index >= size || index < 0)
                	throw new System.IndexOutOfRangeException($"SetStackAtIndex: bad index. index={index} | size={size}");
            stacks[index] = stack;
            OnBagChanged(new(index));
        }

        public void RemoveStackAtIndex(int index) => SetStackAtIndex(null, index);

        public event System.Action<OnBagChangedEvent> OnBagChanged;
        private void FireBagChangedAndClearChanged()
        {
            OnBagChanged(new(changedIndices.ToTinyArray())); //funny new
            changedIndices.Clear();
        }
        
    }
    public readonly struct OnBagChangedEvent
    {
        public readonly int[] indices {get;}

        public OnBagChangedEvent(int[] indices)
        {
            this.indices = indices;
        }
        public OnBagChangedEvent(int index)
        {
            indices = new int[]{index};
        }
    }
}