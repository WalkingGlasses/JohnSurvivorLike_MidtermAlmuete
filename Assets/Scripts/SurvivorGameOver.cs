
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SurvivorGameOver : MonoBehaviour
{
    [Header("Player")]
    public SurvivorPlayerHealth playerHealth;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("Timer")]
    public TextMeshProUGUI timerText;
    public float survivalTime = 480f; // 8 minutes

    [Header("Scene Names")]
    public string gameplaySceneName = "GameScene";
    public string mainMenuSceneName = "MainMenu";

    private float timeRemaining;
    private bool gameFinished;

    private void Awake()
    {
        Time.timeScale = 1f;

        timeRemaining = survivalTime;
        gameFinished = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        UpdateTimerDisplay();
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.PlayerDied += ShowGameOver;
    }

    private void Start()
    {
        if (playerHealth != null && playerHealth.IsDead)
            ShowGameOver();
    }

    private void Update()
    {
        if (gameFinished)
            return;

        // evill count down of doom
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            UpdateTimerDisplay();
            ShowVictory();
            return;
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        int totalSeconds = Mathf.CeilToInt(timeRemaining);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = minutes.ToString("00")
            + ":" + seconds.ToString("00");
    }

    private void ShowGameOver()
    {
        if (gameFinished)
            return;

        gameFinished = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    private void ShowVictory()
    {
        if (gameFinished)
            return;

        gameFinished = true;
        Time.timeScale = 0f;

        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.PlayerDied -= ShowGameOver;
    }
}
