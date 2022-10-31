using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToPool;
    [SerializeField] private int poolSizePerObject = 3;
    
    private Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, GameObject> pooledObjectsReferenceList = new Dictionary<string, GameObject>();

    private void Awake()
    {
        foreach (GameObject objectToPool in objectsToPool)
        {
            string objectTag = objectToPool.tag;

            AddReferenceObject(objectTag, objectToPool);


            List<GameObject> poolList = CreateNewPoolList(objectToPool);
            AddNewListToPool(objectTag, poolList);
        }
    }

    private void AddReferenceObject(string objectTag, GameObject objectToPool)
    {

        objectToPool.SetActive(false);
        pooledObjectsReferenceList.Add(objectTag, objectToPool);
    }
    private List<GameObject> CreateNewPoolList(GameObject objectToPool)
    {
        List<GameObject> poolList = new List<GameObject>();
        for (int i = 0; i < poolSizePerObject; i++)
        {
            GameObject pooledObject = Instantiate(objectToPool);
            poolList.Add(pooledObject);
            pooledObject.SetActive(false);
            
        }
        return poolList;
    }

    private void AddObjectToPool(GameObject objectToPool)
    {
        List<GameObject> poolList;
        if(!pool.TryGetValue(objectToPool.tag, out poolList))
        {
            Debug.LogWarning($"GameObject with tag: {objectToPool.tag} is not a pooled Object");
        }
        else
        {
            poolList.Add(objectToPool);
            UpdateListInPool(objectToPool.tag, poolList);
        }
        
    } 
    private void AddNewListToPool(string objectTag, List<GameObject> poolList)
    {
        pool.Add(objectTag, poolList);
    }

    private void UpdateListInPool(string objectTag, List<GameObject> poolList)
    {
        pool[objectTag] = poolList;
        
    }
    public GameObject GetPooledObject(string objectTag)
    {
        
        List<GameObject> list;
        if (!pool.TryGetValue(objectTag, out list)){
            Debug.LogWarning($"GameObject with tag: {objectTag} is not a pooled Object");
            return null;
        }


        GameObject returnedObject = null;


        foreach(GameObject listObject in list)
        {
            if (listObject.activeInHierarchy) continue;

            returnedObject = listObject;
            break;
        }

        if(returnedObject == null)
        {
            GameObject referenceClone;
            if(!pooledObjectsReferenceList.TryGetValue(objectTag, out referenceClone))
            {
                Debug.LogWarning($"GameObject with tag: {objectTag} is not a pooled Object");
                return null;
            }
            returnedObject = Instantiate(referenceClone);
            AddObjectToPool(returnedObject);
        }
        return returnedObject;
    }



}
