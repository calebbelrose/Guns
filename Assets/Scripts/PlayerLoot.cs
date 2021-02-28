using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoot : Loot
{
    public CharacterInventory Inventory;

    public void Setup(CharacterInventory inventory, string name)
    {
        Inventory = inventory;
        Name = name;
        gameObject.layer = 11;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    public override void Action(AdvancedCamRecoil playerCam)
    {
        playerCam.InventoryCanvas.SetActive(true);
        playerCam.LootEquipment.gameObject.SetActive(true);
        playerCam.LootEquipment.Clear();
        playerCam.LootEquipment.Display(Inventory);
        playerCam.LootEquipment.DisplayPockets(Inventory.SlotGridList[1]);
        playerCam.PlayerMovement.enabled = false;
        Cursor.visible = playerCam.InventoryCanvas.activeSelf;
        Cursor.lockState = CursorLockMode.None;
    }

    public override bool Action(AIMovement aiMovement)
    {
        foreach (SlotGrid slotGrid in Inventory.SlotGridList[0].List)
            for (int x = 0; x < slotGrid.GridSize.x; x++)
            {
                for (int y = 0; y < slotGrid.GridSize.y; y++)
                {
                    if (!slotGrid.SlotInfo[x, y].Empty)
                        aiMovement.Inventory.StoreLoot(slotGrid.SlotInfo[x, y].Item);
                }
            }

        aiMovement.IgnoredObjects.Add(transform);
        return true;
    }
}