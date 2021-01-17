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
        ParentSlotScript.SlotGrid.Inventory.HighlightedSlot = ParentSlotScript;

        if (ItemScript.selectedItem != null)
            ParentSlotScript.SlotGrid.Inventory.RefreshColor(true);
        else if (ParentSlotScript.ItemScript != null)
            Inventory.ColorChangeLoop(ParentSlotScript.SlotGrid, Color.white, ParentSlotScript.ItemScript.Size, ParentSlotScript.ItemStartPos);
    }

    //Reset overlay and slot colour
    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemScript.selectedItem != null)
            ParentSlotScript.SlotGrid.Inventory.RefreshColor(false);
        else if (ParentSlotScript.ItemScript != null)
            Inventory.ColorChangeLoop(ParentSlotScript.SlotGrid, Color.white, ParentSlotScript.ItemScript.Size, ParentSlotScript.ItemStartPos);

        SectorScript = null;
        ParentSlotScript.SlotGrid.Inventory.HighlightedSlot = null;
    }
}