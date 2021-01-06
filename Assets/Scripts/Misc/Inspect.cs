using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inspect : MonoBehaviour {

    [SerializeField] private Text nameText;
    [SerializeField] private Image Icon;
    [SerializeField] private RectTransform IconRect, SlotRect;
    [SerializeField] private GameObject PartSlotPrefab;
    [SerializeField] private ContentSizeFitter ContentSizeFitter;

    //Updates overlay information
    public void Setup(ItemScript itemScript)
    {
        int totalSize = itemScript.Size.x * itemScript.Size.y;

        CreateSlots(itemScript.item.Part);
        nameText.text = itemScript.item.TypeName;
        Icon.color = new Color32(255, 255, 255, 255);
        Icon.sprite = itemScript.item.Icon;
        IconRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemScript.Size.x * SlotGrid.SlotSize);
        IconRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemScript.Size.y * SlotGrid.SlotSize);
    }

    public void Refresh()
    {
        ContentSizeFitter.enabled = false;
        ContentSizeFitter.enabled = true;
    }

    private void CreateSlots(Part part)
    {
        if (part != null)
        {
            for (int i = 0; i < part.PartSlotCount(); i++)
            {
                PartSlot partSlot = Instantiate(PartSlotPrefab, SlotRect).GetComponent<PartSlot>();

                partSlot.SlotInfo = part.PartSlots(i);
                partSlot.Inspect = this;

                if (part.PartSlots(i).Item != null)
                    CreateSlots(part.PartSlots(i).Item.item.Part);
            }
        }
    }
}