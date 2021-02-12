using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootBox : Loot
{
    public Inventory Inventory { get { return inventory; } }

    [SerializeField] private Inventory inventory;
    [SerializeField] private List<IntVector2> GridSizes = new List<IntVector2>();

    //Sets up loot
    void Start()
    {
        foreach (IntVector2 GridSize in GridSizes)
            Inventory.SlotGridList[0].List.Add(new SlotGrid(GridSize.x, GridSize.y, Inventory));

        for (int i = 0; i < Random.Range(1, 5); i++)
        {
            ItemClass newItem = ItemDatabase.Instance.DBList(Random.Range(0, ItemDatabase.Instance.DBCount()));
            InventorySlotInfo slotInfo;

            slotInfo = Inventory.StoreLoot(newItem);
        }
    }

    public override void Action(AdvancedCamRecoil playerCam)
    {
        foreach (Transform child in playerCam.LootBoxRect)
            Destroy(child.gameObject);

        playerCam.InventoryCanvas.SetActive(true);
        playerCam.LootBoxInventory.SetActive(true);
        Cursor.visible = playerCam.InventoryCanvas.activeSelf;
        playerCam.PlayerMovement.enabled = false;
        Inventory.SlotGridList[0].List[0].Display(InventoryManager.SlotPrefab, playerCam.LootBoxRect);
    }

    public override bool Action(AIMovement aiMovement)
    {
        foreach (SlotGrid slotGrid in inventory.SlotGridList[0].List)
            for(int x = 0; x < slotGrid.GridSize.x; x++)
            {
                for (int y = 0; y < slotGrid.GridSize.y; y++)
                {
                    if(!slotGrid.SlotInfo[x, y].Empty)
                        aiMovement.Inventory.StoreLoot(slotGrid.SlotInfo[x, y].Item);
                }
            }

        aiMovement.IgnoredObjects.Add(transform);
        return true;
    }
}