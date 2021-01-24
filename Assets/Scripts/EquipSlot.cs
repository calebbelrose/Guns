﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool Empty = true;
    public EquipSlotInfo EquipSlotInfo;

    public Image Image { get { return image; } }
    public GameObject EquipObject { get {return equipObject; } }

    [SerializeField] private Image image;
    [SerializeField] private GameObject equipObject;
    [SerializeField] private CombatController CombatController;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ItemScript.selectedItem != null)
        {
            if (ItemScript.selectedItem.Item.CategoryName == EquipSlotInfo.CategoryName)
                Image.color = Color.green;
            else
                Image.color = Color.red;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Empty)
            CombatController.Unequip(this);
        else if (eventData.button == PointerEventData.InputButton.Left && ItemScript.selectedItem != null)
            CombatController.EquipInSlot(ItemScript.selectedItem, EquipSlotInfo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
            Image.color = Color.white;
    }
}