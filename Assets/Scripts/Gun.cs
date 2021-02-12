using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Vector3 idleLocation, aimingLocation;
    public Transform BulletSpawn;
    public AudioSource AudioSource;
    public float Maneuverability = 5f;
    public float ShotDelay = 5f;
    public float GunLength;
    public Magazine Magazine;
    public CharacterInventory Inventory;

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(new Ray(BulletSpawn.position, -BulletSpawn.forward), out hit, GunLength))
            transform.Translate(BulletSpawn.position - hit.collider.ClosestPointOnBounds(hit.point));
    }

    public bool UseAmmo()
    {
        if (Magazine != null && Magazine.Ammo.Count > 0)
        {
            //Magazine.Ammo.RemoveAt(0);
            return true;
        }
        else
            return false;
    }

    public IEnumerator Reload(GameObject MagazinePrefab, Transform parent)
    {
        Magazine OldMagazine = Magazine, NewMagazine;

        Magazine = null;
        OldMagazine.transform.SetParent(null);
        OldMagazine.gameObject.AddComponent<Rigidbody>();
        yield return new WaitForSeconds(OldMagazine.UnloadTime);
        Destroy(OldMagazine.gameObject);
        NewMagazine = Instantiate(MagazinePrefab, parent).GetComponent<Magazine>();
        NewMagazine.MagazineReload.enabled = true;
        NewMagazine.MagazineReload.Setup(NewMagazine.ReloadTime);
        yield return new WaitForSeconds(NewMagazine.ReloadTime);
        Magazine = NewMagazine;
    }
}