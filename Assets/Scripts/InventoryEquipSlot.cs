using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryEquipSlot : EquipSlot
{
    [SerializeField] private int Index;
    [SerializeField] private RectTransform SlotParent;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (EquipSlotInfo.Empty && eventData.button == PointerEventData.InputButton.Left && ItemScript.selectedItem != null)
        {
            Debug.Log(EquipSlotInfo.EquipSlot);
            ItemScript.selectedItem.transform.position = EquipSlotInfo.EquipSlot.transform.position;
            ItemScript.selectedItem.transform.SetParent(InventoryManager.DropParent);
            Inventory.SlotGridList[Index] = ItemScript.selectedItem.GetComponent<Inventory>().SlotGridList[0];
            Inventory.EquipInSlot(ItemScript.selectedItem.Item, EquipSlotInfo);
            SlotParent.sizeDelta = Inventory.SlotGridList[Index].Size * SlotGrid.SlotSize;
            ItemScript.ResetSelectedItem();
        }
        else
        {
            Inventory.SlotGridList[Index] = null;
            Inventory.Unequip(this);
        }
    }

    public override void Display()
    {
        if (!EquipSlotInfo.Empty)
        {
            Inventory inventory;

            ItemScript = ItemDatabase.CreateItemScript(EquipSlotInfo.ItemClass, transform);
            ItemScript.transform.localPosition = Vector3.zero;
            inventory = ItemScript.gameObject.AddComponent<Inventory>();
            inventory.SlotGridList.Add(new SlotGridList());
            inventory.SlotGridList[0] = EquipSlotInfo.ItemClass.SlotGridList;
            EquipSlotInfo.EquipSlot = this;
            SlotParent.sizeDelta = Inventory.SlotGridList[Index].Size * SlotGrid.SlotSize;

            foreach (SlotGrid slotGrid in inventory.SlotGridList[0].List)
                slotGrid.Display(InventoryManager.SlotPrefab, SlotParent);
        }
    }
}