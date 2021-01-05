﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IDragHandler
{
    private bool isDragging = false;

    public void OnDrag(PointerEventData eventData)
    {
        transform.Translate(eventData.delta.x, eventData.delta.y, 0.0f);
    }
}
