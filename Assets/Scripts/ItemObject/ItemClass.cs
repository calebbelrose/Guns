using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ItemClass
{
    public int GlobalID { get { return globalID; } }
    public CategoryName CategoryName { get { return categoryName; } }
    public string TypeName { get { return typeName; } }
    public IntVector2 BaseSize { get { return size; } }
    public Sprite Icon { get { return icon; } }
    public SlotScript Slot;
    public PartSlot PartSlot;
    public PartType PartType { get { return partType; } }
    public Modifiers[] SizeModifiers = new Modifiers[4];
    public IntVector2 SizeModifier { get { return new IntVector2(SizeModifiers[1].HighestModifier + SizeModifiers[3].HighestModifier, SizeModifiers[0].HighestModifier + SizeModifiers[2].HighestModifier); } }
    public IntVector2 Size { get { return BaseSize + SizeModifier; } }
    public IntVector2 PartSize; // index, modifier
    public SlotGridList SlotGridList;

    [SerializeField] private PartType partType;
    [SerializeField] private int globalID;
    [SerializeField] private CategoryName categoryName;
    [SerializeField] private string typeName;
    [SerializeField] private IntVector2 size;
    [SerializeField] private Sprite icon;
    [SerializeField] private List<PartSlotInfo> partSlots;

    public PartSlotInfo PartSlots(int index)
    {
        return partSlots[index];
    }

    public bool AllRequiredParts()
    {
        bool allRequiredParts = true;
        int i = 0;

        while( i < partSlots.Count && allRequiredParts)
            allRequiredParts = partSlots[i].Required && !partSlots[i].Empty && partSlots[i].Item.AllRequiredParts();

        return allRequiredParts;
    }

    public int PartSlotCount()
    {
        return partSlots.Count;
    }


    public ItemClass Copy()
    {
        return (ItemClass)this.MemberwiseClone();
    }
}