using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    [Header("World Settings")]
    public int sizeX = 32;
    public int sizeY = 10;
    public int sizeZ = 1;
    public float blockSize = 1f;

    [Header("Prefabs")]
    public GameObject platformBlockPrefab;
    public GameObject trapPrefab;
    public GameObject collectiblePrefab;

    [Header("Generation Settings")]
    public Vector3Int generationStart = new Vector3Int(0, 1, 0);
    public Transform worldParent;

    void Start()
    {
        try
        {
            if (worldParent == null)
                worldParent = new GameObject("World").transform;

            string sceneName = SceneManager.GetActiveScene().name;

            int levelNumber = 1;

            if (sceneName == "Level1Scene") levelNumber = 1;
            else if (sceneName == "Level2Scene") levelNumber = 2;
            else if (sceneName == "Level3Scene") levelNumber = 3;

            Debug.Log($"LevelGenerator: Generating Level {levelNumber} for scene '{sceneName}'");

            GenerateLevel(levelNumber);
        }
        catch (System.Exception e)
        {
            Debug.LogError("LevelGenerator Start error: " + e);
        }
    }

    void GenerateLevel(int level)
    {
        try
        {
            switch (level)
            {
                case 1:
                    GenerateLevel1();
                    break;

                case 2:
                    GenerateLevel2();
                    break;

                case 3:
                    GenerateLevel3();
                    break;

                default:
                    Debug.LogWarning("Unknown level, defaulting to Level 1");
                    GenerateLevel1();
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("GenerateLevel error: " + e);
        }
    }

    void GenerateLevel1()
    {
        int blockCount = 0;

        for (int x = generationStart.x; x < sizeX; x++)
        {
            int height = 0;

            if (x < 12)
                height = 0;

            else
                height = Random.Range(0, 3);

            Vector3 position = new Vector3(
                x * blockSize,
                0,
                0
            );

            Spawn(platformBlockPrefab, position);
            blockCount++;

            TrySpawnTrap(position);
            TrySpawnCollectible(position);
        }

        Debug.Log($"Level 1 generated with {blockCount} blocks");
    }
    void GenerateLevel2()
    {
        int blockCount = 0;

        for (int x = generationStart.x; x < sizeX; x++)
        {
            if (x % 2 == 0)
            {
                Vector3 position = new Vector3(x * blockSize, generationStart.y * blockSize, 0);

                Spawn(platformBlockPrefab, position);
                blockCount++;

                TrySpawnTrap(position);
                TrySpawnCollectible(position);
            }
        }

        Debug.Log($"Level 2 generated with {blockCount} blocks");
    }

    void GenerateLevel3()
    {
        int blockCount = 0;

        for (int x = generationStart.x; x < sizeX; x++)
        {
            if (x % 3 != 0)
            {
                Vector3 position = new Vector3(x * blockSize, generationStart.y * blockSize, 0);

                Spawn(platformBlockPrefab, position);
                blockCount++;

                TrySpawnTrap(position);
                TrySpawnCollectible(position);
            }
        }

        Debug.Log($"Level 3 generated with {blockCount} blocks");
    }

    void TrySpawnTrap(Vector3 basePosition)
    {
        if (trapPrefab == null) return;

        try
        {
            if (Random.value < 0.08f)
            {
                Vector3 pos = basePosition + Vector3.up * blockSize;
                Spawn(trapPrefab, pos);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Trap spawn error: " + e);
        }
    }

    void TrySpawnCollectible(Vector3 basePosition)
    {
        if (collectiblePrefab == null) return;

        try
        {
            if (Random.value < 0.12f)
            {
                Vector3 pos = basePosition + Vector3.up * (blockSize * 2);
                Spawn(collectiblePrefab, pos);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Collectible spawn error: " + e);
        }
    }

    void Spawn(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;

        try
        {
            GameObject obj = Instantiate(prefab, position, Quaternion.identity, worldParent);
            obj.name = prefab.name + "_" + position.x + "_" + position.y;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Spawn error: " + e);
        }
    }
}   