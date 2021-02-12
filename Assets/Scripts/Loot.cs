using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Loot : MonoBehaviour
{
    [SerializeField] protected string Name;

    //Highlights loot and displays its name
    private void OnMouseEnter()
    {
        InventoryManager.LootText.transform.parent.gameObject.SetActive(true);
        InventoryManager.LootText.text = Name;
    }

    //Removes highlight from loot and hides its name
    private void OnMouseExit()
    {
        InventoryManager.LootText.transform.parent.gameObject.SetActive(false);
    }

    public abstract void Action(AdvancedCamRecoil playerCam);
    public abstract bool Action(AIMovement aiMovement);
}
