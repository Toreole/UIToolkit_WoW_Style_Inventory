using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestUIController : MonoBehaviour
{
    [SerializeField]
    private UIDocument uIDocument;
    [SerializeField]
    private Sprite testImage;

    private VisualElement rootElement;

    // Start is called before the first frame update
    void Start()
    {
        rootElement = uIDocument.rootVisualElement;

        var exImg = rootElement.Query<Image>().First();
        if(exImg != null)
        {
            Debug.Log("found existing image.");
            exImg.StretchToParentSize();
            exImg.sprite = testImage;
        }

        //rootElement.layout can be used to avoid overlapping?
        var slot = rootElement.Query("InventorySlot").First();
        Image img = new Image();
        img.StretchToParentSize();
        img.sprite = testImage;
        img.RegisterCallback<MouseDownEvent>((x) => Debug.Log("CLICK!"));
        img.RegisterCallback<DragPerformEvent>((x) => Debug.Log("Dragging"));
        //img.SetEnabled(false);
        slot.Add(img);
        
        //img.tooltip

        //rootElement.visible = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
