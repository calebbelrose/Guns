using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Loot : MonoBehaviour
{
    public int ItemID { get { return itemID; } }

    [SerializeField] private int itemID;

    protected GameObject LootObject;

    public static Loot CurrentLoot { get; private set; }

    public static void Destroy()
    {
        Destroy(CurrentLoot.gameObject);
        CurrentLoot = null;
    }

    //Highlights loot and displays its name
    private void OnMouseEnter()
    {
        LootObject.SetActive(true);
        LootObject.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        CurrentLoot = this;
    }

    //Removes highlight from loot and hides its name
    private void OnMouseExit()
    {
        LootObject.SetActive(false);
        CurrentLoot = null;
    }

    //Sets up loot
    void Start()
    {
        LootObject = ItemDatabase.Instance.CreateLoot();
        LootObject.transform.GetChild(0).GetComponent<Text>().text = ItemDatabase.Instance.DBList(itemID).TypeName;
        LootObject.SetActive(false);
    }

    //Destroys gameobject
    private void OnDestroy()
    {
        GameObject.Destroy(LootObject);
    }
}