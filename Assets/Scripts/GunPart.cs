using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GunPart")]
public class GunPart : ScriptableObject
{
    public PartType PartType { get { return partType; } }
    public PartSize PartSize { get { return PartSize; } }

    [SerializeField] private PartType partType;
    [SerializeField] private List<SlotInfo> partSlots;
    [SerializeField] private PartSize partSize;

    public SlotInfo PartSlots(int index)
    {
        return partSlots[index];
    }

    public int PartSlotCount()
    {
        return partSlots.Count;
    }
}