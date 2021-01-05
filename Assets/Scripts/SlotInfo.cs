﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotInfo
{
    public string Name;
    public List<int> PartIDs;
    public GunPart GunPart;
    public bool Required;
}