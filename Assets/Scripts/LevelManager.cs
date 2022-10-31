using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    

    private PoolManager poolManager;
    private UIManager uiManager;
    private GameObject[] environmentLayers = new GameObject[3];
    [SerializeField] internal List<Rock> rocksScriptableObjects;
    [SerializeField] private GameObject objectsInstantiator;
    [SerializeField] private GameObject rocksHolder;
    [SerializeField] private GameObject electricityHolder;
    [SerializeField] private GameObject environmentsHolder;
    [SerializeField] private float maxTimeBetweenRocks;
    [SerializeField] private float minTimeBetweenRocks;
    [SerializeField] private float minTimeBetweenElectricity;
    [SerializeField] private float maxTimeBetweenElectricity;
    public float electricityDamage = 15f;
    public float temperatureDamage = 10f;

    public static bool startPaused = true;
    internal int score;
    internal float distance;
    private PlayerController playerController;
    private int scoreRound = 1;
    private float maxDistanceBeforeScore = 100f;
    private float scoreValue = 1000f;
    public bool gameOver = false;
    public bool gameStarted = false;
    

    private MeshRenderer objectsInstantiatorMeshRenderer;
    private void Start()
    {
        LeanTween.init(10000);
        uiManager = SingletonFactory.Instance.uiManager;
        playerController = SingletonFactory.Instance.playerController;
        InitEnvironment();
        objectsInstantiatorMeshRenderer = objectsInstantiator.GetComponent<MeshRenderer>();
        StartMenu();
    }


    private void InitEnvironment()
    {
        poolManager = SingletonFactory.Instance.poolManager;


        GameObject environment1 = poolManager.GetPooledObject("Environment");
        
        environment1.SetActive(true);
        environmentLayers[1] = environment1;

        GameObject environment2 = poolManager.GetPooledObject("Environment");
        environment2.SetActive(true);
        environmentLayers[2] = environment2;
        environmentLayers[2].transform.position = environmentLayers[1].transform.position + new Vector3(0f, 0f, 3000f);

        environment1.transform.SetParent(environmentsHolder.transform, false);
        environment2.transform.SetParent(environmentsHolder.transform, false);
        StartCoroutine(RockSpawnerRoutine());
        
    }

    public void UpdateEnvironmentLayers()
    {
        if (environmentLayers[0] != null) environmentLayers[0].SetActive(false);
        environmentLayers[0] = environmentLayers[1];
        environmentLayers[1] = environmentLayers[2];
        environmentLayers[2] = poolManager.GetPooledObject("Environment");
        environmentLayers[2].transform.SetParent(environmentsHolder.transform, false);
        environmentLayers[2].SetActive(true);
        environmentLayers[2].transform.position = environmentLayers[1].transform.position + new Vector3(0f, 0f, 3000f);

    }


    private Vector3 GetRandomPosition()
    {
        float xRange = objectsInstantiatorMeshRenderer.bounds.size.x / 2;
        float yRange = objectsInstantiatorMeshRenderer.bounds.size.y / 2;
        float zRange = objectsInstantiatorMeshRenderer.bounds.size.z / 2;
        Vector3 randomPosition = new Vector3(Random.Range(-xRange, xRange), Random.Range(-yRange, yRange), Random.Range(-zRange, zRange)) + objectsInstantiator.transform.position;

        return randomPosition;
    }

    private void SpawnRock()
    {
        Vector3 randomPosition = GetRandomPosition();

        int randomRockIndex = GetRandomRockIndex();
        GameObject spawnedRock = poolManager.GetPooledObject(rocksScriptableObjects[randomRockIndex].rockPrefab.tag);
        spawnedRock.SetActive(true);
        spawnedRock.transform.SetParent(rocksHolder.transform, false);
        spawnedRock.transform.position = randomPosition;
        TrailRenderer tr = spawnedRock.GetComponent<TrailRenderer>();
        if (tr != null) tr.Clear();
    }

    private int GetRandomRockIndex()
    {
        return Random.Range(0, rocksScriptableObjects.Count);
    }

    private IEnumerator RockSpawnerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(GetRandomRockSpawnTiming());
            SpawnRock();
        }
        
    }

    private IEnumerator ElectricitySpawnerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(GetRandomElectricitySpawnTiming());
            SpawnRandomElectricity();
        }

    }

    private float GetRandomRockSpawnTiming()
    {
        return Random.Range(minTimeBetweenRocks, maxTimeBetweenRocks);
    }
    
    private float GetRandomElectricitySpawnTiming()
    {
        return Random.Range(minTimeBetweenElectricity, maxTimeBetweenElectricity);
    }



    private void SpawnRandomElectricity(string tag)
    {
        GameObject upperElectricity = poolManager.GetPooledObject(tag);
        upperElectricity.transform.position = new Vector3(upperElectricity.transform.position.x, upperElectricity.transform.position.y, GetRandomPosition().z);
        upperElectricity.transform.SetParent(electricityHolder.transform, false);
        upperElectricity.SetActive(true);
    }

    private void SpawnRandomElectricity()
    {
        int randomElectricityIndex = Random.Range(0, 4);

        switch (randomElectricityIndex)
        {
            case 0:
                SpawnRandomElectricity("Upper Electricity");
                break;
            case 1:
                SpawnRandomElectricity("Lower Electricity");
                break;
            case 2:
                SpawnRandomElectricity("Middle Electricity 1");
                break;
            case 3:
                SpawnRandomElectricity("Middle Electricity 2");
                break;
        }
    }


    public void PauseGame()
    {
        //Time.timeScale = 0f;
    }
    private void StartMenu()
    {
        playerController.ResetGravity();
        if (startPaused)
        {
            uiManager.PopMenu(uiManager.mainMenu, PauseGame);

        }
        else
        {
            uiManager.HideMainMenu();
            uiManager.HideMainMenuInstantly();
            StartGame();
        }
    }
    public void StartGame()
    {
        
        gameStarted = true;
        uiManager.HideMainMenu();
        playerController.StartSkating();
        StartCoroutine(ElectricitySpawnerRoutine());
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        ResetValues();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResetValues()
    {
        score = 0;
        distance = 0;
    }


    private void Update()
    {
        UpdateDistance();

    }

    private void UpdateDistance()
    {
        distance = playerController.transform.position.z;
        if(distance >= maxDistanceBeforeScore*scoreRound)
        {
            //score += (int)scoreValue;
            scoreRound++;
            uiManager.ShowScoreAnimation(scoreValue);
        }
        uiManager.SetDistanceToUI(distance);
    }

    public void Gameover()
    {

        gameOver = true;
        uiManager.ShowGameoverMenu();
    }
    
}
