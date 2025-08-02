using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Start, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    // singleton instance
    public static GameManager Instance { get; private set; } 

    // ─── Gameplay reference
    [Header("Gameplay")]
    [SerializeField, Min(1)] private int totalFish = 3; // total fish needed to spawn
    [SerializeField]        private GameObject ratPrefab;   // disabled in scene

    // ─── UI panels
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    // HUD Elements
    [Header("HUD")]
    [SerializeField] private GameObject[] fishIcons; // Fish icon
    [SerializeField] private GameObject   ratIcon; // Rat icon

    // Tutorial Overlay
    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialCanvas;

    // Game State 
    private int  fishCollected; // Count of fish collected so far
    private bool ratCaught; // Flag whether rat caught
    public  GameState CurrentState { get; private set; }


    // Lifecycle Methods

    private void Awake()
    {
        if (Instance == null) Instance = this; // Ensure only 1 game manager exists
        else { Destroy(gameObject); return; }
    }

    // Hide tutorial on launch and go to start state
    private void Start()
    {
        tutorialCanvas?.SetActive(false);
        ChangeState(GameState.Start);
    }

    // Toggle pause with Escape key
    private void Update()
    {
        if (CurrentState == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
            ChangeState(GameState.Paused);
        else if (CurrentState == GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
            ChangeState(GameState.Playing);
    }

    // Public Game State control
    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        // Toggle UI panels
        startPanel.SetActive(newState == GameState.Start);
        gamePanel .SetActive(newState == GameState.Playing);
        pausePanel.SetActive(newState == GameState.Paused);
        gameOverPanel.SetActive(newState == GameState.GameOver);

        // Control time flow
        Time.timeScale = (newState == GameState.Paused || newState == GameState.GameOver) ? 0 : 1;

        // Play background music based on state
        if (newState == GameState.Start || newState == GameState.Paused)
        AudioManager.Instance.PlayIntroMusic();
    else if (newState == GameState.Playing)
        AudioManager.Instance.PlayGameplayMusic();
    }

    public void StartGame()
    {
        ChangeState(GameState.Playing);
        tutorialCanvas?.SetActive(true);
    }

    public void ResumeGame() => ChangeState(GameState.Playing);
    public void Restart   () => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void Quit      () => Application.Quit();
    public void GameOver  () => ChangeState(GameState.GameOver);

    public void CollectFish()
    {
        if (fishCollected >= fishIcons.Length) return;

        fishIcons[fishCollected].SetActive(false);  // hide icon on collection
        fishCollected++;

        if (fishCollected == totalFish && !ratCaught)
            SpawnRat();

        CheckEndCondition();
    }

    public void CatchRat()
    {
        ratCaught = true;
        ratIcon?.SetActive(false);
        CheckEndCondition();
    }

    public void CloseTutorial() => tutorialCanvas?.SetActive(false);

    //  Internal helpers
    private void SpawnRat()
    {
        if (!ratPrefab) return;

        ratPrefab.SetActive(true);
        ratIcon?.SetActive(true);
    }

    private void CheckEndCondition()
    {
        if (fishCollected >= totalFish && ratCaught)
            GameOver();
    }
}
