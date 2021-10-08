using UnityEngine;
using UnityEngine.UIElements;
using WoW_Inventory;

namespace WoW_Inventory
{
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


        NumberInputField yep;

        private void Start()
        {
            var root = uIDocument.rootVisualElement;
            yep = new NumberInputField();
            yep.Init(root.Q<TextField>());
            InventoryGroup = root.Q("InventoryGroup");
            CursorInfo.Cursor = root.Q("Cursor");
            CursorInfo.Image = CursorInfo.Cursor.Q<Image>();
            CursorInfo.DefaultSprite = defaultCursor;
            CursorInfo.Image.sprite = defaultCursor;
            //Hide the "OS" Cursor
            UnityEngine.Cursor.visible = false;
        }

        private void Update() 
        {
            var pos = UnityEngine.Input.mousePosition;
            pos.x *= 1920f / Display.main.renderingWidth ;
            pos.y *= 1080f / Display.main.renderingHeight;
            CursorInfo.Cursor.style.left = new StyleLength((int)pos.x);
            CursorInfo.Cursor.style.bottom = new StyleLength((int)pos.y);
        }

        public static VisualElement InventoryGroup {get; private set;}
    }
}