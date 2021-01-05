using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    public List<Ammo> Ammo;
    public int Size;
    public AmmoType AmmoType;
    public float UnloadTime, ReloadTime;
    public MagazineReload MagazineReload;
}