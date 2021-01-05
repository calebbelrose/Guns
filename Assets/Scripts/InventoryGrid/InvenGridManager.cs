using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InvenGridManager : MonoBehaviour
{
    public SlotScript[,] SlotGrid;
    public static SlotScript HighlightedSlot;
    public static Transform DragParent { get; private set; }

    [SerializeField] private Transform DropParent;
    [SerializeField] private Transform dragParent;
    [SerializeField] private Transform InspectParent;
    [SerializeField] private GameObject Canvas;

    private static IntVector2 totalOffset, checkSize, checkStartPos;
    private List<OtherItem> otherItems = new List<OtherItem>();
    private static int checkState;
    private static bool isOverEdge = false;
    private List<GameObject> InspectWindows = new List<GameObject>();

    //Loads inventory and hides the canvas
    private void Start()
    {
        DragParent = dragParent;
        DontDestroyOnLoad(transform.parent.parent.gameObject);
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
                    if (HighlightedSlot.isOccupied)
                    {
                        ItemScript itemScript = GetItem(HighlightedSlot);

                        for (int y = 0; y < HighlightedSlot.storedItemSize.y; y++)
                        {
                            for (int x = 0; x < HighlightedSlot.storedItemSize.x; x++)
                                SlotGrid[HighlightedSlot.storedItemStartPos.x + x, HighlightedSlot.storedItemStartPos.y + y].GetComponent<Image>().color = Color.white;
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
                                StoreItem(ItemScript.selectedItem);
                                ColorChangeLoop(Color.white, ItemScript.selectedItemSize, totalOffset);
                                ItemScript.ResetSelectedItem();
                                break;
                            case 1: //Swap items
                                ItemScript.SwapSelectedItem(SwapItem(ItemScript.selectedItem));
                                ColorChangeLoop(Color.white, otherItems[0].Item.item.Size, otherItems[0].StartPosition); //*1
                                RefreshColor(true);
                                break;
                        }
                    }
                }
                //If the slot has an item
                else
                {
                    if (HighlightedSlot.isOccupied)
                    {
                        ColorChangeLoop(Color.white, HighlightedSlot.storedItemSize, HighlightedSlot.storedItemStartPos);
                        ItemScript.SetSelectedItem(GetItem(HighlightedSlot));
                        RefreshColor(true);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(1) && HighlightedSlot.isOccupied)
                HighlightedSlot.storedItemObject.Inspect(InspectParent);
        }
    }

    //Returns true if the loot was stored otherwise returns false
    public bool StoreLoot(ItemScript itemScript)
    {
        //Loops over all of the slots to find a space big enough for the item
        for (int x = 0; x < SlotGrid.GetLength(0) - itemScript.item.Size.x; x++)
        {
            for (int y = 0; y < SlotGrid.GetLength(1) - itemScript.item.Size.y; y++)
            {
                int i = 0;
                bool stillEmpty = true;

                while(i < itemScript.item.Size.x && stillEmpty)
                {
                    for (int j = 0; j < itemScript.item.Size.y; j++)
                    {
                        if (SlotGrid[x + i, y + j].isOccupied)
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
                    totalOffset = SlotGrid[x, y].gridPos;
                    StoreItem(itemScript);
                    itemScript.Rect.localScale = Vector3.one;
                    ColorChangeLoop(Color.white, itemScript.item.Size, totalOffset);
                    return true;
                }
            }
        }

        return false;
    }

    //Checks if item to store is outside grid
    private static void CheckArea(SlotGrid slotGrid, IntVector2 itemSize, SlotScript slotScript) //*2
    {
        IntVector2 overCheck;

        totalOffset = slotScript.gridPos - Offset(itemSize);
        checkStartPos = totalOffset;
        checkSize = itemSize;
        overCheck = totalOffset + itemSize;
        isOverEdge = false;

        if (overCheck.x > GridSize.x)
        {
            checkSize.x = GridSize.x - totalOffset.x;
            isOverEdge = true;
        }
        if (totalOffset.x < 0)
        {
            checkSize.x = itemSize.x + totalOffset.x;
            checkStartPos.x = 0;
            isOverEdge = true;
        }
        if (overCheck.y > GridSize.y)
        {
            checkSize.y = GridSize.y - totalOffset.y;
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
    private int SlotCheck(IntVector2 itemSize)//*2
    {
        otherItems.Clear();

        if (!isOverEdge)
        {
            for (int y = 0; y < itemSize.y; y++)
            {
                for (int x = 0; x < itemSize.x; x++)
                {
                    SlotScript instanceScript = SlotGrid[checkStartPos.x + x, checkStartPos.y + y];

                    if (instanceScript.isOccupied)
                    {
                        OtherItem otherItem = new OtherItem(instanceScript.storedItemObject, instanceScript.storedItemStartPos);

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
    public static void RefreshColor(SlotGrid slotGrid, bool enter)
    {
        if (enter)
        {
            CheckArea(ItemScript.selectedItemSize, HighlightedSlot);
            checkState = SlotCheck(checkSize);

            switch (checkState)
            {
                case 0: ColorChangeLoop(Color.green, checkSize, checkStartPos); break;
                case 1:
                    ColorChangeLoop(Color.white, otherItems[0].Item.item.Size, otherItems[0].StartPosition);
                    ColorChangeLoop(Color.green, checkSize, checkStartPos);
                    break;
                default: ColorChangeLoop(Color.red, checkSize, checkStartPos); break;
            }
        }
        else
        {
            isOverEdge = false;
            ColorChangeLoop2(checkSize, checkStartPos);

            foreach (OtherItem otherItem in otherItems)
                ColorChangeLoop(Color.white, otherItem.Item.item.Size, otherItem.StartPosition);
        }
    }

    //Changes slots in an area to specified colour
    public void ColorChangeLoop(Color32 color, IntVector2 size, IntVector2 startPos)
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
                SlotGrid[startPos.x + x, startPos.y + y].GetComponent<Image>().color = color;
        }
    }

    //Changes slots in an area to a colour based on what item is in the slot
    public void ColorChangeLoop2(IntVector2 size, IntVector2 startPos)
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
                SlotGrid[startPos.x + x, startPos.y + y].GetComponent<Image>().color = Color.white;
        }
    }

    //Stores item in slot
    private void StoreItem(ItemScript itemScript)
    {
        SlotScript instanceScript;
        IntVector2 itemSizeL = itemScript.item.Size;

        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
            {
                instanceScript = SlotGrid[totalOffset.x + x, totalOffset.y + y];
                instanceScript.storedItemObject = itemScript;
                instanceScript.storedItemClass = itemScript.item;
                instanceScript.storedItemSize = itemSizeL;
                instanceScript.storedItemStartPos = totalOffset;
                instanceScript.isOccupied = true;
                SlotGrid[totalOffset.x + x, totalOffset.y + y].GetComponent<Image>().color = Color.white;
            }
        }

        itemScript.transform.SetParent(DropParent);
        itemScript.Rect.pivot = new Vector2(0.0f, 1.0f);
        itemScript.transform.position = SlotGrid[totalOffset.x, totalOffset.y].transform.position;
        itemScript.CanvasGroup.alpha = 1f;
    }

    //Gets item in slot
    private ItemScript GetItem(SlotScript slotScript)
    {
        ItemScript retItem = slotScript.storedItemObject;
        IntVector2 tempItemPos = slotScript.storedItemStartPos;
        IntVector2 itemSizeL = retItem.item.Size;
        SlotScript instanceScript;

        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
            {
                instanceScript = SlotGrid[tempItemPos.x + x, tempItemPos.y + y];
                instanceScript.storedItemObject = null;
                instanceScript.storedItemSize = IntVector2.Zero;
                instanceScript.storedItemStartPos = IntVector2.Zero;
                instanceScript.storedItemClass = null;
                instanceScript.isOccupied = false;
            }
        }

        retItem.Rect.pivot = ItemScript.DragPivot;
        retItem.CanvasGroup.alpha = 0.5f;
        retItem.transform.position = Input.mousePosition;
        return retItem;
    }

    //Swaps picked up item with specified item
    private ItemScript SwapItem(ItemScript item)
    {
        ItemScript tempItem;

        tempItem = GetItem(SlotGrid[otherItems[0].StartPosition.x, otherItems[0].StartPosition.y]);
        StoreItem(item);
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
    private IntVector2 Offset(IntVector2 itemSize)
    {
        return new IntVector2((itemSize.x - (itemSize.x % 2 == 0 ? 0 : 1)) / 2, (itemSize.y - (itemSize.y % 2 == 0 ? 0 : 1)) / 2);
    }
}