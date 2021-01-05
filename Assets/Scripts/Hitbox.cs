


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public Health Health;
    public float multiplier = 1f;

    public void TakeDamage(float damage)
    {
        Health.TakeDamage(damage * multiplier);
    }
}
