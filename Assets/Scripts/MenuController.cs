using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;
    public Button pauseButton;
    public Button resumeButton;
    public Button restartButton;
    public GameObject menuPanel;
    public GameObject pauseMenuPanel;
    public bool enablePause = false;

    private bool isPaused = false;

    void Start()
    {
        try
        {
            if (menuPanel != null) menuPanel.SetActive(!enablePause);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

            if (playButton != null) playButton.onClick.AddListener(OnPlayPressed);
            if (quitButton != null) quitButton.onClick.AddListener(OnQuitPressed);
            if (pauseButton != null) pauseButton.onClick.AddListener(OnPausePressed);
            if (resumeButton != null) resumeButton.onClick.AddListener(OnResumePressed);
            if (restartButton != null) restartButton.onClick.AddListener(OnRestartPressed);

            Debug.Log("MenuController initialized | enablePause=" + enablePause);
        }
        catch (System.Exception e)
        {
            Debug.LogError("MenuController Start error: " + e.Message);
        }
    }

    void Update()
    {
        try
        {
            if (enablePause && Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("MenuController Update error: " + e.Message);
        }
    }

    private void TogglePause()
    {
        try
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
        catch (System.Exception e)
        {
            Debug.LogError("TogglePause error: " + e.Message);
        }
    }

    private void PauseGame()
    {
        try
        {
            Time.timeScale = 0f;
            isPaused = true;

            if (GameManager.Instance != null) GameManager.Instance.timerRunning = false;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);

            Debug.Log("Game paused | pauseMenuPanel active=" + (pauseMenuPanel != null ? pauseMenuPanel.activeSelf.ToString() : "null"));
        }
        catch (System.Exception e)
        {
            Debug.LogError("PauseGame error: " + e.Message);
        }
    }

    private void ResumeGame()
    {
        try
        {
            Time.timeScale = 1f;
            isPaused = false;

            if (GameManager.Instance != null) GameManager.Instance.timerRunning = true;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

            Debug.Log("Game resumed | pauseMenuPanel active=" + (pauseMenuPanel != null ? pauseMenuPanel.activeSelf.ToString() : "null"));
        }
        catch (System.Exception e)
        {
            Debug.LogError("ResumeGame error: " + e.Message);
        }
    }

    private void OnPausePressed()
    {
        try { TogglePause(); }
        catch (System.Exception e) { Debug.LogError("OnPausePressed error: " + e.Message); }
    }

    private void OnPlayPressed()
    {
        try
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("LevelSelectScene");
        }
        catch (System.Exception e) { Debug.LogError("OnPlayPressed error: " + e.Message); }
    }

    private void OnQuitPressed()
    {
        try { Application.Quit(); }
        catch (System.Exception e) { Debug.LogError("OnQuitPressed error: " + e.Message); }
    }

    private void OnResumePressed()
    {
        try { ResumeGame(); }
        catch (System.Exception e) { Debug.LogError("OnResumePressed error: " + e.Message); }
    }

    private void OnRestartPressed()
    {
        try
        {
            Time.timeScale = 1f;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetTimer();
                GameManager.Instance.ResetScore();
            }
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.name);
        }
        catch (System.Exception e) { Debug.LogError("OnRestartPressed error: " + e.Message); }
    }
}
