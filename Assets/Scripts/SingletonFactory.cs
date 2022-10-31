using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonFactory : MonoBehaviour
{
    public static SingletonFactory Instance;


    public PoolManager poolManager;
    public LevelManager levelManager;
    public PlayerController playerController;
    public UIManager uiManager;
    public SoundManager soundManager;
    public PPController ppController;

    
    private void Awake()
    {
        Instance = this;
    }


    
}
