using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySlots : MonoBehaviour
{
    [SerializeField] private IntVector2 GridSize;
    [SerializeField] Inventory Inventory;
    [SerializeField] GameObject SlotPrefab;

    private SlotGrid slotGrid;

    // Start is called before the first frame update
    void Start()
    {
        Inventory.slotGridList[0].List.Add(new SlotGrid(GridSize.x, GridSize.y, Inventory));

        for (int i = 0; i < Random.Range(0, 5); i++)
        {
            ItemScript newItem = ItemDatabase.Instance.ItemEquipPool.GetItemScript();
            newItem.SetItemObject(ItemDatabase.Instance.DBList(Random.Range(0, ItemDatabase.Instance.DBCount())));
            Inventory.StoreLoot(newItem);
        }

        Inventory.slotGridList[0].List[0].Display(SlotPrefab, transform);
    }
}
