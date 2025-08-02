using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Start, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ─── Gameplay ───────────────────────────────────────────────────────────
    [Header("Gameplay")]
    [SerializeField, Min(1)] private int totalFish = 3;
    [SerializeField]        private GameObject ratPrefab;   // disabled in scene

    // ─── UI ─────────────────────────────────────────────────────────────────
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("HUD")]
    [SerializeField] private GameObject[] fishIcons;
    [SerializeField] private GameObject   ratIcon;

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialCanvas;

    // ─── State ──────────────────────────────────────────────────────────────
    private int  fishCollected;
    private bool ratCaught;
    public  GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        tutorialCanvas?.SetActive(false);
        ChangeState(GameState.Start);
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
            ChangeState(GameState.Paused);
        else if (CurrentState == GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
            ChangeState(GameState.Playing);
    }

    // ─── Public API ─────────────────────────────────────────────────────────
    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        startPanel.SetActive(newState == GameState.Start);
        gamePanel .SetActive(newState == GameState.Playing);
        pausePanel.SetActive(newState == GameState.Paused);
        gameOverPanel.SetActive(newState == GameState.GameOver);

        Time.timeScale = (newState == GameState.Paused || newState == GameState.GameOver) ? 0 : 1;
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

        fishIcons[fishCollected].SetActive(false);  // hide icon
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

    // ─── Internals ──────────────────────────────────────────────────────────
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
