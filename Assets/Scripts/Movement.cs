using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CombatController CombatController { get { return combatController; } }
    public CharacterInventory Inventory { get { return inventory; } }

    [SerializeField] protected CharacterInventory inventory;
    [SerializeField] protected Transform Body;
    [SerializeField] protected GameObject BulletPrefab;
    [SerializeField] protected AudioClip AudioClip;
    [SerializeField] protected Gun Gun;

    [SerializeField] private CombatController combatController;

    protected Vector2 totalRotation = Vector2.zero;
    protected Vector3 recoilrotation = new Vector3(2f, 2f, 2f), recoilrotationaiming = new Vector3(0.5f, 0.5f, 1.5f), targetGunLocation;
    protected bool canShoot = true, aiming;
    protected int consecutiveShots = 0;
}
