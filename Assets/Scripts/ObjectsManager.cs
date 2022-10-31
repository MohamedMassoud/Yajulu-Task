using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : MonoBehaviour
{
    [SerializeField] private GameObject rockHolder;
    [SerializeField] private GameObject electricityHolder;
    [SerializeField] private float electricitySpeed = 10f;
    [SerializeField] private float electricityRotationSpeed = 10f;

    private LevelManager levelManager;
    void Start()
    {
        levelManager = SingletonFactory.Instance.levelManager;
    }

    
    void Update()
    {
        MoveAllRocks();
        MoveAllElectricity();
    }
    private List<GameObject> GetAllActiveRocks()
    {
        List<GameObject> rocks = new List<GameObject>();
        foreach(Transform child in rockHolder.transform)
        {
            if(child.gameObject.activeInHierarchy)
            rocks.Add(child.gameObject);
        }

        return rocks;
    }

    private List<GameObject> GetAllActiveElectricity()
    {
        List<GameObject> electricity = new List<GameObject>();
        foreach (Transform child in electricityHolder.transform)
        {
            if (child.gameObject.activeInHierarchy)
                electricity.Add(child.gameObject);
        }

        return electricity;
    }

    private void MoveAllRocks()
    {
        List<GameObject> rocks = GetAllActiveRocks();
        foreach(GameObject rock in rocks)
        {


            Rock rockComponent = GetScriptableObject(rock);

            rock.transform.Translate(Vector3.left * rockComponent.rockSpeed * Time.deltaTime);
            rock.transform.Rotate(Vector3.left, rockComponent.rockRotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void MoveAllElectricity()
    {
        List<GameObject> electricity = GetAllActiveElectricity();
        foreach (GameObject electric in electricity)
        {
            electric.transform.Translate(Vector3.back * electricitySpeed * Time.deltaTime);
            electric.transform.Rotate(Vector3.back, electricityRotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    private Rock GetScriptableObject(GameObject rockObject)
    {
        Rock scriptableObject = null;
        foreach(Rock rock in levelManager.rocksScriptableObjects)
        {
            if(rockObject.tag == rock.rockPrefab.tag)
            {
                scriptableObject = rock;
                continue;
            }
        }

        return scriptableObject;
    }
}
