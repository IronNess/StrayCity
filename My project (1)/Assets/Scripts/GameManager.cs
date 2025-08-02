using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Start, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour

{
    public static GameManager Instance;

    public GameObject rat2;

    [Header("Panels")]
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("HUD Icons")]
    public GameObject[] fishIcons;
    public GameObject ratIcon;

    private int fishCollected = 0;
    private bool ratCaught = false;

    [Header("Game Completion Settings")]
    public int totalFish = 3;

    [Header("Tutorial")]
    public GameObject tutorial;

    public GameState CurrentState { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() 
    {
        if (tutorial != null) tutorial.SetActive(false);
        ChangeState(GameState.Start);
        
    }

    void Update()
    {
        if (CurrentState == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
            ChangeState(GameState.Paused);
        else if (CurrentState == GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
            ChangeState(GameState.Playing);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        startPanel.SetActive(newState == GameState.Start);
        gamePanel.SetActive(newState == GameState.Playing);
        pausePanel.SetActive(newState == GameState.Paused);
        gameOverPanel.SetActive(newState == GameState.GameOver);

        Time.timeScale = (newState == GameState.Paused || newState == GameState.GameOver) ? 0f : 1f;
    }

    public void StartGame()
    {
     ChangeState(GameState.Playing);
     if (tutorial != null)
     tutorial.SetActive(true);
    }


    public void ResumeGame() => ChangeState(GameState.Playing);
    public void Restart()    => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void Quit()       => Application.Quit();
    public void GameOver()   => ChangeState(GameState.GameOver);

  public void CollectFish()
{
    if (fishCollected >= fishIcons.Length) return;

    fishIcons[fishCollected].SetActive(false);
    fishCollected++;

    if (fishCollected == totalFish && !ratCaught)
    {
        if (rat2 !=null)
        {
        Debug.Log("Rat Spawned!");
        ratIcon.SetActive(true);
        rat2.SetActive(true);
        }
    }
        CheckEndGameCondition();
}

public void CloseTutorial()
{
    tutorial.SetActive(false);
}


private void CheckEndGameCondition()
{
    if (fishCollected == totalFish && ratCaught)
    {
        GameOver();
    }
}

public void CatchRat()
{
    ratCaught = true;

    if (ratIcon != null)

ratIcon.SetActive(false);

CheckEndGameCondition();
}

}


