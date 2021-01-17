using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Close : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject InspectObject;

    public void OnPointerClick(PointerEventData eventData)
    {
        InspectObject.SetActive(false);
    }
}
