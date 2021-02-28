using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Ammo")]
public class Ammo : ScriptableObject
{
    [SerializeField] private AmmoType AmmoType;
    [SerializeField] private int Fragments;
    [SerializeField] private float Damage;
    [SerializeField] private float Penetration;
    [SerializeField] private float ArmourDamage;
    [SerializeField] private float FragmentationChance;
    [SerializeField] private float BleedChanceLimbs;
    [SerializeField] private float BleedChanceHead;
}
