using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotGrid
{
    public Inventory Inventory { get { return inventory; } }
    public IntVector2 GridSize { get { return gridSize; } }
    public InventorySlotInfo[,] SlotInfo;

    [SerializeField] private Inventory inventory;
    [SerializeField] private IntVector2 gridSize;

    public static float SlotSize { get; private set; } = 50f;

    //Creates inventory slots
    public SlotGrid(int x, int y, Inventory _inventory)
    {
        gridSize = new IntVector2(x, y);
        SlotInfo = new InventorySlotInfo[gridSize.x, gridSize.y];
        inventory = _inventory;

        for (int i = 0; i < gridSize.y; i++)
        {
            for (int j = 0; j < gridSize.x; j++)
                SlotInfo[j,i] = new InventorySlotInfo(new IntVector2(j, i), null, IntVector2.Zero, this);
        }
    }

    public void Display(GameObject slotPrefab, Transform parent)
    {
        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                GameObject obj = UnityEngine.Object.Instantiate(slotPrefab, parent);
                SlotScript slotScript = obj.GetComponent<SlotScript>();

                obj.transform.name = "slot[" + x + "," + y + "]";
                slotScript.InventorySlotInfo = SlotInfo[x, y];
                slotScript.InventorySlotInfo.SlotScript = slotScript;
                slotScript.Rect.localPosition = new Vector3(x * SlotSize, -(y * SlotSize), 0);
                slotScript.Rect.sizeDelta = new Vector2(SlotSize, SlotSize);
                slotScript.Rect.localScale = Vector3.one;

                if (slotScript.InventorySlotInfo.ItemScript != null)
                {
                    slotScript.InventorySlotInfo.ItemScript.transform.SetParent(slotScript.transform);
                    slotScript.InventorySlotInfo.ItemScript.Rect.localScale = Vector3.one;
                    slotScript.InventorySlotInfo.ItemScript.Slot = slotScript.InventorySlotInfo.SlotScript;
                    slotScript.InventorySlotInfo.ItemScript.Rect.position = slotScript.InventorySlotInfo.SlotScript.Rect.position;
                    Inventory.ColorChangeLoop(slotScript.InventorySlotInfo.SlotGrid, Color.white, slotScript.InventorySlotInfo.ItemScript.Size, slotScript.InventorySlotInfo.GridPos);
                }
            }
        }
    }
}
