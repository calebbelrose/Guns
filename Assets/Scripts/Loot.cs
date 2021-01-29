using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Loot : MonoBehaviour
{
    protected GameObject LootTextObject;

    public static Loot CurrentLoot { get { return currentLoot; } }

    protected static Loot currentLoot;

    public static void Destroy()
    {
        Destroy(CurrentLoot.gameObject);
        currentLoot = null;
    }

    //Highlights loot and displays its name
    private void OnMouseEnter()
    {
        LootTextObject.SetActive(true);
        LootTextObject.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        currentLoot = this;
    }

    //Removes highlight from loot and hides its name
    private void OnMouseExit()
    {
        LootTextObject.SetActive(false);
        currentLoot = null;
    }

    //Destroys gameobject
    private void OnDestroy()
    {
        GameObject.Destroy(LootTextObject);
    }

    public abstract void Action(AdvancedCamRecoil playerCam);
}
