using UnityEngine;

public class DisplaySlots : MonoBehaviour
{
    [SerializeField] private GameObject SlotPrefab;
    [SerializeField] private RectTransform InventoryRect;
    [SerializeField] private RectTransform Parent;

    public static float SlotSize { get; private set; } = 50f;

    private float edgePadding;

    //Displays inventory slots
    private void Display(SlotGrid slotGrid)
    {
        float width, height;

        width = (slotGrid.GridSize.x * SlotSize) + (edgePadding * 2);
        height = (slotGrid.GridSize.y * SlotSize) + (edgePadding * 2);
        InventoryRect.sizeDelta = new Vector2(width, height);
        Parent.sizeDelta = new Vector2(width, height);

        for (int y = 0; y < slotGrid.GridSize.y; y++)
        {
            for (int x = 0; x < slotGrid.GridSize.x; x++)
            {
                GameObject obj = Instantiate(SlotPrefab);
                SlotScript slotScript = obj.AddComponent<SlotScript>();
                RectTransform rect = obj.GetComponent<RectTransform>();

                slotScript = slotGrid.Slots[x, y];
                obj.transform.name = "slot[" + x + "," + y + "]";
                obj.transform.SetParent(this.transform);
                rect.localPosition = new Vector3(x * SlotSize + edgePadding, -(y * SlotSize + edgePadding), 0);
                rect.sizeDelta = new Vector2(SlotSize, SlotSize);
                rect.localScale = Vector3.one;
            }
        }
    }
}
