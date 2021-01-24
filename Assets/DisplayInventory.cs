using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayInventory : MonoBehaviour
{
    [SerializeField] private List<EquipSlot> EquipSlots;
    [SerializeField] private GameObject SlotPrefab;
    [SerializeField] private List<RectTransform> Parents = new List<RectTransform>();

    public void Display(CombatController combatController, Inventory inventory)
    {
        for (int i = 0; i < EquipSlots.Count; i++)
        {
            if (combatController.EquipSlot(i).ItemScript)
                Debug.Log("Test");
            else
                EquipSlots[i].EquipSlotInfo = combatController.EquipSlot(i);
        }

        for (int i = 0; i < inventory.slotGridList.Length; i++)
        {
            foreach (SlotGrid slotGrid in inventory.slotGridList[i].List)
                slotGrid.Display(SlotPrefab, Parents[i]);
        }
    }
}