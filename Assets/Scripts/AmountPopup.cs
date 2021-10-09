using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace WoW_Inventory
{
    public class AmountPopup 
    {
        private VisualElement window;
        private NumberInputField inputField;

        private Button increaseButton, decreaseButton;
        private Button confirmButton;

        public int MaxValue {get => inputField.MaxValue; set => inputField.MaxValue = value;}

        public AmountPopup(VisualElement root)
        {
            window = root;
            inputField = new NumberInputField();
            inputField.Init(window.Q<TextField>());
            increaseButton = window.Q<Button>("IncreaseButton");
                increaseButton.clicked += inputField.IncreaseValue;
            decreaseButton = window.Q<Button>("DecreaseButton");
                decreaseButton.clicked += inputField.DecreaseValue;
            confirmButton = window.Q<Button>("ConfirmButton");
                confirmButton.clicked += ConfirmAmountValue;
        }

        public void Show(IInventory inventory, int slotIndex)
        {
            //should appear above the mouse.
            var pos = UnityEngine.Input.mousePosition;
            pos.x *= 1920f / UnityEngine.Display.main.renderingWidth ;
            pos.y *= 1080f / UnityEngine.Display.main.renderingHeight;
            window.style.left = new StyleLength((int)pos.x);
            window.style.bottom = new StyleLength((int)pos.y);

            inv = inventory;
            invSlot = slotIndex;
            inputField.MaxValue = inventory.GetItemStackAtIndex(slotIndex).Count;
            inputField.Reset();
        }

        public void Hide()
        {
            window.style.left = new StyleLength(10000);
            window.style.bottom = new StyleLength(10000);
        }

        //this is the only way general kenobi
        IInventory inv;
        int invSlot;

        private void ConfirmAmountValue()
        {
            int amount = 0;
            if(inputField.TryGetValue(out amount))
            {
                if(amount == MaxValue)
                {
                    CursorInfo.HoldItemStack(inv, invSlot);
                    Hide();
                    return;
                }
                CursorInfo.HoldPartialStackAmount(inv, invSlot, amount);
                Hide();
            }
        }
    }
}