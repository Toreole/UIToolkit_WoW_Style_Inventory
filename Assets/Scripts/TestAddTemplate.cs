using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestAddTemplate : MonoBehaviour
{
    public UnityEngine.UIElements.VisualTreeAsset templateToAdd;

    // Start is called before the first frame update
    void Start()
    {
        var tree = templateToAdd.CloneTree();
        VisualElement window = null;
        foreach(var x in tree.Children())
        {
            window = x; 
            break;
        }
        
        UIDocument doc = GetComponent<UIDocument>();   
        doc.rootVisualElement.Add(window);
        window.style.left = new StyleLength(new Length(200, LengthUnit.Pixel));
        doc.rootVisualElement.Add(new TestVisualElement());
    }
}
