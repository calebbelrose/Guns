using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SlotGrid
{
    public Inventory Inventory { get { return inventory; } }
    public IntVector2 GridSize { get { return gridSize; } }
    public InventorySlotInfo[,] SlotInfo;

    [SerializeField] private Inventory inventory;
    [SerializeField] private IntVector2 gridSize;

    public static float SlotSize { get; } = 50f;
    public static float SlotSpacing { get; } = 5f;

    //Creates inventory slots
    public SlotGrid(int x, int y, Inventory _inventory)
    {
        gridSize = new IntVector2(x, y);
        SlotInfo = new InventorySlotInfo[gridSize.x, gridSize.y];
        inventory = _inventory;

        for (int i = 0; i < gridSize.y; i++)
        {
            for (int j = 0; j < gridSize.x; j++)
                SlotInfo[j, i] = new InventorySlotInfo(new IntVector2(j, i), this);
        }
    }

    public void Display(GameObject slotPrefab, Transform parent, Transform dropParent)
    {
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                SlotScript slotScript = UnityEngine.Object.Instantiate(slotPrefab, parent).GetComponent<SlotScript>();

                slotScript.gameObject.name = "slot[" + x + "," + y + "]";
                slotScript.InventorySlotInfo = SlotInfo[x,y];
                slotScript.InventorySlotInfo.SlotScript = slotScript;
                slotScript.Rect.localPosition = new Vector3(x * SlotSize, -(y * SlotSize), 0);
                slotScript.Rect.sizeDelta = new Vector2(SlotSize, SlotSize);
                slotScript.Rect.localScale = Vector3.one;

                if (slotScript.InventorySlotInfo.Item != null)
                {
                    if (slotScript.InventorySlotInfo.ItemStartPos == slotScript.InventorySlotInfo.GridPos)
                    {
                        slotScript.ItemScript = ItemDatabase.CreateItemScript(slotScript.InventorySlotInfo.Item, dropParent);
                        slotScript.ItemScript.transform.position = slotScript.transform.position;
                    }
                    else
                        slotScript.ItemScript = SlotInfo[slotScript.InventorySlotInfo.ItemStartPos.x, slotScript.InventorySlotInfo.ItemStartPos.y].SlotScript.ItemScript;

                    slotScript.InventorySlotInfo.SlotScript.Image.color = Color.white;
                }
            }
        }
    }
}