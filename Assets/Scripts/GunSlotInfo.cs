using System;

[Serializable]
public class GunSlotInfo : EquipSlotInfo
{
    public override bool CanEquip(ItemClass item)
    {
        return item.CategoryName == categoryName && item.AllRequiredParts();
    }
}
