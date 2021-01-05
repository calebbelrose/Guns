using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.ComponentModel;

public abstract class Pickupable : ScriptableObject
{
    public abstract void AddToInventory(Inventory inventory, GameObject gameObject, int amount);
    public abstract string GetName();

    public static String GetEnumDescription(Enum e)
    {
        FieldInfo fieldInfo = e.GetType().GetField(e.ToString());

        DescriptionAttribute[] enumAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (enumAttributes.Length > 0)
            return enumAttributes[0].Description;

        return e.ToString();
    }
}

public enum PickupableType
{
    Ammo,
    Lethal,
    Tactical
}