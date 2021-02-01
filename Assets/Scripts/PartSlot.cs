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
                ItemClass item = SlotInfo.Item;
                int[] tempModifiers = new int[4];
                bool fits = true;
                int maxX = SlotInfo.ParentItem.Slot.InventorySlotInfo.GridPos.x + SlotInfo.ParentItem.BaseSize.x, maxY = SlotInfo.ParentItem.Slot.InventorySlotInfo.GridPos.y + SlotInfo.ParentItem.BaseSize.y;

                while (item.Part.PartSlot != null)
                    item = item.Part.PartSlot.SlotInfo.ParentItem;

                for (int i = 0; i < tempModifiers.Length; i++)
                {
                    if (item.Part.SizeModifiers[i].HighestModifier < ItemScript.selectedItem.Item.Part.SizeModifiers[i].HighestModifier)
                        tempModifiers[i] = ItemScript.selectedItem.Item.Part.SizeModifiers[i].HighestModifier;
                    else
                        tempModifiers[i] = item.Part.SizeModifiers[i].HighestModifier;
                }

                if (tempModifiers[ItemScript.selectedItem.Item.Part.Size.x] < ItemScript.selectedItem.Item.Part.Size.y)
                    tempModifiers[ItemScript.selectedItem.Item.Part.Size.x] = ItemScript.selectedItem.Item.Part.Size.y;

                if (CheckArea(new IntVector2(item.BaseSize.x + tempModifiers[1] + tempModifiers[3], item.BaseSize.y + tempModifiers[0] + tempModifiers[2]), SlotInfo.ParentItem.Slot))
                {
                    fits = false;
                }
                else
                {
                    for (int x = item.Slot.InventorySlotInfo.GridPos.x; x < maxX; x++)
                    {
                        for (int y = maxY; y < maxY + tempModifiers[0] + tempModifiers[2]; y++)
                        {
                            if (item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].Item != null && item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].Item != item)
                            {
                                fits = false;
                                item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image.color = Color.red;
                            }
                            else
                                item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image.color = Color.green;

                            changedImages.Add(item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image);
                        }
                    }

                    for (int x = maxX; x < maxX + tempModifiers[1] + tempModifiers[3]; x++)
                    {
                        for (int y = item.Slot.InventorySlotInfo.GridPos.y; y < maxY + tempModifiers[0] + tempModifiers[2]; y++)
                        {
                            if (item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].Item != null)
                            {
                                fits = false;
                                item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image.color = Color.red;
                            }
                            else
                                item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image.color = Color.green;

                            changedImages.Add(item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image);
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
                    FillSlot(ItemScript.selectedItem.Item);
                    CreateSlots(ItemScript.selectedItem.Item);
                    ItemScript.ResetSelectedItem();
                }
            }
            else
            {
                ItemClass item = SlotInfo.Item;

                while (item.Part.PartSlot != null)
                {
                    int[] changedModifiers = new int[4];
                    int modifier;

                    item = item.Part.PartSlot.SlotInfo.ParentItem;

                    for (int i = 0; i < 4; i++)
                        changedModifiers[i] = SlotInfo.Item.Part.SizeModifiers[i].RemoveFromPart(item.Part, i);

                    modifier = item.Part.SizeModifiers[SlotInfo.Item.Part.Size.x].Remove(SlotInfo.Item.Part.Size.y);

                    if (changedModifiers[SlotInfo.Item.Part.Size.x] < modifier)
                        changedModifiers[SlotInfo.Item.Part.Size.x] = modifier;

                    for (int x = item.Slot.InventorySlotInfo.GridPos.x; x < item.Size.x; x++)
                    {
                        for (int y = item.Size.y; y < item.Size.y + changedModifiers[0] + changedModifiers[2]; y++)
                        {
                            item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image.color = Color.white;
                            item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].ChangeItem(null, IntVector2.Zero);
                            item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.ItemScript = null;
                        }
                    }

                    for (int x = item.Size.x; x < item.Size.x + changedModifiers[1] + changedModifiers[3]; x++)
                    {
                        for (int y = item.Slot.InventorySlotInfo.GridPos.y; y < item.Size.y + changedModifiers[0] + changedModifiers[2]; y++)
                        {
                            item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.Image.color = Color.white;
                            item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].ChangeItem(null, IntVector2.Zero);
                            item.Slot.InventorySlotInfo.SlotGrid.SlotInfo[x, y].SlotScript.ItemScript = null;
                        }
                    }
                }

                foreach (GameObject gameObject in CreatedSlots)
                    Destroy(gameObject);

                SlotInfo.Item.Part.PartSlot = null;
                IconImage.sprite = null;
                IconImage.enabled = false;
                SlotInfo.Slot.ItemScript.Image.enabled = true;
                ItemScript.SetSelectedItem(SlotInfo.Slot.ItemScript);
                Image.color = Color.white;
                Empty = true;
                item.Slot.ItemScript.Rect.sizeDelta = new Vector2(SlotGrid.SlotSize * item.Size.x, SlotGrid.SlotSize * item.Size.y);
            }
        }
    }

    public void FillSlot(ItemClass item)
    {
        if (ItemScript.selectedItem.Item.Size.x > ItemScript.selectedItem.Item.Size.y)
            IconRect.sizeDelta = new Vector2(SlotGrid.SlotSize, ItemScript.selectedItem.Item.Size.y * SlotGrid.SlotSize / ItemScript.selectedItem.Item.Size.x);
        else
            IconRect.sizeDelta = new Vector2( ItemScript.selectedItem.Item.Size.x * SlotGrid.SlotSize / ItemScript.selectedItem.Item.Size.y, SlotGrid.SlotSize);

        item.Part.PartSlot = this;
        SlotInfo.Item = ItemScript.selectedItem.Item;
        Inspect.Refresh();
        Empty = false;
        ItemScript.selectedItem.transform.SetParent(InventoryManager.DropParent);
        IconImage.enabled = true;
        IconImage.sprite = ItemScript.selectedItem.Image.sprite;
        ItemScript.selectedItem.Image.enabled = false;
        Image.color = Color.white;

        while (item.Part.PartSlot != null)
        {
            item = item.Part.PartSlot.SlotInfo.ParentItem;

            for (int i = 0; i < 4; i++)
                ItemScript.selectedItem.Item.Part.SizeModifiers[i].AddToPart(item.Part, i);

            item.Part.SizeModifiers[ItemScript.selectedItem.Item.Part.Size.x].Add(ItemScript.selectedItem.Item.Part.Size.y);
        }

        item.Slot.ItemScript.Rect.sizeDelta = new Vector2(SlotGrid.SlotSize * item.Size.x, SlotGrid.SlotSize * item.Size.y);
    }

    private void CreateSlots(ItemClass item)
    {
        if (item != null && item.Part != null)
        {
            int siblingIndex;

            if (item.Part.PartSlot == null)
                siblingIndex = 0;
            else
                siblingIndex = item.Slot.ItemScript.transform.parent.GetSiblingIndex();

            for (int i = 0; i < item.Part.PartSlotCount(); i++)
            {
                PartSlot newPart = Instantiate(Inspect.PartSlotPrefab, transform.parent).GetComponent<PartSlot>();

                CreatedSlots.Add(newPart.gameObject);
                newPart.Inspect = Inspect;
                newPart.SlotInfo = item.Part.PartSlots(i);
                newPart.SlotInfo.ParentItem = item;
                newPart.transform.SetSiblingIndex(siblingIndex + i);

                if (newPart.SlotInfo.Item != null)
                {
                    Debug.Log(newPart.SlotInfo.Slot.ItemScript.Image.sprite);
                    newPart.IconImage.enabled = true;
                    newPart.IconImage.sprite = newPart.SlotInfo.Slot.ItemScript.Image.sprite;

                    if (newPart.SlotInfo.Item.Size.x > newPart.SlotInfo.Item.Size.y)
                        newPart.IconRect.sizeDelta = new Vector2(SlotGrid.SlotSize, newPart.SlotInfo.Item.Size.y * SlotGrid.SlotSize / newPart.SlotInfo.Item.Size.x);
                    else
                        newPart.IconRect.sizeDelta = new Vector2(newPart.SlotInfo.Item.Size.x * SlotGrid.SlotSize / newPart.SlotInfo.Item.Size.y, SlotGrid.SlotSize);
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