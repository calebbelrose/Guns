using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory : Inventory
{
    public List<EquipSlotInfo> EquipSlotInfo = new List<EquipSlotInfo>();
    public List<GunSlotInfo> GunSlotInfo = new List<GunSlotInfo>();

    [SerializeField] private int pockets;

    private void Start()
    {
        for (int i = 0; i < pockets; i++)
            SlotGridList[1].List.Add(new SlotGrid(1, 1, this));
    }

    //Equips item
    public void Equip(ItemClass itemClass)
    {
        EquipSlotInfo slotInfo = FindSlot(itemClass);

        if (slotInfo != null)
            EquipInSlot(itemClass, slotInfo);
    }

    //Equips item in slot
    public void EquipInSlot(ItemClass itemClass, EquipSlotInfo equipSlotInfo)
    {
        if (equipSlotInfo.CanEquip(itemClass))
        {
            bool wasEmpty = equipSlotInfo.Empty;

            equipSlotInfo.ItemClass = itemClass;
            equipSlotInfo.Empty = false;

            if (equipSlotInfo.EquipSlot != null)
                equipSlotInfo.EquipSlot.Display();
        }
    }

    //Unequips item
    public void Unequip(EquipSlot equipSlot)
    {
        Debug.Log(equipSlot);
        Debug.Log(equipSlot.EquipSlotInfo);
        Debug.Log(equipSlot.EquipSlotInfo.EquipSlot);
        Debug.Log(equipSlot.EquipSlotInfo.EquipSlot.ItemScript);
        ItemScript.SetSelectedItem(equipSlot.EquipSlotInfo.EquipSlot.ItemScript);
        equipSlot.EquipSlotInfo.Empty = true;
    }

    public EquipSlotInfo FindSlot(ItemClass itemClass)
    {
        return EquipSlotInfo.Find(x => x.CanEquip(itemClass));
    }

    public EquipSlotInfo EquipSlot(int index)
    {
        return EquipSlotInfo[index];
    }
}