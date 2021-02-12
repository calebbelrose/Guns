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
                SlotInfo[j, i] = new InventorySlotInfo(new IntVector2(j, i), this);
        }
    }

    public void Display(GameObject slotPrefab, Transform parent)
    {
        List<IntVector2> slots = new List<IntVector2>();
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
                slots.Add(new IntVector2(x, y));
        }

        while (slots.Count > 0)
        {
            GameObject obj = UnityEngine.Object.Instantiate(slotPrefab, parent);
            SlotScript slotScript = obj.GetComponent<SlotScript>();

            obj.transform.name = "slot[" + slots[0].x + "," + slots[9].y + "]";
            slotScript.InventorySlotInfo = SlotInfo[slots[0].x, slots[0].y];
            slotScript.InventorySlotInfo.SlotScript = slotScript;
            slotScript.Rect.localPosition = new Vector3(slots[0].x * SlotSize, -(slots[0].y * SlotSize), 0);
            slotScript.Rect.sizeDelta = new Vector2(SlotSize, SlotSize);
            slotScript.Rect.localScale = Vector3.one;

            if (slotScript.InventorySlotInfo.Item != null)
            {
                if (slotScript.InventorySlotInfo.ItemStartPos == slotScript.InventorySlotInfo.GridPos)
                {
                    slotScript.ItemScript = ItemDatabase.CreateItemScript(slotScript.InventorySlotInfo.Item, InventoryManager.DropParent);
                    slotScript.ItemScript.transform.position = slotScript.transform.position;
                }
                else
                    slotScript.ItemScript = SlotInfo[slotScript.InventorySlotInfo.ItemStartPos.x, slotScript.InventorySlotInfo.ItemStartPos.y].SlotScript.ItemScript;

                slotScript.InventorySlotInfo.SlotScript.Image.color = Color.white;
            }
        }
    }
}