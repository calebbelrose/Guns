using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public CombatController CombatController;
    public float multiplier = 1f;

    [SerializeField] private int index;

    public void TakeDamage(float damage, CombatController attacker)
    {
        CombatController.TakeDamage(damage, multiplier, attacker);
    }
}
