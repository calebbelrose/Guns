using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatMultiplier
{
    public float MultiplierValue;
    public StatName Name;

    public StatMultiplier(StatName name, float multiplierValue)
    {
        Name = name;
        MultiplierValue = multiplierValue;
    }
}
