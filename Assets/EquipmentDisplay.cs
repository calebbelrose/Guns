using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentDisplay : MonoBehaviour
{
    public List<EquipSlot> EquipSlots;
    public List<EquipSlot> GunSlots;

    [SerializeField] private Transform PocketParent;

    public void Display()
    {
        foreach (EquipSlot equipSlot in EquipSlots)
            equipSlot.Display();

        foreach (EquipSlot gunSlot in GunSlots)
            gunSlot.Display();
    }

    public void Display(CharacterInventory inventory)
    {
        for (int i = 0; i < EquipSlots.Count; i++)
        {
            EquipSlots[i].Setup(inventory, inventory.EquipSlotInfo[i], EquipSlots[i]);
            EquipSlots[i].Display();
        }

        for (int i = 0; i < GunSlots.Count; i++)
        {
            GunSlots[i].Setup(inventory, inventory.GunSlotInfo[i], GunSlots[i]);
            GunSlots[i].Display();
        }
    }

    public void DisplayPockets(SlotGridList slotGridList)
    {
        for (int i = 0; i < slotGridList.List.Count; i++)
        {
            for (int y = 0; y < slotGridList.List[i].GridSize.y; y++)
            {
                for (int x = 0; x < slotGridList.List[i].GridSize.x; x++)
                {
                    SlotScript slotScript = UnityEngine.Object.Instantiate(InventoryManager.SlotPrefab, PocketParent).GetComponent<SlotScript>();

                    slotScript.gameObject.name = "slot[" + x + "," + y + "]";
                    slotScript.InventorySlotInfo = slotGridList.List[i].SlotInfo[x, y];
                    slotScript.InventorySlotInfo.SlotScript = slotScript;
                    slotScript.Rect.localPosition = new Vector3(x * SlotGrid.SlotSize, -(y * SlotGrid.SlotSize), 0);
                    slotScript.Rect.sizeDelta = new Vector2(SlotGrid.SlotSize, SlotGrid.SlotSize);
                    slotScript.Rect.localScale = Vector3.one;

                    if (slotScript.InventorySlotInfo.Item != null)
                    {
                        if (slotScript.InventorySlotInfo.ItemStartPos == slotScript.InventorySlotInfo.GridPos)
                        {
                            slotScript.ItemScript = ItemDatabase.CreateItemScript(slotScript.InventorySlotInfo.Item, slotScript.transform);
                            slotScript.ItemScript.transform.position = slotScript.transform.position;
                        }
                        else
                            slotScript.ItemScript = slotGridList.List[i].SlotInfo[slotScript.InventorySlotInfo.ItemStartPos.x, slotScript.InventorySlotInfo.ItemStartPos.y].SlotScript.ItemScript;

                        slotScript.InventorySlotInfo.SlotScript.Image.color = Color.white;
                    }
                }
            }
        }
    }

    public void Clear()
    {
        foreach (EquipSlot equipSlot in EquipSlots)
            equipSlot.Clear();

        foreach(EquipSlot gunSlot in GunSlots)
            gunSlot.Clear();
    }
}