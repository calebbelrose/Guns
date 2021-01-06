using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PartSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public bool Empty = true;

    public Image Image { get { return image; } }

    public SlotInfo SlotInfo;
    public Inspect Inspect;

    [SerializeField] private CategoryName categoryName;
    [SerializeField] private Image image;
    [SerializeField] private GameObject PartSlotPrefab;

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
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Empty)
            {
                if (ItemScript.selectedItem != null && SlotInfo.PartIDs.Contains(ItemScript.selectedItem.item.GlobalID))
                {
                    Part part = ItemScript.selectedItem.item.Part;

                    if (ItemScript.selectedItem.Size.x > ItemScript.selectedItem.Size.y)
                    {
                        ItemScript.selectedItem.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SlotGrid.SlotSize);
                        ItemScript.selectedItem.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ItemScript.selectedItem.Size.y * SlotGrid.SlotSize / ItemScript.selectedItem.Size.x);
                    }
                    else
                    {
                        ItemScript.selectedItem.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ItemScript.selectedItem.Size.x * SlotGrid.SlotSize / ItemScript.selectedItem.Size.y);
                        ItemScript.selectedItem.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SlotGrid.SlotSize);
                    }

                    part.PartSlot = this;
                    SlotInfo.Item = ItemScript.selectedItem;
                    Inspect.Refresh();
                    Empty = false;
                    SlotInfo.Item.transform.SetParent(transform);
                    SlotInfo.Item.transform.localPosition = new Vector3(-ItemScript.selectedItem.Rect.sizeDelta.x / 2, ItemScript.selectedItem.Rect.sizeDelta.y / 2);
                    SlotInfo.Item.CanvasGroup.alpha = 1f;
                    Image.color = Color.white;

                    while (part.PartSlot != null)
                    {
                        foreach (PartSize modifier in ItemScript.selectedItem.item.Part.SizeModifiers)
                            part.PartSlot.SlotInfo.Item.item.Part.AddModifier(modifier);

                        part = part.PartSlot.SlotInfo.ParentPart;
                    }

                    CreateSlots(ItemScript.selectedItem);

                    ItemScript.ResetSelectedItem();
                }
            }
            else
            {
                ItemScript.SetSelectedItem(SlotInfo.Item);
                Image.color = Color.white;
                Empty = true;
            }
        }
    }

    private void CreateSlots(ItemScript itemScript)
    {
        if (itemScript != null && itemScript.item.Part != null)
        {
            int siblingIndex;

            if (itemScript.item.Part.PartSlot == null)
                siblingIndex = 0;
            else
                siblingIndex = itemScript.transform.parent.GetSiblingIndex();

            for (int i = 0; i < itemScript.item.Part.PartSlotCount(); i++)
            {
                PartSlot newPart = Instantiate(PartSlotPrefab, transform.parent).GetComponent<PartSlot>();

                newPart.Inspect = Inspect;
                newPart.SlotInfo = itemScript.item.Part.PartSlots(i);
                newPart.transform.SetSiblingIndex(siblingIndex + i);

                CreateSlots(newPart.SlotInfo.Item);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Image.color = Color.white;
    }
}