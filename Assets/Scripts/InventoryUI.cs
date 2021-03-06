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
        Label[] slotLabels;

        private bool uiOpened = true; //open by default for now.
        private bool refreshOnOpen = false;

        internal void Initialize(InventoryBag bag)
        {
            bag.OnBagChanged += OnInventoryBagChange;
            slotGroup = this.Q<GroupBox>("InventoryBag");
            
            slots = new VisualElement[slotGroup.childCount];
            slotImages = new Image[slotGroup.childCount];
            slotLabels = new Label[slotGroup.childCount];

            //Add images to all the slots. By default these are empty (transparent)
            int index = 0;
            foreach(var element in slotGroup.Children())
            {
                slots[index] = element;
                var img = new Image();
                slotImages[index] = img;
                element.Add(img);
                int slotIndex = index;
                img.RegisterCallback<MouseDownEvent>((e) => CursorInfo.HandleInventorySlotMouseDown(e, bag, slotIndex, img)); //not sure how performant this is...
                img.StretchToParentSize();

                //create label:
                var label = new Label();
                label.AddToClassList("itemSlotLabel");
                label.pickingMode = PickingMode.Ignore; //ignore mousevents on the label please thanks.
                label.text = "";
                slotLabels[index] = label;
                img.Add(label);

                //yep yep actually get the info up-to-date
                var info = bag.GetStackInfo(index);
                img.sprite = info.item?.Sprite;
                label.text = info.amount > 0? info.amount.ToString() : "";

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
                    slotLabels[index].text = "";
                }
                else
                {
                    slotImages[index].sprite = stack.item.Sprite; //assign the new sprite.
                    slotLabels[index].text = stack.amount.ToString(); //update amount text.
                }
            }
        }

        private void HandleMouseDown(MouseDownEvent e, InventoryBag bag, int slotIndex)
        {
            Debug.Log($"Mouse down: {e.button}, slotIndex={slotIndex}");

            //CursorInfo.HandleInventoryClick

            //if(CursorInfo.HoldItem(bag, slotIndex, slotImages[slotIndex]))
            //{
                //Success!
            //}
            //else if(CursorInfo.CurrentState is CursorState.HoldingItem)
            //{
                //CursorInfo.PlaceItem(bag, slotIndex);
            //}
        }


        public new class UxmlFactory : UxmlFactory<InventoryUI> {}
    }
}