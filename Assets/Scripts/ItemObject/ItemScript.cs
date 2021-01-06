using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    public ItemClass item { get; private set; }
    public CanvasGroup CanvasGroup{ get { return canvasGroup; } }
    public RectTransform Rect { get { return rect; } }
    public IntVector2 Size { get { return size; } }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image Image;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Text text;
    [SerializeField] private GameObject InspectPrefab;

    private int quantity;
    private IntVector2 size;
    private GameObject InspectWindow = null;

    public static ItemScript selectedItem;
    public static IntVector2 selectedItemSize;
    public static bool isDragging = false;
    public static Vector2 DragPivot = new Vector2(0.5f, 0.5f);
    public static Vector2 DropPivot = new Vector2(0.0f, 1.0f);

    //Sets up the object
    public void SetItemObject(ItemClass passedItem)
    {
        item = passedItem;
        size = item.Size;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size.x * SlotGrid.SlotSize);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size.y * SlotGrid.SlotSize);
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
            InspectWindow.GetComponent<Inspect>().Setup(this);
        }
        else
            InspectWindow.transform.localPosition = Vector3.zero;
    }

    //Sets picked up item
    public static void SetSelectedItem(ItemScript obj)
    {
        selectedItem = obj;
        selectedItemSize = obj.Size;
        isDragging = true;
        obj.transform.SetParent(InventoryManager.DragParent);
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

    public void UpdateSize()
    {
        //Up Right Down Left
        int[] totalModifier = { 0, 0, 0, 0 };

        foreach (PartSize modifier in item.Part.SizeModifiers)
        {
            int index = (int)modifier.PartDirection;

            if (modifier.Size > totalModifier[index])
                totalModifier[index] = modifier.Size;
        }

        size = item.Size + new IntVector2(totalModifier[1] + totalModifier[3], totalModifier[0] + totalModifier[2]);
    }
}