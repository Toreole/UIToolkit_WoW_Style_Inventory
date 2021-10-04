using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestVisualElement : VisualElement
{

    public TestVisualElement() : base()
    {
        this.style.backgroundColor = new StyleColor(Color.red);
        this.style.height = new StyleLength(new Length(100, LengthUnit.Pixel));
    }

    public override void HandleEvent(EventBase evt)
    {
        base.HandleEvent(evt);
        if(evt is MouseDownEvent)
        {
            Debug.Log("Visual Element MouseDown");
        }
    }
}
