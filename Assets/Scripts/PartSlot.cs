using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PartSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public bool Empty = true;
    public ItemScript Item;

    public Image Image { get { return image; } }

    public SlotInfo SlotInfo;

    [SerializeField] private CategoryName categoryName;
    [SerializeField] private Image image;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ItemScript.selectedItem != null)
        {
            if (Empty && SlotInfo.PartIDs.Contains(ItemScript.selectedItem.item.GlobalID))
                Image.color = Color.green;
            else
                Image.color = Color.red;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Empty)
        {
            if (ItemScript.selectedItem != null && SlotInfo.PartIDs.Contains(ItemScript.selectedItem.item.GlobalID))
            {
                Item = ItemScript.selectedItem;
                Empty = false;
                Image.color = Color.white;
                Item.transform.SetParent(transform);
                Item.Rect.pivot = new Vector2(0.0f, 1.0f);
                Item.transform.position = transform.position;
                Item.CanvasGroup.alpha = 1f;
                Image.color = Color.white;
                ItemScript.ResetSelectedItem();
            }
        }
        else
        {
            ItemScript.SetSelectedItem(Item);
            Image.color = Color.white;
            Empty = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Image.color = Color.white;
    }
}