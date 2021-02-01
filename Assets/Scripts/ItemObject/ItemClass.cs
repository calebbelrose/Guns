using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ItemClass
{
    public int GlobalID { get { return globalID; } }
    public CategoryName CategoryName { get { return categoryName; } }
    public string TypeName { get { return typeName; } }
    public IntVector2 Size { get { if (Part != null) return BaseSize + Part.SizeModifier; else return BaseSize; } }
    public IntVector2 BaseSize { get { return size; } }
    public Sprite Icon { get { return icon; } }
    public Part Part { get { return part; } }
    public SlotScript Slot;

    [SerializeField] private int globalID;
    [SerializeField] private CategoryName categoryName;
    [SerializeField] private string typeName;
    [SerializeField] private IntVector2 size;
    [SerializeField] private Sprite icon;
    [SerializeField] private Part part;

    public ItemClass Copy()
    {
        return (ItemClass)this.MemberwiseClone();
    }
}