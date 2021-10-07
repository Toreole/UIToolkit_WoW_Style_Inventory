using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace WoW_Inventory
{
    ///<summary>
    ///UI Element for easy handling of inventory stuff. Specifically for InventoryBags
    ///</summary>
    public class InventoryUI : VisualElement
    {
        public int testInventorySize = 12;

        GroupBox slotGroup;
        VisualElement[] slots;
        Image[] slotImages;

        private bool uiOpened = true; //open by default for now.
        private bool refreshOnOpen = false;

        internal void Initialize(InventoryBag bag)
        {
            bag.OnBagChanged += OnInventoryBagChange;
            slotGroup = this.Q<GroupBox>("InventoryBag");
            
            slots = new VisualElement[slotGroup.childCount];
            slotImages = new Image[slotGroup.childCount];

            //Add images to all the slots. By default these are empty (transparent)
            int index = 0;
            foreach(var element in slotGroup.Children())
            {
                slots[index] = element;
                var img = new Image();
                slotImages[index] = img;
                element.Add(img);
                int slotIndex = index;
                img.RegisterCallback<MouseDownEvent>((e) => HandleMouseDown(e, slotIndex));
                img.StretchToParentSize();
                //hastily get the sprite lol
                img.sprite = bag.GetStackInfo(index).item?.Sprite;
                index++;
            }
        }   

        ///<summary>
        ///The handler for the OnBagChanged event in the inventorybag. Needs to be added to event in setup.
        ///</summary>
        private void OnInventoryBagChange(OnBagChangedEvent e)
        {
            if(uiOpened == false)//UI inactive, should not update. Will be refreshed with next open if necessary
            {   
                refreshOnOpen = true; //mark for a full refresh.
                return;
            }
            foreach(int index in e.indices)//using a foreach is so simple i cant help it
            {
                ItemStackInfo stack = e.bag.GetStackInfo(index);
                if(stack == ItemStackInfo.Empty)
                {
                    //Stack has been removed from the inventory.
                    //Clear all information on the slot UI.
                    slotImages[index].sprite = null;
                }
                else
                {
                    slotImages[index].sprite = stack.item.Sprite; //assign the new sprite.
                }
            }
        }

        private void HandleMouseDown(MouseDownEvent e, int slotIndex)
        {
            Debug.Log($"Mouse down: {e.button}, slotIndex={slotIndex}");
        }

        public new class UxmlFactory : UxmlFactory<InventoryUI> {}
    }
}