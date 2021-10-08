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
        Cursor = root.Q("Cursor");
        //Hide the "OS" Cursor
        UnityEngine.Cursor.visible = false;
    }

    private void Update() 
    {
        var pos = UnityEngine.Input.mousePosition;
        pos.x *= 1920f / Display.main.renderingWidth ;
        pos.y *= 1080f / Display.main.renderingHeight;
        Cursor.style.left = new StyleLength((int)pos.x);
        Cursor.style.bottom = new StyleLength((int)pos.y);
    }

    public static VisualElement InventoryGroup;
    public static VisualElement Cursor;
}