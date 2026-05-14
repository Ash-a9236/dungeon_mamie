using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneID
{
    MainMenu,
    TileTest,
    DungeonTest,
    DemoEnd
}

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private readonly Dictionary<SceneID, string> _sceneMap = new Dictionary<SceneID, string>
    {
        { SceneID.MainMenu, "MainMenu" },
        { SceneID.TileTest, "TileTestScene" },
        { SceneID.DungeonTest, "DungeonTestScene" },
        { SceneID.DemoEnd, "DemoEnd" }
    };

    private void Awake()
    {
        // Setup Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// Public method: SceneLoader.Instance.Load(SceneID.TileTest);
    public void Load(SceneID id)
    {
        if (_sceneMap.ContainsKey(id))
        {
            string sceneName = _sceneMap[id];
            Debug.Log($"Loading Scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"SceneID {id} is not mapped in the Dictionary!");
        }
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
