using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour
{
    public IntVector2 GridPos { get; private set; }
    public ItemScript ItemScript { get; private set; }
    public IntVector2 ItemStartPos { get; private set; }
    public bool IsOccupied { get; private set; }
    public SlotGrid SlotGrid { get; private set; }
    public Rect Rect { get { return rect; } }
    public Image Image { get { return image; } }

    [SerializeField] private Image image;
    [SerializeField] private Rect rect;

    public void Setup(IntVector2 gridPos, ItemScript itemObject, IntVector2 itemStartPos, bool isOccupied, SlotGrid slotGrid)
    {
        GridPos = gridPos;
        SlotGrid = slotGrid;
        ChangeItem(itemObject, itemStartPos, isOccupied);
    }

    public void ChangeItem(ItemScript itemScript, IntVector2 itemStartPos, bool isOccupied)
    {
        ItemScript = itemScript;
        ItemStartPos = itemStartPos;
        IsOccupied = isOccupied;
    }
}
