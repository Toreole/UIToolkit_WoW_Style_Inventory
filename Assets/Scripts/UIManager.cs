using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    [SerializeField]
    private UIDocument uIDocument;

    private void Awake() 
    {
        if(instance)
            Destroy(this.gameObject);
        else
        {
            instance = this;
        }
        if(!uIDocument)
            uIDocument = GetComponent<UIDocument>();
    }

    private void Start()
    {
        var root = uIDocument.rootVisualElement;
        InventoryGroup = root.Q("InventoryGroup");
    }

    public static VisualElement InventoryGroup;
}