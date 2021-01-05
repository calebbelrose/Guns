using UnityEngine;

public class InvenGridScript : MonoBehaviour
{
    [SerializeField] private GameObject SlotPrefab;
    [SerializeField] private IntVector2 GridSize;
    [SerializeField] private InvenGridManager InvenGridManager;
    [SerializeField] private RectTransform InventoryRect;
    [SerializeField] private RectTransform DropRect;
    [SerializeField] private RectTransform Parent;

    private float edgePadding;

    public static float SlotSize = 50f;

    //Sets up the inventory grid
    public void Start()
    {
        InvenGridManager.SlotGrid = new SlotScript[GridSize.x, GridSize.y];
        ResizePanels();
        CreateSlots();
    }

    //Creates inventory slots
    private void CreateSlots()
    {
        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
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
                slotScript.gridPos = new IntVector2(x, y);
                InvenGridManager.SlotGrid[x, y] = slotScript;
            }
        }
    }

    //Resizes panel
    private void ResizePanels()
    {
        float width, height;

        width = (GridSize.x * SlotSize) + (edgePadding * 2);
        height = (GridSize.y * SlotSize) + (edgePadding * 2);
        InventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        InventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        DropRect.sizeDelta = InventoryRect.sizeDelta;
        Parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        Parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
