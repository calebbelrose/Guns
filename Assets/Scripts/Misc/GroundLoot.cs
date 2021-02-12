using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundLoot : Loot
{
    public int ItemID { get { return itemID; } }

    [SerializeField] private int itemID;
    [SerializeField] private int InventoryIndex;

    //Sets up loot
    void Start()
    {
        Name = ItemDatabase.Instance.DBList(itemID).TypeName;
    }

    public override void Action(AdvancedCamRecoil playerCam)
    {
        ItemClass itemClass = ItemDatabase.Instance.DBList(itemID);
        EquipSlotInfo equipSlotInfo;

        equipSlotInfo = playerCam.Inventory.FindSlot(itemClass);

        if (equipSlotInfo != null)
        {
            Inventory inventory = GetComponent<Inventory>();

            if (inventory != null)
            {
                itemClass.SlotGridList = inventory.SlotGridList[0];
                playerCam.Inventory.SlotGridList[InventoryIndex] = itemClass.SlotGridList;
            }

            playerCam.Inventory.EquipInSlot(itemClass, equipSlotInfo);
            Destroy(gameObject);
        }
        else
        {
            InventorySlotInfo slotInfo = playerCam.Inventory.StoreLoot(itemClass);

            if (slotInfo != null)
            {
                ItemScript newItem = ItemDatabase.CreateItemScript(itemClass, InventoryManager.DropParent);
                Inventory inventory = GetComponent<Inventory>();

                if (inventory != null)
                    newItem.gameObject.AddComponent<Inventory>().SlotGridList = inventory.SlotGridList;

                playerCam.Inventory.PlaceItem(slotInfo, newItem);
                Destroy(gameObject);
            }
        }
    }

    public override bool Action(AIMovement aiMovement)
    {
        ItemClass itemClass = ItemDatabase.Instance.DBList(itemID);
        EquipSlotInfo equipSlotInfo;

        equipSlotInfo = aiMovement.Inventory.FindSlot(itemClass);

        if (equipSlotInfo != null)
        {
            Debug.Log("Equip");
            Inventory inventory = GetComponent<Inventory>();

            if (inventory != null)
            {
                itemClass.SlotGridList = inventory.SlotGridList[0];
                aiMovement.Inventory.SlotGridList[InventoryIndex] = itemClass.SlotGridList;
            }

            aiMovement.Inventory.EquipInSlot(itemClass, equipSlotInfo);
            Destroy(gameObject);
        }
        {
            Debug.Log("Loot");
            InventorySlotInfo slotInfo = aiMovement.Inventory.StoreLoot(itemClass);

            if (slotInfo != null)
            {
                Destroy(gameObject);
                return true;
            }
            else
            {
                aiMovement.IgnoredObjects.Add(transform);
                return false;
            }
        }
    }
}