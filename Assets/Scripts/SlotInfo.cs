using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotInfo
{
    public ItemScript Item;
    public Part ParentPart;
    public string Name;
    public List<int> PartIDs;
    public bool Required;
}
