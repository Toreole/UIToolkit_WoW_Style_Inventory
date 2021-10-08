using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WoW_Inventory
{
    public class InventoryBag : IInventory
    {
        private ItemStack[] stacks;
        private int size;
        public int ItemCount {get; private set;}

        public int Size => size;
        private CountArray<int> changedIndices;
        private int storedStacks = 0;

        public InventoryBag() : this(1) {}
        public InventoryBag(int size)
        {
            stacks = new ItemStack[size];
            this.size = size;
            ItemCount = 0;
            changedIndices = new CountArray<int>(size);
        }

        ///<summary>
        ///Returns the ItemStack at the given index.
        ///</summary>
        public ItemStack GetItemStackAtIndex(int index)
        {
            return stacks[index];
        }

        ///<summary>
        ///Tries adding the full stack at once. Requires an empty slot.
        ///O(n)
        ///</summary>
        public bool TryAddFull(ItemStack stack)
        {
            if(stack.Count == 0)
                return true;
            for(int i = 0; i < size; i++)
            {
                if(stacks[i] is null)
                {
                    stacks[i] = stack;
                    //mark the index as changed.
                    changedIndices.Add(i);
                    FireBagChangedAndClearChanged();
                    storedStacks++;
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
            if(stack.Count == 0)
                return true;
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

        internal void QueryStacks(Item item, CountArray<ItemStack> arr)
        {
            for(int i = 0; i < size; i++)
                if(stacks[i] is not null && stacks[i].Item == item)
                    arr.Add(stacks[i]);
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
                    storedStacks--;
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
                if(stacks[i] is not null && stacks[i].Item == item)
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
                    //maximum amount of items to remove from the stack.
                    int remove = Mathf.Min(stack.Count, amount);
                    stack.Count -= remove;
                    if(stack.Count == 0)
                    {
                        stacks[i] = null; //remove itemStack at this index.
                        storedStacks--;
                    }
                    //mark changed index.
                    changedIndices.Add(i);
                    //reduce the remaining amount of items to remove.
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
        ///just a duplicate but using ref.
        public void RemoveItemAmount(Item item, ref int amount)
        {
            for(int i = 0; i < size; i++)
            {
                var stack = stacks[i];
                if(stack.Item == item)
                {
                    //maximum amount of items to remove from the stack.
                    int remove = Mathf.Min(stack.Count, amount);
                    stack.Count -= remove;
                    if(stack.Count == 0)
                    {
                        stacks[i] = null; //remove itemStack at this index.
                        storedStacks--;
                    }
                    //mark changed index.
                    changedIndices.Add(i);
                    //reduce the remaining amount of items to remove.
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
        public void SetItemStackAtIndex(ItemStack stack, int index)
        {
            if(index >= size || index < 0)
                	throw new System.IndexOutOfRangeException($"SetStackAtIndex: bad index. index={index} | size={size}");
            var current = stacks[index];
            if(current is null && stack is not null)
                storedStacks++;
            else if (current is not null && stack is null)
                storedStacks--;
            stacks[index] = stack;
            OnBagChanged(new(index, this));
        }

        public void RemoveStackAtIndex(int index) => SetItemStackAtIndex(null, index);

        ///<summary>
        ///Safely resizes the InventoryBag.
        ///O(n)
        ///</summary>
        internal void Resize(int newSize)
        {
            if(newSize < storedStacks)
                throw(new System.Exception($"New Size too small. Currently stored={storedStacks}, newSize={newSize}"));
            ItemStack[] arr = new ItemStack[newSize];
            changedIndices = new CountArray<int>(newSize);
            int index = 0; 
            //copy over the items from the old array in a safe way.
            for(int i = 0; i < size; i++)
            {
                if(stacks[i] is null)
                    continue;
                arr[index] = stacks[i];
                index++;
            }
        }

        ///<summary>
        ///Get a representation of what the stack at the given index contains.
        ///ItemStackInfo is a struct, therefore making this unable to manipulate the content of the bag.
        ///</summary>
        public ItemStackInfo GetStackInfo(int index)
        {
            var stack = stacks[index];
            if(stack is null)
                return ItemStackInfo.Empty;
            return new ItemStackInfo(){item = stack.Item, amount = stack.Count};
        }

        public void NotifyChange(int changedIndex)
        {
            changedIndices.Add(changedIndex);
            FireBagChangedAndClearChanged();
        }

        public event System.Action<OnBagChangedEvent> OnBagChanged;
        //Just a helper to easily fire the OnBagChanged event and clear the CountArray of changed indices
        private void FireBagChangedAndClearChanged()
        {
            OnBagChanged?.Invoke(new(changedIndices.ToTinyArray(), this)); //funny new
            changedIndices.Clear();
        }
        
    }
    public readonly struct OnBagChangedEvent
    {
        public readonly InventoryBag bag;
        public readonly int[] indices {get;}

        public OnBagChangedEvent(int[] indices, InventoryBag b)
        {
            this.indices = indices;
            bag = b;
        }
        public OnBagChangedEvent(int index, InventoryBag b)
        {
            indices = new int[]{index};
            bag = b;
        }
    }
}