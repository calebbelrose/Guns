using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PartSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public bool Empty = true;

    public SlotInfo SlotInfo;
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
                int maxX = SlotInfo.ParentItem.Slot.GridPos.x + SlotInfo.ParentItem.Item.Size.x, maxY = SlotInfo.ParentItem.Slot.GridPos.y + SlotInfo.ParentItem.Item.Size.y;

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
                    for (int x = itemScript.Slot.GridPos.x; x < maxX; x++)
                    {
                        for (int y = maxY; y < maxY + tempModifiers[0] + tempModifiers[2]; y++)
                        {
                            if (itemScript.Slot.SlotGrid.Slots[x, y].IsOccupied && itemScript.Slot.SlotGrid.Slots[x, y].ItemScript != itemScript)
                            {
                                fits = false;
                                itemScript.Slot.SlotGrid.Slots[x, y].Image.color = Color.red;
                            }
                            else
                                itemScript.Slot.SlotGrid.Slots[x, y].Image.color = Color.green;

                            changedImages.Add(itemScript.Slot.SlotGrid.Slots[x, y].Image);
                        }
                    }

                    for (int x = maxX; x < maxX + tempModifiers[1] + tempModifiers[3]; x++)
                    {
                        for (int y = itemScript.Slot.GridPos.y; y < maxY + tempModifiers[0] + tempModifiers[2]; y++)
                        {
                            if (itemScript.Slot.SlotGrid.Slots[x, y].IsOccupied)
                            {
                                fits = false;
                                itemScript.Slot.SlotGrid.Slots[x, y].Image.color = Color.red;
                            }
                            else
                                itemScript.Slot.SlotGrid.Slots[x, y].Image.color = Color.green;

                            changedImages.Add(itemScript.Slot.SlotGrid.Slots[x, y].Image);
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
        IntVector2 overCheck = slotScript.GridPos + itemSize;

        if (overCheck.x > slotScript.SlotGrid.GridSize.x || slotScript.GridPos.x < 0 || overCheck.y > slotScript.SlotGrid.GridSize.y || slotScript.GridPos.y < 0)
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
                ItemScript itemScript = SlotInfo.Item;

                while (itemScript.Item.Part.PartSlot != null)
                {
                    int[] changedModifiers = new int[4];
                    int modifier;

                    itemScript = itemScript.Item.Part.PartSlot.SlotInfo.ParentItem;

                    for (int i = 0; i < 4; i++)
                        changedModifiers[i] = SlotInfo.Item.Item.Part.SizeModifiers[i].RemoveFromPart(itemScript.Item.Part, i);

                    modifier = itemScript.Item.Part.SizeModifiers[SlotInfo.Item.Item.Part.Size.x].Remove(SlotInfo.Item.Item.Part.Size.y);

                    if (changedModifiers[SlotInfo.Item.Item.Part.Size.x] < modifier)
                        changedModifiers[SlotInfo.Item.Item.Part.Size.x] = modifier;

                    for (int x = itemScript.Slot.GridPos.x; x < itemScript.Size.x; x++)
                    {
                        for (int y = itemScript.Size.y; y < itemScript.Size.y + changedModifiers[0] + changedModifiers[2]; y++)
                        {
                            itemScript.Slot.SlotGrid.Slots[x, y].Image.color = Color.white;
                            itemScript.Slot.SlotGrid.Slots[x, y].ChangeItem(null, IntVector2.Zero, false);
                        }
                    }

                    for (int x = itemScript.Size.x; x < itemScript.Size.x + changedModifiers[1] + changedModifiers[3]; x++)
                    {
                        for (int y = itemScript.Slot.GridPos.y; y < itemScript.Size.y + changedModifiers[0] + changedModifiers[2]; y++)
                        {
                            itemScript.Slot.SlotGrid.Slots[x, y].Image.color = Color.white;
                            itemScript.Slot.SlotGrid.Slots[x, y].ChangeItem(null, IntVector2.Zero, false);
                        }
                    }
                }

                foreach (GameObject gameObject in CreatedSlots)
                    Destroy(gameObject);

                SlotInfo.Item.Item.Part.PartSlot = null;
                IconImage.sprite = null;
                IconImage.enabled = false;
                SlotInfo.Item.Image.enabled = true;
                ItemScript.SetSelectedItem(SlotInfo.Item);
                Image.color = Color.white;
                Empty = true;
                itemScript.Rect.sizeDelta = new Vector2(DisplaySlots.SlotSize * itemScript.Size.x, DisplaySlots.SlotSize * itemScript.Size.y);
            }
        }
    }

    public void FillSlot(ItemScript itemScript)
    {
        if (ItemScript.selectedItem.Size.x > ItemScript.selectedItem.Size.y)
            IconRect.sizeDelta = new Vector2(DisplaySlots.SlotSize, ItemScript.selectedItem.Size.y * DisplaySlots.SlotSize / ItemScript.selectedItem.Size.x);
        else
            IconRect.sizeDelta = new Vector2( ItemScript.selectedItem.Size.x * DisplaySlots.SlotSize / ItemScript.selectedItem.Size.y, DisplaySlots.SlotSize);

        itemScript.Item.Part.PartSlot = this;
        SlotInfo.Item = ItemScript.selectedItem;
        Inspect.Refresh();
        Empty = false;
        SlotInfo.Item.transform.SetParent(InventoryManager.DropParent);
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

        itemScript.Rect.sizeDelta = new Vector2(DisplaySlots.SlotSize * itemScript.Size.x, DisplaySlots.SlotSize * itemScript.Size.y);
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

                if (newPart.SlotInfo.Item != null)
                {
                    Debug.Log(newPart.SlotInfo.Item.Image.sprite);
                    newPart.IconImage.enabled = true;
                    newPart.IconImage.sprite = newPart.SlotInfo.Item.Image.sprite;

                    if (newPart.SlotInfo.Item.Size.x > newPart.SlotInfo.Item.Size.y)
                        newPart.IconRect.sizeDelta = new Vector2(DisplaySlots.SlotSize, newPart.SlotInfo.Item.Size.y * DisplaySlots.SlotSize / newPart.SlotInfo.Item.Size.x);
                    else
                        newPart.IconRect.sizeDelta = new Vector2(newPart.SlotInfo.Item.Size.x * DisplaySlots.SlotSize / newPart.SlotInfo.Item.Size.y, DisplaySlots.SlotSize);
                }

                CreateSlots(newPart.SlotInfo.Item);
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