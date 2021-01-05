using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public ObjectPoolScript ItemEquipPool { get { return itemEquipPool; } }

    [SerializeField] private ObjectPoolScript itemEquipPool;
    [SerializeField] private List<LootDrop> RareLootDrops = new List<LootDrop>();
    [SerializeField] private Transform LootParent;
    [SerializeField] private ObjectPoolScript ObjectPoolScript;
    [SerializeField] private List<ItemClass> dbList = new List<ItemClass>();
    [SerializeField] private GameObject LootTextPrefab;

    private int totalLootDropWeights = 0;

    public static ItemDatabase Instance { get; private set; } = null;

    //Initializes singleton, loads database items from text file and prepares the rare loot drops for loot selection
    private void Awake()
    {
        string[] lines = File.ReadAllLines("./Assets/Scripts/CreateItem/DataBase.csv");

        if (Instance != null && Instance != this)
            Destroy(this.gameObject);

        Instance = this;
        DontDestroyOnLoad(transform.parent.parent.gameObject);
        RareLootDrops = RareLootDrops.OrderBy(x => x.Weight).ToList();

        foreach (string line in lines)
        {
            string[] data = line.Split(',');

            //DBList.Add(new ItemClass(Int32.Parse(data[0]), (CategoryName)Enum.Parse(typeof(CategoryName), data[1]), data[2], new IntVector2(Int32.Parse(data[3]), Int32.Parse(data[4])), Resources.Load<Sprite>("ItemIcons/" + data[2])));
        }

        foreach (LootDrop drop in RareLootDrops)
            totalLootDropWeights += drop.Weight;
    }

    //Spawns a rare loot drop at the location based on the player's magic find chance
    public void SpawnLoot(Vector3 position)
    {
        Instantiate(Resources.Load("Loot/" + RareLootDrops[UnityEngine.Random.Range(0, RareLootDrops.Count)].ItemClass.TypeName) as GameObject).transform.position = position;
    }

    //Creates the specified loot with the set parent
    public GameObject CreateLoot()
    {
        return Instantiate(LootTextPrefab, LootParent);
    }
    
    public ItemClass DBList(int index)
    {
        return dbList[index];
    }

    //Returns the specified object
    public void ReturnObject(GameObject toReturn)
    {
        ObjectPoolScript.ReturnObject(toReturn);
    }

        /*public void SpawnLoot(Vector3 position, int itemID, int amount)
        {
            int weightSum = 0;
            int index = 0;
            int roll;
            ItemClass newItem;
            GameObject newLoot;

            foreach (LootDrop drop in RareLootDrops)
                weightSum += drop.Weight;
            roll = UnityEngine.Random.Range(0, weightSum);

            while (index < RareLootDrops.Count && roll <= RareLootDrops[index].Weight)
                roll -= RareLootDrops[index].Weight;

            newItem = new ItemClass(itemID, );
            newLoot = Instantiate(Resources.Load("Loot/" + newItem.TypeName) as GameObject);
            newLoot.GetComponent<Loot>().item = newItem;
            newLoot.transform.position = position;
        }*/
    }