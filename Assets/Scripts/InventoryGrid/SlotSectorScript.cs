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
        InvenGridManager.HighlightedSlot = ParentSlotScript;

        if (ItemScript.selectedItem != null)
            InvenGridManager.RefreshColor(true);
        else if (ParentSlotScript.storedItemObject != null)
            InvenGridManager.ColorChangeLoop(Color.white, ParentSlotScript.storedItemSize, ParentSlotScript.storedItemStartPos);
    }

    //Reset overlay and slot colour
    public void OnPointerExit(PointerEventData eventData)
    {
        SectorScript = null;
        InvenGridManager.Instance.HighlightedSlot = null;

        if (ItemScript.selectedItem != null)
            InvenGridManager.Instance.RefreshColor(false);
        else if (ParentSlotScript.storedItemObject != null)
            InvenGridManager.Instance.ColorChangeLoop(Color.white, ParentSlotScript.storedItemSize, ParentSlotScript.storedItemStartPos);
    }
}