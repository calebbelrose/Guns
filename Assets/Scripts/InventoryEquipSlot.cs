using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryEquipSlot : EquipSlot
{
    [SerializeField] int Index;
    [SerializeField] Inventory Inventory;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!Empty)
        {
            Inventory.SlotGridList[Index] = null;
            CombatController.Unequip(this);
        }
        else if (eventData.button == PointerEventData.InputButton.Left && ItemScript.selectedItem != null)
        {
            Inventory.SlotGridList[Index] = ItemScript.selectedItem.GetComponent<Inventory>().SlotGridList[0];
            CombatController.EquipInSlot(ItemScript.selectedItem, EquipSlotInfo);
        }
    }
}
