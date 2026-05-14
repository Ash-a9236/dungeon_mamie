using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    public void PlayGame() {
        SceneManager.LoadScene("TileTestScene");
    }

    public void OpenSettings() {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings() {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}