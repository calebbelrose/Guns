using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public IntVector2 totalOffset, checkStartPos;
    public SlotScript HighlightedSlot;
    public IntVector2 CheckSize;

    public bool IsOverEdge { get; private set; } = false;
    public int CheckState { get; private set; }

    public SlotGridList[] slotGridList = new SlotGridList[4];
    [SerializeField] private int pockets;

    private List<OtherItem> otherItems = new List<OtherItem>();

    private void Start()
    {
        for (int i = 0; i < pockets; i++)
            slotGridList[1].List.Add(new SlotGrid(1, 1, this));
    }

    //Returns true if the loot was stored otherwise returns false
    public bool StoreLoot(ItemScript itemScript)
    {
        foreach (SlotGridList slotGridArray in slotGridList)
        {
            foreach (SlotGrid slotGrid in slotGridArray.List)
            {
                //Loops over all of the slots to find a space big enough for the item
                for (int x = 0; x <= slotGrid.SlotInfo.GetLength(0) - itemScript.Size.x; x++)
                {
                    for (int y = 0; y <= slotGrid.SlotInfo.GetLength(1) - itemScript.Size.y; y++)
                    {
                        int i = 0;
                        bool stillEmpty = true;

                        while (i < itemScript.Size.x && stillEmpty)
                        {
                            for (int j = 0; j < itemScript.Size.y; j++)
                            {
                                if (slotGrid.SlotInfo[x + i, y + j].ItemScript != null)
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
                            StoreItem(slotGrid, itemScript);
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    //Stores item in slot
    public void StoreItem(SlotGrid slotGrid, ItemScript itemScript)
    {
        IntVector2 itemSize = itemScript.Size;

        for (int y = 0; y < itemSize.y; y++)
        {
            for (int x = 0; x < itemSize.x; x++)
                slotGrid.SlotInfo[totalOffset.x + x, totalOffset.y + y].ChangeItem(itemScript, totalOffset);
        }

        itemScript.transform.SetParent(InventoryManager.DropParent);
        itemScript.Rect.pivot = new Vector2(0.0f, 1.0f);
        itemScript.CanvasGroup.alpha = 1f;
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

                    if (instanceScript.InventorySlotInfo.ItemScript != null)
                    {
                        OtherItem otherItem = new OtherItem(instanceScript.InventorySlotInfo.ItemScript, instanceScript.InventorySlotInfo.ItemStartPos);

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
            CheckArea(ItemScript.selectedItemSize, HighlightedSlot);
            CheckState = SlotCheck(HighlightedSlot.InventorySlotInfo.SlotGrid, CheckSize);

            switch (CheckState)
            {
                case 0: ColorChangeLoop(HighlightedSlot.InventorySlotInfo.SlotGrid, Color.green, CheckSize, checkStartPos); break;
                case 1:
                    ColorChangeLoop(HighlightedSlot.InventorySlotInfo.SlotGrid, Color.white, otherItems[0].Item.Size, otherItems[0].StartPosition);
                    ColorChangeLoop(HighlightedSlot.InventorySlotInfo.SlotGrid, Color.green, CheckSize, checkStartPos);
                    break;
                default: ColorChangeLoop(HighlightedSlot.InventorySlotInfo.SlotGrid, Color.red, CheckSize, checkStartPos); break;
            }
        }
        else
        {
            IsOverEdge = false;
            ColorChangeLoop2(HighlightedSlot.InventorySlotInfo.SlotGrid, CheckSize, checkStartPos);

            foreach (OtherItem otherItem in otherItems)
                ColorChangeLoop(HighlightedSlot.InventorySlotInfo.SlotGrid, Color.white, otherItem.Item.Size, otherItem.StartPosition);
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
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
                slotGrid.SlotInfo[startPos.x + x, startPos.y + y].SlotScript.Image.color = color;
        }
    }

    //Changes slots in an area to a colour based on what item is in the slot
    public static void ColorChangeLoop2(SlotGrid slotGrid, IntVector2 size, IntVector2 startPos)
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
                slotGrid.SlotInfo[startPos.x + x, startPos.y + y].SlotScript.Image.color = Color.white;
        }
    }

    //Gets item in slot
    public ItemScript GetItem(InventorySlotInfo slotInfo)
    {
        ItemScript retItem = slotInfo.ItemScript;
        IntVector2 tempItemPos = slotInfo.ItemStartPos;
        IntVector2 itemSizeL = retItem.Size;

        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
                slotInfo.SlotGrid.SlotInfo[tempItemPos.x + x, tempItemPos.y + y].ChangeItem(null, IntVector2.Zero);
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
    private IntVector2 Offset(IntVector2 itemSize)
    {
        return new IntVector2((itemSize.x - (itemSize.x % 2 == 0 ? 0 : 1)) / 2, (itemSize.y - (itemSize.y % 2 == 0 ? 0 : 1)) / 2);
    }
}
