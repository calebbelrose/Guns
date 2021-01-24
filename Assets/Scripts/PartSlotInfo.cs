using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartSlotInfo
{
    public ItemScript ItemScript;
    public ItemScript ParentItem;
    public string Name;
    public List<int> PartIDs;
    public bool Required;
}
