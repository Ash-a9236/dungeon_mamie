using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectController : MonoBehaviour
{
    [Header("Buttons")]
    public Button levelButton1;
    public Button levelButton2;
    public Button levelButton3;
    public Button backButton;

    [Header("Menu Panel")]
    public GameObject LevelSelectPanel;

    private void Start()
    {
        if (levelButton1 != null) levelButton1.onClick.AddListener(OnLevelButton1Pressed);
        if (levelButton2 != null) levelButton2.onClick.AddListener(OnLevelButton2Pressed);
        if (levelButton3 != null) levelButton3.onClick.AddListener(OnLevelButton3Pressed);
        if (backButton != null) backButton.onClick.AddListener(OnBackButtonPressed);
    }

    public void ShowLevelSelect()
    {
        LevelSelectPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HideLevelSelect()
    {
        LevelSelectPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnLevelButton1Pressed()
    {
        HideLevelSelect();
        SceneManager.LoadScene("Level1Scene");
    }
    private void OnLevelButton2Pressed()
    {
        HideLevelSelect();
        SceneManager.LoadScene("Level2Scene");
    }
    private void OnLevelButton3Pressed()
    {
        HideLevelSelect();
        SceneManager.LoadScene("Level3Scene");
    }

    private void OnBackButtonPressed()
    {
        HideLevelSelect();
        SceneManager.LoadScene("MainMenuScene");
    }
}