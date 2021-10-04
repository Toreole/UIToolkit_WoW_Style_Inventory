using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace WoW_Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public UIDocument doc;
        InventoryBag targetBag;
        public int testInventorySize = 12;

        internal InventoryBag TargetBag => targetBag; //DEBUG ONLY

        VisualElement bagWindow;
        GroupBox slotGroup;
        VisualElement[] slots;
        Image[] slotImages;

        private bool uiOpened = true; //open by default for now.
        private bool refreshOnOpen = false;


        // Start is called before the first frame update
        void Start()
        {
            InitInventoryBag(); //Testing more or less.
            targetBag.OnBagChanged += OnInventoryBagChange;
            InitializeSlots();
        }

        void InitializeSlots()
        {
            bagWindow = doc.rootVisualElement.Q("InventoryBagWindow");
            slotGroup = bagWindow.Q<GroupBox>("InventoryBag");
            
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
                img.StretchToParentSize();
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
                ItemStack stack = targetBag.GetItemStackAtIndex(index);
                if(stack is null)
                {
                    //Stack has been removed from the inventory.
                    //Clear all information on the slot UI.
                    slotImages[index].sprite = null;
                }
                else
                {
                    slotImages[index].sprite = stack.Item.Sprite; //assign the new sprite.
                }
            }
        }

        ///<summary>
        ///Creates an inventory bag. Pretty much for testing purposes only.
        ///</summary>
        private void InitInventoryBag()
        {
            targetBag = new InventoryBag(testInventorySize);
        }
    }
}