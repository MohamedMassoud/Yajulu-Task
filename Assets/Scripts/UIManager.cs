using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI scoreText;
    public GameObject mainMenu;
    public GameObject gameoverMenu;
    [SerializeField] private TextMeshProUGUI floatingScoreText;
    [SerializeField] private TextMeshProUGUI endScoreText;

    private LevelManager levelManager;
    private Vector3 floatingScoreTextStartPosition;

    
    private void Start()
    {
        levelManager = SingletonFactory.Instance.levelManager;
        floatingScoreTextStartPosition = floatingScoreText.transform.position;

    }
    public void ResumeGame()
    {
        levelManager.StartGame();
    }
    public void ExitGame()
    {
        levelManager.ExitGame();
    }


    public void SetScoreToUI(float value)
    {
        scoreText.text = $"Score: {(int)value}";
    }

    public void SetDistanceToUI(float value)
    {
        distanceText.text = $"Distance: {(int)value}";
    }

    public void ShowMainMenu()
    {
        LevelManager.startPaused = true;
        levelManager.RestartGame();
    }

    
    
    public void HideMainMenu()
    {
        PopMenuOff(mainMenu, () => {
            mainMenu.SetActive(false);
            }
        );
    }

    public void ShowGameoverMenu()
    {
        
        PopMenu(gameoverMenu, () => { levelManager.PauseGame(); });
        gameoverMenu.SetActive(true);
        endScoreText.text = $"Your {scoreText.text}";
        
    }

    public void HideGameoverMenu()
    {
        PopMenuOff(gameoverMenu, () =>
        {
            gameoverMenu.SetActive(false);
        }
        );
    }

    


    public void RestartGame()
    {
        LevelManager.startPaused = false;
        levelManager.RestartGame();

    }

    public void StartGame()
    {
        levelManager.gameStarted = true;
        levelManager.StartGame();
    }

    public void ShowScoreAnimation(float scoreValue)
    {
        floatingScoreText.enabled = true;
        floatingScoreText.text = ((int)scoreValue).ToString();
        LeanTween.move(floatingScoreText.gameObject, scoreText.transform.position, 0.5f).setOnComplete(()=> { 
            levelManager.score += (int)scoreValue;
            SetScoreToUI(levelManager.score);
            floatingScoreText.transform.position = floatingScoreTextStartPosition;
            floatingScoreText.enabled = false;
        });
    }

    public void HideMainMenuInstantly()
    {
        mainMenu.SetActive(false);
    }

    public void PopMenu(GameObject menu, Action action)
    {
        menu.transform.localScale *= 0.1f;

        LeanTween.scale(menu, menu.transform.localScale * 12.5f, 0.5f).setOnComplete(() => {
            LeanTween.scale(menu, menu.transform.localScale / 1.25f, 0.25f).setOnComplete(()=> action?.Invoke());
            
        });
    }
    
    public void PopMenuOff(GameObject menu, Action action)
    {

        LeanTween.scale(menu, menu.transform.localScale * 1.25f, 0.25f).setOnComplete(() => {
            LeanTween.scale(menu, Vector3.zero, 1f).setOnComplete(()=> action?.Invoke());
            
        });
    }
}
