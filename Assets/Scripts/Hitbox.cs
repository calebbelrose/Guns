using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public CombatController CombatController;
    public float multiplier = 1f;
    public Collider Collider { get { return hitCollider; } }

    [SerializeField] private int index;
    [SerializeField] private Collider hitCollider;

    public void TakeDamage(float damage, CombatController attacker)
    {
        CombatController.TakeDamage(damage, multiplier, attacker);
    }
}
