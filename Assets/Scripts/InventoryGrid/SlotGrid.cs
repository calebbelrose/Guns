using UnityEngine;

public class SlotGrid : MonoBehaviour
{
    public IntVector2 GridSize { get { return gridSize; } }

    [SerializeField] private GameObject SlotPrefab;
    [SerializeField] private IntVector2 gridSize;
    [SerializeField] private RectTransform InventoryRect;
    [SerializeField] private RectTransform Parent;
    [SerializeField] private InventoryManager InventoryManager;

    public SlotScript[,] Slots;

    private float edgePadding;

    public static float SlotSize = 50f;

    //Sets up the inventory grid
    public void Start()
    {
        InventoryManager.AddSlotGrid(this);
        Slots = new SlotScript[gridSize.x, gridSize.y];
        ResizePanels();
        CreateSlots();
    }

    //Creates inventory slots
    private void CreateSlots()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                GameObject obj = (GameObject)Instantiate(SlotPrefab);
                SlotScript slotScript = obj.GetComponent<SlotScript>();
                RectTransform rect = obj.GetComponent<RectTransform>();

                obj.transform.name = "slot[" + x + "," + y + "]";
                obj.transform.SetParent(this.transform);
                rect.localPosition = new Vector3(x * SlotSize + edgePadding, -(y * SlotSize + edgePadding), 0);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SlotSize);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SlotSize);
                rect.localScale = Vector3.one;
                slotScript.Setup(new IntVector2(x, y), null, IntVector2.Zero, IntVector2.Zero, null, false, this);
                Slots[x, y] = slotScript;
            }
        }
    }

    //Resizes panel
    private void ResizePanels()
    {
        float width, height;

        width = (gridSize.x * SlotSize) + (edgePadding * 2);
        height = (gridSize.y * SlotSize) + (edgePadding * 2);
        InventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        InventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        Parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        Parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
