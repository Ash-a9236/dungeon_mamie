using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score;
    public float timeElapsed;
    public bool timerRunning = true;
    public Text scoreText;
    public Text timerText;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateTimer();
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void UpdateTimer()
    {
        if (!timerRunning) return;

        timeElapsed += Time.deltaTime;

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeElapsed / 60f);
            int seconds = Mathf.FloorToInt(timeElapsed % 60f);

            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void ResetTimer()
    {
        timeElapsed = 0f;
        timerRunning = true;
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }
}