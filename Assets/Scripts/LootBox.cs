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
        Inventory.SlotGridList[0].List.Add(new SlotGrid(GridSize.x, GridSize.y, Inventory));

        for (int i = 0; i < Random.Range(1, 5); i++)
        {
            ItemClass newItem = ItemDatabase.Instance.DBList(1/*Random.Range(0, ItemDatabase.Instance.DBCount())*/);
            InventorySlotInfo slotInfo;

            slotInfo = Inventory.StoreLoot(newItem);

            if (slotInfo != null)
            {
                ItemScript itemScript = GameObject.Instantiate(ItemDatabase.Instance.ItemPrefab).GetComponent<ItemScript>();
                itemScript.SetItemObject(newItem);
            }
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
        Inventory.SlotGridList[0].List[0].Display(SlotPrefab, playerCam.LootBoxRect);
    }
}