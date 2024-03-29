﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public IntVector2 totalOffset;
    public IntVector2 CheckSize;
    public IntVector2 CheckStartPos;
    public bool IsOverEdge { get; private set; } = false;
    public int CheckState { get; private set; }
    public List<SlotGridList> SlotGridList = new List<SlotGridList>();

    private IntVector2 checkStartPos;
    private List<OtherItem> otherItems = new List<OtherItem>();

    //Returns true if the loot was stored otherwise returns false
    public InventorySlotInfo StoreLoot(ItemClass item)
    {
        foreach (SlotGridList slotGridArray in SlotGridList)
        {
            foreach (SlotGrid slotGrid in slotGridArray.List)
            {
                int q = 0;
                //Loops over all of the slots to find a space big enough for the item
                for (int x = 0; x <= slotGrid.SlotInfo.GetLength(0) - item.Size.x; x++)
                {
                    for (int y = 0; y <= slotGrid.SlotInfo.GetLength(1) - item.Size.y; y++)
                    {
                        int i = 0;
                        bool stillEmpty = true;

                        while (i < item.Size.x && stillEmpty)
                        {
                            for (int j = 0; j < item.Size.y; j++)
                            {
                                if (!slotGrid.SlotInfo[x + i, y + j].Empty)
                                {
                                    stillEmpty = false;
                                    break;
                                }
                            }

                            i++;
                        }

                        // Stores the item if there's a space big enough
                        if (stillEmpty)
                        {
                            totalOffset.x = x;
                            totalOffset.y = y;
                            StoreItem(slotGrid, item);
                            return slotGrid.SlotInfo[totalOffset.x, totalOffset.y];
                        }
                    }
                }
            }
        }

        return null;
    }

    //Stores item in slot
    public void StoreItem(SlotGrid slotGrid, ItemClass item)
    {
        IntVector2 itemSize = item.Size;

        for (int y = 0; y < itemSize.y; y++)
        {
            for (int x = 0; x < itemSize.x; x++)
                slotGrid.SlotInfo[totalOffset.x + x, totalOffset.y + y].ChangeItem(item, totalOffset);
        }
    }

    public void PlaceItem(InventorySlotInfo slotInfo, ItemScript itemScript, Transform parent)
    {
        /*itemScript.Rect.sizeDelta = new Vector2(SlotGrid.SlotSize * slotInfo.Item.Size.x, SlotGrid.SlotSize * slotInfo.Item.Size.y);
        itemScript.Rect.position = slotInfo.SlotScript.Rect.position;
        slotInfo.SlotScript.ItemScript = itemScript;
        itemScript.Image.color = Color.red;
        itemScript.Rect.localScale = Vector3.one;*/
        itemScript.transform.SetParent(parent);
        itemScript.Rect.position = slotInfo.SlotScript.Rect.position;

        for (int x = 0; x < slotInfo.Item.Size.x; x++)
        {
            for (int y = 0; y < slotInfo.Item.Size.y; y++)
                slotInfo.SlotGrid.SlotInfo[slotInfo.GridPos.x + x, slotInfo.GridPos.y + y].SlotScript.ItemScript = itemScript;
        }    

        slotInfo.SlotScript.ItemScript.Item.Slot = slotInfo.SlotScript;
    }

    //Checks how many items the picked up item overlaps with
    private int SlotCheck(SlotGrid slotGrid, IntVector2 itemSize)//*2
    {
        otherItems.Clear();

        if (!IsOverEdge)
        {
            for (int y = 0; y < itemSize.y; y++)
            {
                for (int x = 0; x < itemSize.x; x++)
                {
                    SlotScript instanceScript = slotGrid.SlotInfo[checkStartPos.x + x, checkStartPos.y + y].SlotScript;

                    if (instanceScript.InventorySlotInfo.Item != null)
                    {
                        OtherItem otherItem = new OtherItem(instanceScript.InventorySlotInfo.Item, instanceScript.InventorySlotInfo.ItemStartPos);

                        if (!otherItems.Contains(otherItem))
                            otherItems.Add(otherItem);
                    }
                }
            }
            return otherItems.Count;
        }
        return 2;
    }

    //Changes the colour of the slot based on how many items are overlapping or if the picked up item is over the edge
    public void RefreshColor(bool enter)
    {
        if (enter)
        {
            CheckArea(ItemScript.selectedItemSize, InventoryManager.HighlightedSlot);
            CheckState = SlotCheck(InventoryManager.HighlightedSlot.InventorySlotInfo.SlotGrid, CheckSize);

            switch (CheckState)
            {
                case 0: ColorChangeLoop(InventoryManager.HighlightedSlot.InventorySlotInfo.SlotGrid, Color.green, CheckSize, checkStartPos); break;
                case 1:
                    ColorChangeLoop(InventoryManager.HighlightedSlot.InventorySlotInfo.SlotGrid, Color.white, otherItems[0].Item.Size, otherItems[0].StartPosition);
                    ColorChangeLoop(InventoryManager.HighlightedSlot.InventorySlotInfo.SlotGrid, Color.green, CheckSize, checkStartPos);
                    break;
                default: ColorChangeLoop(InventoryManager.HighlightedSlot.InventorySlotInfo.SlotGrid, Color.red, CheckSize, checkStartPos); break;
            }
        }
        else
        {
            IsOverEdge = false;
            ColorChangeLoop2(InventoryManager.HighlightedSlot.InventorySlotInfo.SlotGrid, CheckSize, checkStartPos);

            foreach (OtherItem otherItem in otherItems)
                ColorChangeLoop(InventoryManager.HighlightedSlot.InventorySlotInfo.SlotGrid, Color.white, otherItem.Item.Size, otherItem.StartPosition);
        }
    }

    //Checks if item to store is outside grid
    private void CheckArea(IntVector2 itemSize, SlotScript slotScript) //*2
    {
        IntVector2 overCheck;

        totalOffset = slotScript.InventorySlotInfo.GridPos - Offset(itemSize);
        checkStartPos = totalOffset;
        CheckSize = itemSize;
        overCheck = totalOffset + itemSize;
        IsOverEdge = false;

        if (overCheck.x > slotScript.InventorySlotInfo.SlotGrid.GridSize.x)
        {
            CheckSize.x = slotScript.InventorySlotInfo.SlotGrid.GridSize.x - totalOffset.x;
            IsOverEdge = true;
        }
        if (totalOffset.x < 0)
        {
            CheckSize.x = itemSize.x + totalOffset.x;
            checkStartPos.x = 0;
            IsOverEdge = true;
        }
        if (overCheck.y > slotScript.InventorySlotInfo.SlotGrid.GridSize.y)
        {
            CheckSize.y = slotScript.InventorySlotInfo.SlotGrid.GridSize.y - totalOffset.y;
            IsOverEdge = true;
        }
        if (totalOffset.y < 0)
        {
            CheckSize.y = itemSize.y + totalOffset.y;
            checkStartPos.y = 0;
            IsOverEdge = true;
        }
    }

    //Changes slots in an area to specified colour
    public static void ColorChangeLoop(SlotGrid slotGrid, Color32 color, IntVector2 size, IntVector2 startPos)
    {
        for (int y = 0; y < size.y && startPos.y + y < slotGrid.SlotInfo.GetLength(1); y++)
        {
            for (int x = 0; x < size.x && startPos.x + x < slotGrid.SlotInfo.GetLength(0); x++)
                slotGrid.SlotInfo[startPos.x + x, startPos.y + y].SlotScript.Image.color = color;
        }
    }

    //Changes slots in an area to a colour based on what item is in the slot
    public static void ColorChangeLoop2(SlotGrid slotGrid, IntVector2 size, IntVector2 startPos)
    {
        for (int y = 0; y < size.y && startPos.y +y < slotGrid.SlotInfo.GetLength(1); y++)
        {
            for (int x = 0; x < size.x && startPos.x + x < slotGrid.SlotInfo.GetLength(0); x++)
                slotGrid.SlotInfo[startPos.x + x, startPos.y + y].SlotScript.Image.color = Color.white;
        }
    }

    //Gets item in slot
    public ItemScript GetItem(InventorySlotInfo slotInfo)
    {
        ItemScript retItem = slotInfo.SlotScript.ItemScript;
        IntVector2 tempItemPos = slotInfo.ItemStartPos, itemSizeL = retItem.Item.Size;

        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
            {
                slotInfo.SlotGrid.SlotInfo[tempItemPos.x + x, tempItemPos.y + y].RemoveItem();
                slotInfo.SlotGrid.SlotInfo[tempItemPos.x + x, tempItemPos.y + y].SlotScript.ItemScript = null;
            }
        }

        retItem.Rect.pivot = new Vector2(0.5f, 0.5f);
        retItem.CanvasGroup.alpha = 0.5f;
        retItem.transform.position = Input.mousePosition;
        return retItem;
    }

    public OtherItem OtherItems(int index)
    {
        return otherItems[index];
    }

    //Offset of the object
    public static IntVector2 Offset(IntVector2 itemSize)
    {
        return new IntVector2((itemSize.x - (itemSize.x % 2 == 0 ? 0 : 1)) / 2, (itemSize.y - (itemSize.y % 2 == 0 ? 0 : 1)) / 2);
    }
}