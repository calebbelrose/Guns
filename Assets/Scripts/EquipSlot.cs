using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public EquipSlotInfo EquipSlotInfo { get; private set; }
    public ItemScript ItemScript { get; protected set; }

    public Image Image { get { return image; } }

    [SerializeField] private Image image;
    [SerializeField] protected CharacterInventory Inventory;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ItemScript.selectedItem != null)
        {
            if (EquipSlotInfo.CanEquip(ItemScript.selectedItem.Item))
                Image.color = Color.green;
            else
                Image.color = Color.red;
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (EquipSlotInfo.Empty && eventData.button == PointerEventData.InputButton.Left && ItemScript.selectedItem != null)
        {
            ItemScript.selectedItem.transform.position = EquipSlotInfo.EquipSlot.transform.position;
            ItemScript.selectedItem.transform.SetParent(transform);
            Inventory.EquipInSlot(ItemScript.selectedItem.Item, EquipSlotInfo);
            ItemScript.ResetSelectedItem();
        }
        else
            Inventory.Unequip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Image.color = Color.white;
    }

    public void Setup(CharacterInventory inventory, EquipSlotInfo equipSlotInfo, EquipSlot equipSlot)
    {
        Inventory = inventory;
        EquipSlotInfo = equipSlotInfo;
        EquipSlotInfo.EquipSlot = equipSlot;
    }

    public void SetInfo(EquipSlotInfo equipSlotInfo)
    {
        EquipSlotInfo = equipSlotInfo;
    }

    public void Clear()
    {
        if (ItemScript != null)
            Destroy(ItemScript.gameObject);
    }

    public virtual void Display()
    {
        if (!EquipSlotInfo.Empty)
        {
            ItemScript = ItemDatabase.CreateItemScript(EquipSlotInfo.ItemClass, transform);
            ItemScript.transform.localPosition = Vector3.zero;
        }
    }
}