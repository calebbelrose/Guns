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
    [SerializeField] private RectTransform PocketParent;
    [SerializeField] private GameObject SlotPrefab;
    [SerializeField] private Inventory Inventory;

    private List<GameObject> InspectWindows = new List<GameObject>();

    //Loads inventory and hides the canvas
    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject obj = Instantiate(SlotPrefab, PocketParent);
            SlotScript slotScript = obj.GetComponent<SlotScript>();

            obj.transform.name = "slot[0,0]";
            slotScript.InventorySlotInfo = Inventory.slotGridList[1].List[i].SlotInfo[0, 0];
            slotScript.InventorySlotInfo.SlotScript = slotScript;
        }

        DragParent = dragParent;
        DropParent = dropParent;

    }

    private void Update()
    {
        if (Inventory.HighlightedSlot != null)
        {
            Inventory inventory = Inventory.HighlightedSlot.InventorySlotInfo.SlotGrid.Inventory;

            if (Input.GetMouseButtonUp(0))
            {
                //If alt is held down equip item
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    //Uses item in slot
                    if (Inventory.HighlightedSlot.InventorySlotInfo.ItemScript != null)
                    {
                        ItemScript itemScript = inventory.GetItem(Inventory.HighlightedSlot.InventorySlotInfo);

                        for (int y = 0; y < Inventory.HighlightedSlot.InventorySlotInfo.ItemScript.Size.y; y++)
                        {
                            for (int x = 0; x < Inventory.HighlightedSlot.InventorySlotInfo.ItemScript.Size.x; x++)
                                Inventory.HighlightedSlot.InventorySlotInfo.SlotGrid.SlotInfo[Inventory.HighlightedSlot.InventorySlotInfo.ItemStartPos.x + x, Inventory.HighlightedSlot.InventorySlotInfo.ItemStartPos.y + y].SlotScript.Image.color = Color.white;
                        }
                    }
                }
                //If an item is picked up
                else if (ItemScript.selectedItem != null)
                {
                    if (!inventory.IsOverEdge)
                    {
                        switch (inventory.CheckState)
                        {
                            case 0: //Store on empty slots
                                inventory.StoreItem(Inventory.HighlightedSlot.InventorySlotInfo.SlotGrid, ItemScript.selectedItem);
                                inventory.PlaceItem(Inventory.HighlightedSlot.InventorySlotInfo.SlotGrid.SlotInfo[Inventory.HighlightedSlot.InventorySlotInfo.ItemStartPos.x, Inventory.HighlightedSlot.InventorySlotInfo.ItemStartPos.y], ItemScript.selectedItem);
                                Inventory.ColorChangeLoop(Inventory.HighlightedSlot.InventorySlotInfo.SlotGrid, Color.white, ItemScript.selectedItemSize, inventory.totalOffset);
                                ItemScript.ResetSelectedItem();
                                break;
                            case 1: //Swap items
                                ItemScript.SwapSelectedItem(SwapItem(ItemScript.selectedItem));
                                Inventory.ColorChangeLoop(Inventory.HighlightedSlot.InventorySlotInfo.SlotGrid, Color.white, inventory.OtherItems(0).Item.Size, inventory.OtherItems(0).StartPosition); //*1
                                inventory.RefreshColor(true);
                                break;
                        }
                    }
                }
                //If the slot has an item
                else
                {
                    if (Inventory.HighlightedSlot.InventorySlotInfo.ItemScript != null)
                    {
                        Inventory.ColorChangeLoop(Inventory.HighlightedSlot.InventorySlotInfo.SlotGrid, Color.white, Inventory.HighlightedSlot.InventorySlotInfo.ItemScript.Size, Inventory.HighlightedSlot.InventorySlotInfo.ItemStartPos);
                        ItemScript.SetSelectedItem(inventory.GetItem(Inventory.HighlightedSlot.InventorySlotInfo));
                        inventory.RefreshColor(true);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(1) && Inventory.HighlightedSlot.InventorySlotInfo.ItemScript != null)
                Inventory.HighlightedSlot.InventorySlotInfo.ItemScript.Inspect(InspectParent);
        }
    }

    //Swaps picked up item with specified item
    public ItemScript SwapItem(ItemScript item)
    {
        ItemScript tempItem;

        tempItem = Inventory.GetItem(Inventory.HighlightedSlot.InventorySlotInfo.SlotGrid.SlotInfo[Inventory.OtherItems(0).StartPosition.x, Inventory.OtherItems(0).StartPosition.y]);
        Inventory.StoreItem(Inventory.HighlightedSlot.InventorySlotInfo.SlotGrid, item);
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