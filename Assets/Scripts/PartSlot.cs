using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PartSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public bool Empty = true;

    public PartSlotInfo SlotInfo;
    public Inspect Inspect;

    [SerializeField] private CategoryName categoryName;
    [SerializeField] private Image Image;
    [SerializeField] private Image IconImage;
    [SerializeField] private RectTransform IconRect;

    private List<Image> changedImages = new List<Image>();
    private List<GameObject> CreatedSlots = new List<GameObject>();

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ItemScript.selectedItem != null)
        {
            if (Empty && SlotInfo.PartIDs.Contains(ItemScript.selectedItem.Item.GlobalID))
            {
                ItemScript itemScript = SlotInfo.ParentItem;
                int[] tempModifiers = new int[4];
                bool fits = true;
                int maxX = SlotInfo.ParentItem.Slot.InventorySlotInfo.GridPos.x + SlotInfo.ParentItem.Item.Size.x, maxY = SlotInfo.ParentItem.Slot.InventorySlotInfo.GridPos.y + SlotInfo.ParentItem.Item.Size.y;

                while (itemScript.Item.Part.PartSlot != null)
                    itemScript = itemScript.Item.Part.PartSlot.SlotInfo.ParentItem;

                for (int i = 0; i < tempModifiers.Length; i++)
                {
                    if (itemScript.Item.Part.SizeModifiers[i].HighestModifier < ItemScript.selectedItem.Item.Part.SizeModifiers[i].HighestModifier)
                        tempModifiers[i] = ItemScript.selectedItem.Item.Part.SizeModifiers[i].HighestModifier;
                    else
                        tempModifiers[i] = itemScript.Item.Part.SizeModifiers[i].HighestModifier;
                }

                if (tempModifiers[ItemScript.selectedItem.Item.Part.Size.x] < ItemScript.selectedItem.Item.Part.Size.y)
                    tempModifiers[ItemScript.selectedItem.Item.Part.Size.x] = ItemScript.selectedItem.Item.Part.Size.y;

                if (CheckArea(new IntVector2(itemScript.Item.Size.x + tempModifiers[1] + tempModifiers[3], itemScript.Item.Size.y + tempModifiers[0] + tempModifiers[2]), SlotInfo.ParentItem.Slot))
                {
                    fits = false;
                }
                else
                {
                    for (int x = itemScript.Slot.InventorySlotInfo.GridPos.x; x < maxX; x++)
                    {
                        for (int y = maxY; y < maxY + tempModifiers[0] + tempModifiers[2]; y++)
                        {
                            if (itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].ItemScript != null && itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].ItemScript != itemScript)
                            {
                                fits = false;
                                itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image.color = Color.red;
                            }
                            else
                                itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image.color = Color.green;

                            changedImages.Add(itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image);
                        }
                    }

                    for (int x = maxX; x < maxX + tempModifiers[1] + tempModifiers[3]; x++)
                    {
                        for (int y = itemScript.Slot.InventorySlotInfo.GridPos.y; y < maxY + tempModifiers[0] + tempModifiers[2]; y++)
                        {
                            if (itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].ItemScript != null)
                            {
                                fits = false;
                                itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image.color = Color.red;
                            }
                            else
                                itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image.color = Color.green;

                            changedImages.Add(itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image);
                        }
                    }
                }

                if (fits)
                    Image.color = Color.green;
                else
                    Image.color = Color.red;
            }
            else
                Image.color = Color.red;
        }
    }

    private bool CheckArea(IntVector2 itemSize, SlotScript slotScript)
    {
        IntVector2 overCheck = slotScript.InventorySlotInfo.GridPos + itemSize;

        if (overCheck.x > slotScript.InventorySlotInfo.SlotGrid.GridSize.x || slotScript.InventorySlotInfo.GridPos.x < 0 || overCheck.y > slotScript.InventorySlotInfo.SlotGrid.GridSize.y || slotScript.InventorySlotInfo.GridPos.y < 0)
            return true;

        return false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Empty)
            {
                if (ItemScript.selectedItem != null && SlotInfo.PartIDs.Contains(ItemScript.selectedItem.Item.GlobalID))
                {
                    FillSlot(ItemScript.selectedItem);
                    CreateSlots(ItemScript.selectedItem);
                    ItemScript.ResetSelectedItem();
                }
            }
            else
            {
                ItemScript itemScript = SlotInfo.ItemScript;

                while (itemScript.Item.Part.PartSlot != null)
                {
                    int[] changedModifiers = new int[4];
                    int modifier;

                    itemScript = itemScript.Item.Part.PartSlot.SlotInfo.ParentItem;

                    for (int i = 0; i < 4; i++)
                        changedModifiers[i] = SlotInfo.ItemScript.Item.Part.SizeModifiers[i].RemoveFromPart(itemScript.Item.Part, i);

                    modifier = itemScript.Item.Part.SizeModifiers[SlotInfo.ItemScript.Item.Part.Size.x].Remove(SlotInfo.ItemScript.Item.Part.Size.y);

                    if (changedModifiers[SlotInfo.ItemScript.Item.Part.Size.x] < modifier)
                        changedModifiers[SlotInfo.ItemScript.Item.Part.Size.x] = modifier;

                    for (int x = itemScript.Slot.InventorySlotInfo.GridPos.x; x < itemScript.Size.x; x++)
                    {
                        for (int y = itemScript.Size.y; y < itemScript.Size.y + changedModifiers[0] + changedModifiers[2]; y++)
                        {
                            itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image.color = Color.white;
                            itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].ChangeItem(null, IntVector2.Zero);
                        }
                    }

                    for (int x = itemScript.Size.x; x < itemScript.Size.x + changedModifiers[1] + changedModifiers[3]; x++)
                    {
                        for (int y = itemScript.Slot.InventorySlotInfo.GridPos.y; y < itemScript.Size.y + changedModifiers[0] + changedModifiers[2]; y++)
                        {
                            itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image.color = Color.white;
                            itemScript.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].ChangeItem(null, IntVector2.Zero);
                        }
                    }
                }

                foreach (GameObject gameObject in CreatedSlots)
                    Destroy(gameObject);

                SlotInfo.ItemScript.Item.Part.PartSlot = null;
                IconImage.sprite = null;
                IconImage.enabled = false;
                SlotInfo.ItemScript.Image.enabled = true;
                ItemScript.SetSelectedItem(SlotInfo.ItemScript);
                Image.color = Color.white;
                Empty = true;
                itemScript.Rect.sizeDelta = new Vector2(SlotGrid.SlotSize * itemScript.Size.x, SlotGrid.SlotSize * itemScript.Size.y);
            }
        }
    }

    public void FillSlot(ItemScript itemScript)
    {
        if (ItemScript.selectedItem.Size.x > ItemScript.selectedItem.Size.y)
            IconRect.sizeDelta = new Vector2(SlotGrid.SlotSize, ItemScript.selectedItem.Size.y * SlotGrid.SlotSize / ItemScript.selectedItem.Size.x);
        else
            IconRect.sizeDelta = new Vector2( ItemScript.selectedItem.Size.x * SlotGrid.SlotSize / ItemScript.selectedItem.Size.y, SlotGrid.SlotSize);

        itemScript.Item.Part.PartSlot = this;
        SlotInfo.ItemScript = ItemScript.selectedItem;
        Inspect.Refresh();
        Empty = false;
        SlotInfo.ItemScript.transform.SetParent(InventoryManager.DropParent);
        IconImage.enabled = true;
        IconImage.sprite = ItemScript.selectedItem.Image.sprite;
        ItemScript.selectedItem.Image.enabled = false;
        Image.color = Color.white;

        while (itemScript.Item.Part.PartSlot != null)
        {
            itemScript = itemScript.Item.Part.PartSlot.SlotInfo.ParentItem;

            for (int i = 0; i < 4; i++)
                ItemScript.selectedItem.Item.Part.SizeModifiers[i].AddToPart(itemScript.Item.Part, i);

            itemScript.Item.Part.SizeModifiers[ItemScript.selectedItem.Item.Part.Size.x].Add(ItemScript.selectedItem.Item.Part.Size.y);
        }

        itemScript.Rect.sizeDelta = new Vector2(SlotGrid.SlotSize * itemScript.Size.x, SlotGrid.SlotSize * itemScript.Size.y);
    }

    private void CreateSlots(ItemScript itemScript)
    {
        if (itemScript != null && itemScript.Item.Part != null)
        {
            int siblingIndex;

            if (itemScript.Item.Part.PartSlot == null)
                siblingIndex = 0;
            else
                siblingIndex = itemScript.transform.parent.GetSiblingIndex();

            for (int i = 0; i < itemScript.Item.Part.PartSlotCount(); i++)
            {
                PartSlot newPart = Instantiate(Inspect.PartSlotPrefab, transform.parent).GetComponent<PartSlot>();

                CreatedSlots.Add(newPart.gameObject);
                newPart.Inspect = Inspect;
                newPart.SlotInfo = itemScript.Item.Part.PartSlots(i);
                newPart.SlotInfo.ParentItem = itemScript;
                newPart.transform.SetSiblingIndex(siblingIndex + i);

                if (newPart.SlotInfo.ItemScript != null)
                {
                    Debug.Log(newPart.SlotInfo.ItemScript.Image.sprite);
                    newPart.IconImage.enabled = true;
                    newPart.IconImage.sprite = newPart.SlotInfo.ItemScript.Image.sprite;

                    if (newPart.SlotInfo.ItemScript.Size.x > newPart.SlotInfo.ItemScript.Size.y)
                        newPart.IconRect.sizeDelta = new Vector2(SlotGrid.SlotSize, newPart.SlotInfo.ItemScript.Size.y * SlotGrid.SlotSize / newPart.SlotInfo.ItemScript.Size.x);
                    else
                        newPart.IconRect.sizeDelta = new Vector2(newPart.SlotInfo.ItemScript.Size.x * SlotGrid.SlotSize / newPart.SlotInfo.ItemScript.Size.y, SlotGrid.SlotSize);
                }

                CreateSlots(newPart.SlotInfo.ItemScript);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Image.color = Color.white;

        while (changedImages.Count != 0)
        {
            changedImages[0].color = Color.white;
            changedImages.RemoveAt(0);
        }
    }
}