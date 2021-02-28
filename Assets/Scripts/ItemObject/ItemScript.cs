using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    public ItemClass Item { get; private set; }
    public CanvasGroup CanvasGroup{ get { return canvasGroup; } }
    public RectTransform Rect { get { return rect; } }
    public Image Image { get { return image; } }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image image;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Text text;
    [SerializeField] private GameObject InspectPrefab;

    private int quantity;
    private GameObject InspectWindow = null;

    public static ItemScript selectedItem;
    public static IntVector2 selectedItemSize;
    public static bool isDragging = false;
    public static Vector2 DragPivot = new Vector2(0.5f, 0.5f);
    public static Vector2 DropPivot = new Vector2(0.0f, 1.0f);

    //Sets up the object
    public void SetItemObject(ItemClass passedItem)
    {
        Item = passedItem;
        rect.sizeDelta = new Vector2(Item.Size.x * SlotGrid.SlotSize, Item.Size.y * SlotGrid.SlotSize);
        image.sprite = passedItem.Icon;
    }

    public void SetQuantity(int _quantity)
    {
        quantity = _quantity;
        text.text = quantity.ToString();
    }

    //Moves picked up item to cursor
    private void Update()
    {
        if (isDragging)
            selectedItem.transform.position = Input.mousePosition;
    }

    public void Inspect(Transform parent)
    {
        if (InspectWindow == null)
        {
            InspectWindow = Instantiate(InspectPrefab, parent);
            InspectWindow.GetComponent<Inspect>().Setup(Item);
        }
        else
        {
            InspectWindow.SetActive(true);
            InspectWindow.transform.localPosition = Vector3.zero;
        }
    }

    //Sets picked up item
    public static void SetSelectedItem(ItemScript itemScript)
    {
        selectedItem = itemScript;
        selectedItemSize = itemScript.Item.Size;
        isDragging = true;
        itemScript.transform.SetParent(InventoryManager.DragParent);
        itemScript.rect.localScale = Vector3.one;
        selectedItem.CanvasGroup.alpha = 0.5f;
        selectedItem.Rect.pivot = DragPivot;
    }

    public static void SwapSelectedItem(ItemScript itemScript)
    {
        selectedItem.CanvasGroup.alpha = 1.0f;
        selectedItem.Rect.pivot = DropPivot;
        SetSelectedItem(itemScript);
    }

    //Resets picked up item
    public static void ResetSelectedItem()
    {
        selectedItem.CanvasGroup.alpha = 1.0f;
        selectedItem.Rect.pivot = DropPivot;
        selectedItem = null;
        selectedItemSize = IntVector2.Zero;
        isDragging = false;
    }
}