using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundLoot : Loot
{
    public int ItemID { get { return itemID; } }

    [SerializeField] private int itemID;

    //Sets up loot
    void Start()
    {
        LootTextObject = ItemDatabase.Instance.CreateLootText();
        LootTextObject.transform.GetChild(0).GetComponent<Text>().text = ItemDatabase.Instance.DBList(itemID).TypeName;
        LootTextObject.SetActive(false);
    }

    public override void Action(AdvancedCamRecoil playerCam)
    {
        ItemScript newItem = GameObject.Instantiate(ItemDatabase.Instance.ItemPrefab).GetComponent<ItemScript>();
        InventorySlotInfo slotInfo;
        EquipSlotInfo equipSlotInfo;

        newItem.SetItemObject(ItemDatabase.Instance.DBList(itemID));
        equipSlotInfo = playerCam.CombatController.FindSlot(newItem);

        if (equipSlotInfo.ItemScript == null)
        {
            ItemScript.SetSelectedItem(newItem);
            playerCam.CombatController.EquipInSlot(newItem, equipSlotInfo);
            GroundLoot.Destroy();
        }
        else
        {
            slotInfo = playerCam.Inventory.StoreLoot(newItem.Item);

            if (slotInfo != null)
            {
                playerCam.Inventory.PlaceItem(slotInfo, newItem);
                GroundLoot.Destroy();
            }
            else
                Destroy(newItem.gameObject);
        }
    }
}