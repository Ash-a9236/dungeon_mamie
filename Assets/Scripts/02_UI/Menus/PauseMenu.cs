using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    public GameObject pausePanel;
    private bool isPaused = false;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume() {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause() {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void QuitGame() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}