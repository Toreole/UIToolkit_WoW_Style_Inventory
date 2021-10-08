using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    [SerializeField]
    private UIDocument uIDocument;
    [SerializeField]
    private Sprite defaultCursor;

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
        CursorImage = Cursor.Q<Image>();
        DefaultCursor = defaultCursor;
        CursorImage.sprite = defaultCursor;
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

    public static VisualElement InventoryGroup {get; private set;}
    public static VisualElement Cursor {get; private set;}
    public static Image CursorImage {get; private set;}
    public static Sprite DefaultCursor {get; private set;}
}