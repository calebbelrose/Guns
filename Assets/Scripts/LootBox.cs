using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootBox : Loot
{
    public Inventory Inventory { get { return inventory; } }

    [SerializeField] private string Name;
    [SerializeField] private Inventory inventory;
    [SerializeField] private IntVector2 GridSize;
    [SerializeField] GameObject SlotPrefab;

    //Sets up loot
    void Start()
    {
        Inventory.slotGridList[0].List.Add(new SlotGrid(GridSize.x, GridSize.y, Inventory));

        for (int i = 0; i < Random.Range(1, 5); i++)
        {
            ItemScript newItem = ItemDatabase.Instance.ItemEquipPool.GetItemScript();
            InventorySlotInfo slotInfo;

            newItem.SetItemObject(ItemDatabase.Instance.DBList(1/*Random.Range(0, ItemDatabase.Instance.DBCount())*/));
            slotInfo = Inventory.StoreLoot(newItem);

            if (slotInfo == null)
                Destroy(newItem.gameObject);
        }

        LootTextObject = ItemDatabase.Instance.CreateLootText();
        LootTextObject.transform.GetChild(0).GetComponent<Text>().text = Name;
        LootTextObject.SetActive(false);
    }

    public override void Action(AdvancedCamRecoil playerCam)
    {
        foreach (Transform child in playerCam.LootBoxRect)
            Destroy(child.gameObject);

        playerCam.InventoryCanvas.SetActive(true);
        playerCam.LootBoxInventory.SetActive(true);
        Cursor.visible = playerCam.InventoryCanvas.activeSelf;
        playerCam.PlayerMovement.enabled = false;
        Inventory.slotGridList[0].List[0].Display(SlotPrefab, playerCam.LootBoxRect);
    }
}