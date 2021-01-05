using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    int lethalAmount = 0;
    int tacticalAmount = 0;
    LethalType lethal;
    TacticalType tactical;
    int[] ammo = new int[] { 0, 0, 0, 0 }; // 9mm .456 .50 cal, slugs

    public void AddToInventory(GameObject gameObject, LethalType addedLethal, int amount)
    {
        if (lethal == addedLethal)
            lethalAmount += amount;
        else
        {
            lethal = addedLethal;
            lethalAmount = amount;
        }

        Destroy(gameObject);
    }

    public void AddToInventory(GameObject gameObject, TacticalType addedTactical, int amount)
    {
        if (tactical == addedTactical)
            tacticalAmount += amount;
        else
        {
            tactical = addedTactical;
            tacticalAmount = amount;
        }

        Destroy(gameObject);
    }

    public void AddToInventory(GameObject gameObject, AmmoType addedAmmo, int amount)
    {
        ammo[(int)addedAmmo] += amount;
        Destroy(gameObject);
    }
}