using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                    //LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
                    if (slotScript.InventorySlotInfo.ItemStartPos == slotScript.InventorySlotInfo.GridPos)
                    {
                        slotScript.InventorySlotInfo.ItemScript.Slot = slotScript;
                        slotScript.InventorySlotInfo.ItemScript.transform.SetParent(InventoryManager.DropParent);
                        slotScript.InventorySlotInfo.ItemScript.Rect.localScale = Vector3.one;
                        slotScript.InventorySlotInfo.ItemScript.Image.color = Color.red;
                        slotScript.InventorySlotInfo.ItemScript.transform.position = slotScript.transform.position;
                        slotScript.InventorySlotInfo.ItemScript.Rect.sizeDelta = new Vector2(SlotGrid.SlotSize * slotScript.InventorySlotInfo.ItemScript.Size.x, SlotGrid.SlotSize * slotScript.InventorySlotInfo.ItemScript.Size.y); ;
                        Debug.Log(slotScript.InventorySlotInfo.ItemScript.Rect.sizeDelta);
                    }

                    slotScript.InventorySlotInfo.SlotScript.Image.color = Color.white;
                }
            }
        }
    }
}
