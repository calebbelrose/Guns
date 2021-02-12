using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static Transform DragParent { get; private set; }
    public static Transform DropParent { get; private set; }
    public static GameObject SlotPrefab { get; private set; }
    public static Text LootText { get; private set; }
    public static SlotScript HighlightedSlot;

    [SerializeField] private Transform dropParent;
    [SerializeField] private Transform dragParent;
    [SerializeField] private Transform InspectParent;
    [SerializeField] private CharacterInventory CharacterInventory;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Text lootText;

    private List<GameObject> InspectWindows = new List<GameObject>();

    //Loads inventory and hides the canvas
    private void Start()
    {
        SlotPrefab = slotPrefab;
        DragParent = dragParent;
        DropParent = dropParent;
        LootText = lootText;
    }

    private void Update()
    {
        if (HighlightedSlot != null)
        {
            Inventory inventory = HighlightedSlot.InventorySlotInfo.SlotGrid.Inventory;

            if (Input.GetMouseButtonUp(0))
            {
                //If alt is held down equip item
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    //Uses item in slot
                    if (HighlightedSlot.InventorySlotInfo.Item != null)
                    {
                        ItemScript itemScript = inventory.GetItem(HighlightedSlot.InventorySlotInfo);

                        for (int y = 0; y < HighlightedSlot.InventorySlotInfo.Item.Size.y; y++)
                        {
                            for (int x = 0; x < HighlightedSlot.InventorySlotInfo.Item.Size.x; x++)
                                HighlightedSlot.InventorySlotInfo.SlotGrid.SlotInfo[HighlightedSlot.InventorySlotInfo.ItemStartPos.x + x, HighlightedSlot.InventorySlotInfo.ItemStartPos.y + y].SlotScript.Image.color = Color.white;
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
                                inventory.StoreItem(HighlightedSlot.InventorySlotInfo.SlotGrid, ItemScript.selectedItem.Item);
                                inventory.PlaceItem(HighlightedSlot.InventorySlotInfo.SlotGrid.SlotInfo[HighlightedSlot.InventorySlotInfo.ItemStartPos.x, HighlightedSlot.InventorySlotInfo.ItemStartPos.y], ItemScript.selectedItem);
                                Inventory.ColorChangeLoop(HighlightedSlot.InventorySlotInfo.SlotGrid, Color.white, ItemScript.selectedItemSize, inventory.totalOffset);
                                ItemScript.ResetSelectedItem();
                                break;
                            case 1: //Swap items
                                ItemScript.SwapSelectedItem(SwapItem(ItemScript.selectedItem.Item));
                                Inventory.ColorChangeLoop(HighlightedSlot.InventorySlotInfo.SlotGrid, Color.white, inventory.OtherItems(0).Item.Size, inventory.OtherItems(0).StartPosition); //*1
                                inventory.RefreshColor(true);
                                break;
                        }
                    }
                }
                //If the slot has an item
                else
                {
                    if (HighlightedSlot.InventorySlotInfo.Item != null)
                    {
                        Inventory.ColorChangeLoop(HighlightedSlot.InventorySlotInfo.SlotGrid, Color.white, HighlightedSlot.InventorySlotInfo.Item.Size, HighlightedSlot.InventorySlotInfo.ItemStartPos);
                        ItemScript.SetSelectedItem(inventory.GetItem(HighlightedSlot.InventorySlotInfo));
                        inventory.RefreshColor(true);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(1) && HighlightedSlot.InventorySlotInfo.Item != null)
                HighlightedSlot.ItemScript.Inspect(InspectParent);
        }
    }

    //Swaps picked up item with specified item
    public ItemScript SwapItem(ItemClass item)
    {
        ItemScript tempItem;

        tempItem = CharacterInventory.GetItem(HighlightedSlot.InventorySlotInfo.SlotGrid.SlotInfo[CharacterInventory.OtherItems(0).StartPosition.x, CharacterInventory.OtherItems(0).StartPosition.y]);
        CharacterInventory.StoreItem(HighlightedSlot.InventorySlotInfo.SlotGrid, item);
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