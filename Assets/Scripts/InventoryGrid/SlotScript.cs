using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour
{
    public InventorySlotInfo InventorySlotInfo;
    public ItemScript ItemScript;
    public RectTransform Rect { get { return rect; } }
    public Image Image { get { return image; } }

    [SerializeField] private Image image;
    [SerializeField] private RectTransform rect;
}
