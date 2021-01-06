using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static SlotScript HighlightedSlot;
    public static Transform DragParent { get; private set; }
    public static Transform DropParent { get; private set; }

    [SerializeField] private Transform dropParent;
    [SerializeField] private Transform dragParent;
    [SerializeField] private Transform InspectParent;
    [SerializeField] private GameObject Canvas;

    private static IntVector2 totalOffset, checkSize, checkStartPos;
    private static List<OtherItem> otherItems = new List<OtherItem>();
    private static int checkState;
    private static bool isOverEdge = false;
    private List<GameObject> InspectWindows = new List<GameObject>();
    private List<SlotGrid> slotGrids = new List<SlotGrid>();

    //Loads inventory and hides the canvas
    private void Start()
    {
        DragParent = dragParent;
        DropParent = dropParent;
        Canvas.SetActive(false);
    }

    private void Update()
    {
        if (HighlightedSlot != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                //If alt is held down equip item
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    //Uses item in slot
                    if (HighlightedSlot.IsOccupied)
                    {
                        ItemScript itemScript = GetItem(HighlightedSlot);

                        for (int y = 0; y < HighlightedSlot.ItemSize.y; y++)
                        {
                            for (int x = 0; x < HighlightedSlot.ItemSize.x; x++)
                                HighlightedSlot.SlotGrid.Slots[HighlightedSlot.ItemStartPos.x + x, HighlightedSlot.ItemStartPos.y + y].Image.color = Color.white;
                        }
                    }
                }
                //If an item is picked up
                else if (ItemScript.selectedItem != null)
                {
                    if (!isOverEdge)
                    {
                        switch (checkState)
                        {
                            case 0: //Store on empty slots
                                StoreItem(HighlightedSlot.SlotGrid, ItemScript.selectedItem);
                                ColorChangeLoop(HighlightedSlot.SlotGrid, Color.white, ItemScript.selectedItemSize, totalOffset);
                                ItemScript.ResetSelectedItem();
                                break;
                            case 1: //Swap items
                                ItemScript.SwapSelectedItem(SwapItem(ItemScript.selectedItem));
                                ColorChangeLoop(HighlightedSlot.SlotGrid, Color.white, otherItems[0].Item.Size, otherItems[0].StartPosition); //*1
                                RefreshColor(true);
                                break;
                        }
                    }
                }
                //If the slot has an item
                else
                {
                    if (HighlightedSlot.IsOccupied)
                    {
                        ColorChangeLoop(HighlightedSlot.SlotGrid, Color.white, HighlightedSlot.ItemSize, HighlightedSlot.ItemStartPos);
                        ItemScript.SetSelectedItem(GetItem(HighlightedSlot));
                        RefreshColor(true);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(1) && HighlightedSlot.IsOccupied)
                HighlightedSlot.ItemObject.Inspect(InspectParent);
        }
    }

    public void AddSlotGrid(SlotGrid slotGrid)
    {
        slotGrids.Add(slotGrid);
    }

    //Returns true if the loot was stored otherwise returns false
    public bool StoreLoot(ItemScript itemScript)
    {
        foreach (SlotGrid slotGrid in slotGrids)
        {
            //Loops over all of the slots to find a space big enough for the item
            for (int x = 0; x < slotGrid.Slots.GetLength(0) - itemScript.Size.x; x++)
            {
                for (int y = 0; y < slotGrid.Slots.GetLength(1) - itemScript.Size.y; y++)
                {
                    int i = 0;
                    bool stillEmpty = true;

                    while (i < itemScript.Size.x && stillEmpty)
                    {
                        for (int j = 0; j < itemScript.Size.y; j++)
                        {
                            if (slotGrid.Slots[x + i, y + j].IsOccupied)
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
                        totalOffset = slotGrid.Slots[x, y].GridPos;
                        StoreItem(slotGrid, itemScript);
                        itemScript.Rect.localScale = Vector3.one;
                        ColorChangeLoop(slotGrid, Color.white, itemScript.Size, totalOffset);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    //Checks if item to store is outside grid
    private static void CheckArea(IntVector2 itemSize, SlotScript slotScript) //*2
    {
        IntVector2 overCheck;

        totalOffset = slotScript.GridPos - Offset(itemSize);
        checkStartPos = totalOffset;
        checkSize = itemSize;
        overCheck = totalOffset + itemSize;
        isOverEdge = false;

        if (overCheck.x > slotScript.SlotGrid.GridSize.x)
        {
            checkSize.x = slotScript.SlotGrid.GridSize.x - totalOffset.x;
            isOverEdge = true;
        }
        if (totalOffset.x < 0)
        {
            checkSize.x = itemSize.x + totalOffset.x;
            checkStartPos.x = 0;
            isOverEdge = true;
        }
        if (overCheck.y > slotScript.SlotGrid.GridSize.y)
        {
            checkSize.y = slotScript.SlotGrid.GridSize.y - totalOffset.y;
            isOverEdge = true;
        }
        if (totalOffset.y < 0)
        {
            checkSize.y = itemSize.y + totalOffset.y;
            checkStartPos.y = 0;
            isOverEdge = true;
        }
    }

    //Checks how many items the picked up item overlaps with
    private static int SlotCheck(SlotGrid slotGrid, IntVector2 itemSize)//*2
    {
        otherItems.Clear();

        if (!isOverEdge)
        {
            for (int y = 0; y < itemSize.y; y++)
            {
                for (int x = 0; x < itemSize.x; x++)
                {
                    SlotScript instanceScript = slotGrid.Slots[checkStartPos.x + x, checkStartPos.y + y];

                    if (instanceScript.IsOccupied)
                    {
                        OtherItem otherItem = new OtherItem(instanceScript.ItemObject, instanceScript.ItemStartPos);

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
    public static void RefreshColor(bool enter)
    {
        if (enter)
        {
            CheckArea(ItemScript.selectedItemSize, HighlightedSlot);
            checkState = SlotCheck(HighlightedSlot.SlotGrid, checkSize);

            switch (checkState)
            {
                case 0: ColorChangeLoop(HighlightedSlot.SlotGrid, Color.green, checkSize, checkStartPos); break;
                case 1:
                    ColorChangeLoop(HighlightedSlot.SlotGrid, Color.white, otherItems[0].Item.Size, otherItems[0].StartPosition);
                    ColorChangeLoop(HighlightedSlot.SlotGrid, Color.green, checkSize, checkStartPos);
                    break;
                default: ColorChangeLoop(HighlightedSlot.SlotGrid, Color.red, checkSize, checkStartPos); break;
            }
        }
        else
        {
            isOverEdge = false;
            ColorChangeLoop2(HighlightedSlot.SlotGrid, checkSize, checkStartPos);

            foreach (OtherItem otherItem in otherItems)
                ColorChangeLoop(HighlightedSlot.SlotGrid, Color.white, otherItem.Item.Size, otherItem.StartPosition);
        }
    }

    //Changes slots in an area to specified colour
    public static void ColorChangeLoop(SlotGrid slotGrid, Color32 color, IntVector2 size, IntVector2 startPos)
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
                slotGrid.Slots[startPos.x + x, startPos.y + y].Image.color = color;
        }
    }

    //Changes slots in an area to a colour based on what item is in the slot
    public static void ColorChangeLoop2(SlotGrid slotGrid, IntVector2 size, IntVector2 startPos)
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
                slotGrid.Slots[startPos.x + x, startPos.y + y].Image.color = Color.white;
        }
    }

    //Stores item in slot
    private void StoreItem(SlotGrid slotGrid, ItemScript itemScript)
    {
        IntVector2 itemSize = itemScript.Size;

        for (int y = 0; y < itemSize.y; y++)
        {
            for (int x = 0; x < itemSize.x; x++)
            {
                SlotScript slotScript = slotGrid.Slots[totalOffset.x + x, totalOffset.y + y];

                slotScript.ChangeItem(itemScript, itemSize, totalOffset, itemScript.item, true);
                slotScript.Image.color = Color.white;
            }
        }

        itemScript.transform.SetParent(DropParent);
        itemScript.Rect.pivot = new Vector2(0.0f, 1.0f);
        itemScript.transform.position = slotGrid.Slots[totalOffset.x, totalOffset.y].transform.position;
        itemScript.CanvasGroup.alpha = 1f;
    }

    //Gets item in slot
    private ItemScript GetItem(SlotScript slotScript)
    {
        ItemScript retItem = slotScript.ItemObject;
        IntVector2 tempItemPos = slotScript.ItemStartPos;
        IntVector2 itemSizeL = retItem.Size;

        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
                slotScript.SlotGrid.Slots[tempItemPos.x + x, tempItemPos.y + y].ChangeItem(null, IntVector2.Zero, IntVector2.Zero, null, false);
        }
        retItem.Rect.pivot = new Vector2(0.5f, 0.5f);
        retItem.CanvasGroup.alpha = 0.5f;
        retItem.transform.position = Input.mousePosition;
        return retItem;
    }

    //Swaps picked up item with specified item
    private ItemScript SwapItem(ItemScript item)
    {
        ItemScript tempItem;

        tempItem = GetItem(HighlightedSlot.SlotGrid.Slots[otherItems[0].StartPosition.x, otherItems[0].StartPosition.y]);
        StoreItem(HighlightedSlot.SlotGrid, item);
        return tempItem;
    }

    //Gets the position of the item in the inventory
    private IntVector2 GetPosition(string line)
    {
        string[] data = line.Split(',');

        return new IntVector2(int.Parse(data[0]), int.Parse(data[1]));
    }

    //Whether the object can be stacked or not
    private bool IsStackable(CategoryName categoryName)
    {
        return categoryName == CategoryName.Material || categoryName == CategoryName.Consumable;
    }

    //Offset of the object
    private static IntVector2 Offset(IntVector2 itemSize)
    {
        return new IntVector2((itemSize.x - (itemSize.x % 2 == 0 ? 0 : 1)) / 2, (itemSize.y - (itemSize.y % 2 == 0 ? 0 : 1)) / 2);
    }
}