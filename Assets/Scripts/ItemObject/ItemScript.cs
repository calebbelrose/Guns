using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    public ItemClass item { get; private set; }
    public CanvasGroup CanvasGroup{ get { return canvasGroup; } }
    public RectTransform Rect { get { return rect; } }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image Image;
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
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, passedItem.Size.x * InvenGridScript.SlotSize);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, passedItem.Size.y * InvenGridScript.SlotSize);
        item = passedItem;
        Image.sprite = passedItem.Icon;
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
            InspectWindow.GetComponent<Inspect>().Setup(item);
        }
        else
            InspectWindow.transform.localPosition = Vector3.zero;
    }

    //Sets picked up item
    public static void SetSelectedItem(ItemScript obj)
    {
        selectedItem = obj;
        selectedItemSize = obj.item.Size;
        isDragging = true;
        obj.transform.SetParent(InvenGridManager.Instance.DragParent);
        obj.rect.localScale = Vector3.one;
        selectedItem.CanvasGroup.alpha = 0.5f;
        selectedItem.Rect.pivot = DragPivot;
    }

    public static void SwapSelectedItem(ItemScript obj)
    {
        selectedItem.CanvasGroup.alpha = 1.0f;
        selectedItem.Rect.pivot = DropPivot;
        SetSelectedItem(obj);
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