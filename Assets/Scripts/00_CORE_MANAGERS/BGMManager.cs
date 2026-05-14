using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BGMController : MonoBehaviour
{
    public AudioSource audioSource;
    public float normalVolume = 1.0f;
    public float pauseVolume = 0.3f;
    public float fadeDuration = 1.5f;

    void Start()
    {
        audioSource.volume = normalVolume;
    }

    // Logic for lowering volume when the game window is minimized/paused
    void OnApplicationPause(bool pauseStatus)
    {
        audioSource.volume = pauseStatus ? pauseVolume : normalVolume;
    }

    // Call this method instead of SceneManager.LoadScene directly
    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float startVolume = audioSource.volume;

        // Gradually decrease volume
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();

        // Now change the scene
        SceneManager.LoadScene(sceneName);
    }
}
