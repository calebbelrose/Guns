using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part
{
    public PartSlot PartSlot;
    public PartType PartType { get { return partType; } }
    public Modifiers[] SizeModifiers = new Modifiers[4];
    public IntVector2 SizeModifier { get { return new IntVector2(SizeModifiers[1].HighestModifier + SizeModifiers[3].HighestModifier, SizeModifiers[0].HighestModifier + SizeModifiers[2].HighestModifier); } }
    public IntVector2 Size; // index, modifier

    [SerializeField] private PartType partType;
    [SerializeField] private List<SlotInfo> partSlots;

    public SlotInfo PartSlots(int index)
    {
        return partSlots[index];
    }

    public int PartSlotCount()
    {
        return partSlots.Count;
    }
}