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
    public void Setup(ItemScript itemScript)
    {
        int totalSize = itemScript.Size.x * itemScript.Size.y;

        CreateSlots(itemScript);
        nameText.text = itemScript.Item.TypeName;
        Icon.color = new Color32(255, 255, 255, 255);
        Icon.sprite = itemScript.Item.Icon;
        IconRect.sizeDelta = new Vector2(itemScript.Size.x * SlotGrid.SlotSize, itemScript.Size.y * SlotGrid.SlotSize);
    }

    public void Refresh()
    {
        ContentSizeFitter.enabled = false;
        ContentSizeFitter.enabled = true;
    }

    private void CreateSlots(ItemScript itemScript)
    {
        Part part = itemScript.Item.Part;

        if (part != null)
        {
            for (int i = 0; i < part.PartSlotCount(); i++)
            {
                PartSlot partSlot = Instantiate(PartSlotPrefab, SlotRect).GetComponent<PartSlot>();

                partSlot.SlotInfo = part.PartSlots(i);
                partSlot.SlotInfo.ParentItem = itemScript;
                partSlot.Inspect = this;

                if (part.PartSlots(i).ItemScript != null)
                {
                    partSlot.FillSlot(part.PartSlots(i).ItemScript);
                    CreateSlots(part.PartSlots(i).ItemScript);
                }
            }
        }
    }
}