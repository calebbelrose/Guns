using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inspect : MonoBehaviour {

    [SerializeField] private Text nameText;
    [SerializeField] private Image Icon;
    [SerializeField] private RectTransform IconRect, SlotRect;
    [SerializeField] private GameObject PartSlotPrefab;

    //Updates overlay information
    public void Setup(ItemClass item)
    {
        int totalSize = item.Size.x * item.Size.y;

        if (item.ScriptableObject != null)
        {
            GunPart part = (GunPart)item.ScriptableObject;
            for (int i = 0; i < part.PartSlotCount(); i++)
                Instantiate(PartSlotPrefab, SlotRect).GetComponent<PartSlot>().SlotInfo = part.PartSlots(i);
        }

        nameText.text = item.TypeName;
        Icon.color = new Color32(255, 255, 255, 255);
        Icon.sprite = item.Icon;
        IconRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, item.Size.x * InvenGridScript.SlotSize);
        IconRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, item.Size.y * InvenGridScript.SlotSize);
    }
}
