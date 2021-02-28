using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SlotGridList
{
    public List<SlotGrid> List = new List<SlotGrid>();
    public IntVector2 GridSize;
    public Vector2 Size;
}