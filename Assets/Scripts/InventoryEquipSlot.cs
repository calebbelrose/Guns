using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryEquipSlot : EquipSlot
{
    [SerializeField] private int Index;
    [SerializeField] private RectTransform GridParent;
    [SerializeField] private Transform DropParent;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (EquipSlotInfo.Empty && eventData.button == PointerEventData.InputButton.Left && ItemScript.selectedItem != null)
        {
            ItemScript.selectedItem.transform.position = EquipSlotInfo.EquipSlot.transform.position;
            ItemScript.selectedItem.transform.SetParent(EquipSlotInfo.EquipSlot.transform);
            Inventory.SlotGridList[Index] = ItemScript.selectedItem.Item.SlotGridList;
            Inventory.EquipInSlot(ItemScript.selectedItem.Item, EquipSlotInfo);
            Display();
            GridParent.sizeDelta = Inventory.SlotGridList[Index].Size * SlotGrid.SlotSize;
            ItemScript.ResetSelectedItem();
        }
        else
        {
            Inventory.SlotGridList[Index] = null;
            Inventory.Unequip(this);

            foreach (Transform child in GridParent)
                Destroy(child.gameObject);

            foreach (Transform child in DropParent)
                Destroy(child.gameObject);
        }
    }

    public override void Display()
    {
        if (!EquipSlotInfo.Empty)
        {
            Inventory inventory;
            List<IntVector2> slots = new List<IntVector2>();

            if (ItemScript == null)
            {
                ItemScript = ItemDatabase.CreateItemScript(EquipSlotInfo.ItemClass, transform);
                ItemScript.transform.localPosition = Vector3.zero;
            }

            inventory = ItemScript.gameObject.AddComponent<Inventory>();
            inventory.SlotGridList.Add(EquipSlotInfo.ItemClass.SlotGridList);
            EquipSlotInfo.EquipSlot = this;
            GridParent.sizeDelta = Inventory.SlotGridList[Index].Size;

            for (int i = 0; i < inventory.SlotGridList[0].GridSize.y; i++)
            {
                for (int j = 0; j < inventory.SlotGridList[0].GridSize.x; j++)
                {
                    slots.Add(new IntVector2(j, i));
                }
            }

            foreach (SlotGrid slotGrid in inventory.SlotGridList[0].List)
            {
                RectTransform slotParent = Instantiate(InventoryManager.GridPrefab, GridParent).GetComponent<RectTransform>();
                int x = slots[0].x, y = slots[0].y;

                slotParent.sizeDelta = new Vector2(slotGrid.GridSize.x * SlotGrid.SlotSize, slotGrid.GridSize.y * SlotGrid.SlotSize);
                slotParent.localPosition = new Vector3(x * (SlotGrid.SlotSize + SlotGrid.SlotSpacing), -y * (SlotGrid.SlotSize + SlotGrid.SlotSpacing));

                for (int i = 0; i < slotGrid.GridSize.x; i++)
                {
                    for (int j = 0; j < slotGrid.GridSize.y; j++)
                        slots.Remove(new IntVector2(x + i, y + j));
                }

                slotGrid.Display(InventoryManager.SlotPrefab, slotParent, DropParent);
            }
        }
    }
}