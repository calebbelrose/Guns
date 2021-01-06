using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour
{
    public IntVector2 GridPos { get; private set; }
    public ItemScript ItemObject { get; private set; }
    public IntVector2 ItemSize { get; private set; }
    public IntVector2 ItemStartPos { get; private set; }
    public ItemClass ItemClass { get; private set; }
    public bool IsOccupied { get; private set; }
    public SlotGrid SlotGrid { get; private set; }
    public Image Image;

    public void Setup(IntVector2 gridPos, ItemScript itemObject, IntVector2 itemSize, IntVector2 itemStartPos, ItemClass itemClass, bool isOccupied, SlotGrid slotGrid)
    {
        GridPos = gridPos;
        SlotGrid = slotGrid;
        ChangeItem(itemObject, itemSize, itemStartPos, itemClass, isOccupied);
    }

    public void ChangeItem(ItemScript itemObject, IntVector2 itemSize, IntVector2 itemStartPos, ItemClass itemClass, bool isOccupied)
    {
        ItemObject = itemObject;
        ItemSize = itemSize;
        ItemStartPos = itemStartPos;
        ItemClass = itemClass;
        IsOccupied = isOccupied;
    }
}
