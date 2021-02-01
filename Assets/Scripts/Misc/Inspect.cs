using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inspect : MonoBehaviour {

    public GameObject PartSlotPrefab { get { return partSlotPrefab; } }

    [SerializeField] private Text nameText;
    [SerializeField] private Image Icon;
    [SerializeField] private RectTransform IconRect, SlotRect;
    [SerializeField] private GameObject partSlotPrefab;
    [SerializeField] private ContentSizeFitter ContentSizeFitter;

    //Updates overlay information
    public void Setup(ItemClass item)
    {
        int totalSize = item.Size.x * item.Size.y;

        CreateSlots(item);
        nameText.text = item.TypeName;
        Icon.color = new Color32(255, 255, 255, 255);
        Icon.sprite = item.Icon;
        IconRect.sizeDelta = new Vector2(item.Size.x * SlotGrid.SlotSize, item.Size.y * SlotGrid.SlotSize);
    }

    public void Refresh()
    {
        ContentSizeFitter.enabled = false;
        ContentSizeFitter.enabled = true;
    }

    private void CreateSlots(ItemClass item)
    {
        Part part = item.Part;

        if (part != null)
        {
            for (int i = 0; i < part.PartSlotCount(); i++)
            {
                PartSlot partSlot = Instantiate(PartSlotPrefab, SlotRect).GetComponent<PartSlot>();

                partSlot.SlotInfo = part.PartSlots(i);
                partSlot.SlotInfo.ParentItem = item;
                partSlot.Inspect = this;

                if (part.PartSlots(i).Item != null)
                {
                    partSlot.FillSlot(part.PartSlots(i).Item);
                    CreateSlots(part.PartSlots(i).Item);
                }
            }
        }
    }
}