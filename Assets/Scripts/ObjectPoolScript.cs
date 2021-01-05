using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolScript : MonoBehaviour
{
    [SerializeField] private GameObject Prefab;

    private Stack<GameObject> inactiveInstances = new Stack<GameObject>();

    //Gets spawned gameobject
    public ItemScript GetItemScript()
    {
        GameObject spawnedGameObject;

        if (inactiveInstances.Count > 0)
            spawnedGameObject = inactiveInstances.Pop();
        else
        {
            spawnedGameObject = (GameObject)GameObject.Instantiate(Prefab);
            spawnedGameObject.AddComponent<PooledObject>().Pool = this;
        }

        return spawnedGameObject.GetComponent<ItemScript>();
    }

    //Destroys gameobject
    public void ReturnObject(GameObject toReturn)
    {

        PooledObject pooledObject = toReturn.GetComponent<PooledObject>();

        if (pooledObject != null && pooledObject.Pool == this)
        {
            toReturn.transform.SetParent(transform);
            toReturn.transform.position = this.transform.position;
            toReturn.SetActive(false);
            inactiveInstances.Push(toReturn);
        }
        else
        {
            Debug.LogWarning(toReturn.name + " was returned to a pool it wasn't spawned from! Destroying.");
            Destroy(toReturn);
        }
    }
}

public class PooledObject : MonoBehaviour
{
    public ObjectPoolScript Pool;
}

