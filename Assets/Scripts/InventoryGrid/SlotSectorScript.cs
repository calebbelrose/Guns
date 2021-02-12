using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotSectorScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int QuadNum {  get { return quadNum; } }

    [SerializeField] private int quadNum;
    [SerializeField] private SlotScript ParentSlotScript;

    public static SlotSectorScript SectorScript { get; private set; }

    //Highlight slot, display overlay if it has an item and change slot colour
    public void OnPointerEnter(PointerEventData eventData)
    {
        SectorScript = this;
        InventoryManager.HighlightedSlot = ParentSlotScript;

        if (ItemScript.selectedItem != null)
            ParentSlotScript.InventorySlotInfo.SlotGrid.Inventory.RefreshColor(true);
        else if (ParentSlotScript.InventorySlotInfo.Item != null)
            Inventory.ColorChangeLoop(ParentSlotScript.InventorySlotInfo.SlotGrid, Color.white, ParentSlotScript.InventorySlotInfo.Item.Size, ParentSlotScript.InventorySlotInfo.ItemStartPos);
    }

    //Reset overlay and slot colour
    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemScript.selectedItem != null)
            ParentSlotScript.InventorySlotInfo.SlotGrid.Inventory.RefreshColor(false);
        else if (ParentSlotScript.InventorySlotInfo.Item != null)
            Inventory.ColorChangeLoop(ParentSlotScript.InventorySlotInfo.SlotGrid, Color.white, ParentSlotScript.InventorySlotInfo.Item.Size, ParentSlotScript.InventorySlotInfo.ItemStartPos);

        SectorScript = null;
        InventoryManager.HighlightedSlot = null;
    }
}