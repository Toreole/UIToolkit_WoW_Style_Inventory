using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    public UIDocument doc;

    VisualElement bagWindow;
    GroupBox slotGroup;
    VisualElement[] slots;
    Image[] slotImages;

    // Start is called before the first frame update
    void Start()
    {
        InitializeSlots();
    }

    void InitializeSlots()
    {
        bagWindow = doc.rootVisualElement.Q("InventoryBagWindow");
        slotGroup = bagWindow.Q<GroupBox>("InventoryBag");
        //var slotGroupStyle = slotGroup.style; CHANGES TO THE STYLE CAN ALWAYS BE MADE!
        //slotGroup.style.height = new StyleLength(new Length(156, LengthUnit.Pixel));
        //slotGroupStyle.height = new((slotGroup.childCount/4) * 50 + slotGroupStyle.paddingBottom.value.value + slotGroupStyle.paddingTop.value.value);
        
        slots = new VisualElement[slotGroup.childCount];
        slotImages = new Image[slotGroup.childCount];

        //Add images to all the slots. By default these are empty (transparent)
        int index = 0;
        foreach(var c in slotGroup.Children())
        {
            slots[index] = c;
            var img = new Image();
            slotImages[index] = img;
            c.Add(img);
            img.StretchToParentSize();
            Debug.Log(c.name);
            index++;
        }
        
    }
}
