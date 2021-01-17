using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Modifiers
{
    public int HighestModifier { get { return highestModifier; } }

    [SerializeField] private int highestModifier;
    [SerializeField] private List<int> list = new List<int>();

    public void Add(int modifier)
    {
        list.Add(modifier);

        if (modifier > highestModifier)
            highestModifier = modifier;
    }

    public int Remove(int oldModifier)
    {
        list.Remove(oldModifier);

        if (oldModifier == highestModifier)
        {
            highestModifier = 0;

            foreach (int modifier in list)
            {
                if (modifier == oldModifier)
                    break;
                else if (modifier > highestModifier)
                    highestModifier = modifier;
            }
        }

        return oldModifier - highestModifier;
    }

    public int RemoveFromPart(Part part, int index)
    {
        int oldHighestModifier = highestModifier;

        foreach (int modifier in list)
            part.SizeModifiers[index].Remove(modifier);

        return oldHighestModifier - highestModifier;
    }

    public void AddToPart(Part part, int index)
    {
        foreach (int modifier in list)
            part.SizeModifiers[index].Add(modifier);
    }
}
