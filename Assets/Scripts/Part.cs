using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GunPart")]
public class Part : ScriptableObject
{
    public PartSlot PartSlot;
    public PartType PartType { get { return partType; } }
    public List<PartSize> SizeModifiers = new List<PartSize>();

    [SerializeField] private PartType partType;
    [SerializeField] private List<SlotInfo> partSlots;
    [SerializeField] private PartSize partSize;

    public SlotInfo PartSlots(int index)
    {
        return partSlots[index];
    }
    
    public void AddModifier(PartSize modifier)
    {
        SizeModifiers.Add(modifier);

        PartSlot.SlotInfo.Item.UpdateSize();   
    }

    public int PartSlotCount()
    {
        return partSlots.Count;
    }
}