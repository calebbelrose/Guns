using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static Transform DragParent { get; private set; }
    public static Transform DropParent { get; private set; }

    [SerializeField] private Transform dropParent;
    [SerializeField] private Transform dragParent;
    [SerializeField] private Transform InspectParent;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private Inventory Inventory;

    private List<GameObject> InspectWindows = new List<GameObject>();

    //Loads inventory and hides the canvas
    private void Start()
    {
        DragParent = dragParent;
        DropParent = dropParent;
        Canvas.SetActive(false);
    }

    private void Update()
    {
        if (Inventory.HighlightedSlot != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                //If alt is held down equip item
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    //Uses item in slot
                    if (Inventory.HighlightedSlot.IsOccupied)
                    {
                        ItemScript itemScript = Inventory.GetItem(Inventory.HighlightedSlot);

                        for (int y = 0; y < Inventory.HighlightedSlot.ItemScript.Size.y; y++)
                        {
                            for (int x = 0; x < Inventory.HighlightedSlot.ItemScript.Size.x; x++)
                                Inventory.HighlightedSlot.SlotGrid.Slots[Inventory.HighlightedSlot.ItemStartPos.x + x, Inventory.HighlightedSlot.ItemStartPos.y + y].Image.color = Color.white;
                        }
                    }
                }
                //If an item is picked up
                else if (ItemScript.selectedItem != null)
                {
                    if (!Inventory.IsOverEdge)
                    {
                        switch (Inventory.CheckState)
                        {
                            case 0: //Store on empty slots
                                Inventory.StoreItem(Inventory.HighlightedSlot.SlotGrid, ItemScript.selectedItem);
                                Inventory.ColorChangeLoop(Inventory.HighlightedSlot.SlotGrid, Color.white, ItemScript.selectedItemSize, Inventory.totalOffset);
                                ItemScript.ResetSelectedItem();
                                break;
                            case 1: //Swap items
                                ItemScript.SwapSelectedItem(SwapItem(ItemScript.selectedItem));
                                Inventory.ColorChangeLoop(Inventory.HighlightedSlot.SlotGrid, Color.white, Inventory.OtherItems(0).Item.Size, Inventory.OtherItems(0).StartPosition); //*1
                                Inventory.RefreshColor(true);
                                break;
                        }
                    }
                }
                //If the slot has an item
                else
                {
                    if (Inventory.HighlightedSlot.IsOccupied)
                    {
                        Inventory.ColorChangeLoop(Inventory.HighlightedSlot.SlotGrid, Color.white, Inventory.HighlightedSlot.ItemScript.Size, Inventory.HighlightedSlot.ItemStartPos);
                        ItemScript.SetSelectedItem(Inventory.GetItem(Inventory.HighlightedSlot));
                        Inventory.RefreshColor(true);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(1) && Inventory.HighlightedSlot.IsOccupied)
                Inventory.HighlightedSlot.ItemScript.Inspect(InspectParent);
        }
    }

    //Swaps picked up item with specified item
    public ItemScript SwapItem(ItemScript item)
    {
        ItemScript tempItem;

        tempItem = Inventory.GetItem(Inventory.HighlightedSlot.SlotGrid.Slots[Inventory.OtherItems(0).StartPosition.x, Inventory.OtherItems(0).StartPosition.y]);
        Inventory.StoreItem(Inventory.HighlightedSlot.SlotGrid, item);
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
}