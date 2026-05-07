using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private bool _addToDontDestroyOnLoad = false;
    // singleton option
    // public static ObjectPoolManager Instance { get; private set; }
    // parent object for the other pools
    private GameObject _emptyHolder;
    private static GameObject _particleSystemsEmpty;
    private static GameObject _gameObjectEmpty;
    private static GameObject _soundFXEmpty;

    private static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
    private static Dictionary<GameObject, GameObject> _cloneToPrefabMap;

    public enum PoolType
    {
        ParticleSystems,
        GameObjects,
        SoundFX
    }

    public static PoolType PoolingType;

    private void Awake()
    {
        // Instance = this;
        _objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        _cloneToPrefabMap = new Dictionary<GameObject, GameObject>();

        SetupEmpties();
    }

    private void SetupEmpties()
    {
        _emptyHolder = new GameObject("Object Pools");

        _particleSystemsEmpty = new GameObject("Particle Effects");
        _particleSystemsEmpty.transform.SetParent(_emptyHolder.transform);

        _gameObjectEmpty = new GameObject("GameObjects");
        _gameObjectEmpty.transform.SetParent(_emptyHolder.transform);

        _soundFXEmpty = new GameObject("Sound FX");
        _soundFXEmpty.transform.SetParent(_emptyHolder.transform);

        if (_addToDontDestroyOnLoad)
        {
            DontDestroyOnLoad(_particleSystemsEmpty.transform.root);
        }
    }

    private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        // create a pool
        ObjectPool<GameObject> pool = new(
            createFunc: () => CreateObject(prefab, pos, rot, poolType),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject
        );

        _objectPools.Add(prefab, pool);
    }

    // transform instead of vector3
    private static void CreatePool(GameObject prefab, Transform pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        ObjectPool<GameObject> pool = new(
            createFunc: () => CreateObject(prefab, pos, rot, poolType),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject
        );

        _objectPools.Add(prefab, pool);
    }

    private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        // ensure that spawns object and guaranteed not wait so on so on
        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab, pos, rot);

        prefab.SetActive(true);

        GameObject parentObject = SetParentObject(poolType);
        obj.transform.SetParent(parentObject.transform);

        return obj;
    }

    private static GameObject CreateObject(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        // ensure that spawns object and guaranteed not wait or so on
        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab, parent);

        obj.transform.SetLocalPositionAndRotation(Vector3.zero, rot);
        obj.transform.localScale = Vector3.zero;

        prefab.SetActive(true);

        return obj;
    }

    private static void OnGetObject(GameObject obj)
    {
        // optional logic
        // obj.SetActive(true);
    }

    private static void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    private static void OnDestroyObject(GameObject obj)
    {
        // if the obj exists, remove it
        if (_cloneToPrefabMap.ContainsKey(obj))
        {
            _cloneToPrefabMap.Remove(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        // new switch tech
        return poolType switch
        {
            PoolType.ParticleSystems => _particleSystemsEmpty,
            PoolType.GameObjects => _gameObjectEmpty,
            PoolType.SoundFX => _soundFXEmpty,
            _ => null,
        };
    }

    // with a specific spawn position  
    private static T SpawnObject<T>(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRot, PoolType poolType = PoolType.GameObjects) where T : UnityEngine.Object
    {
        if (!_objectPools.ContainsKey(objectToSpawn))
        {
            Debug.Log("pool criada");
            CreatePool(objectToSpawn, spawnPos, spawnRot, poolType);
        }

        GameObject obj = _objectPools[objectToSpawn].Get();

        if (obj != null)
        {
            // ensure we get that one (obj) to know which one we return to the pool
            if (!_cloneToPrefabMap.ContainsKey(obj))
            {
                _cloneToPrefabMap.Add(obj, objectToSpawn);
            }

            obj.transform.SetPositionAndRotation(spawnPos, spawnRot);
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }


            if (!obj.TryGetComponent(out T component))
            {
                Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");
                return null;
            }

            return component;
        }

        return null;
    }

    // for component
    /// <summary>
    /// Spawn, or enable, an instance of an prefab already pooled in one of the pools available
    /// </summary>
    /// <typeparam name="T">Class type of the object to be spawned/enabled</typeparam>
    /// <param name="typePrefab">Prefab to be spawned</param>
    /// <param name="spawnPos">The position of the object</param>
    /// <param name="spawnRot">The rotation of the object</param>
    /// <param name="poolType">GameObjects, ParticleSystems or SoundFX</param>
    /// <returns>Returns a reference to the object spawned</returns>
    public static T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRot, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRot, poolType);
    }

    /// <summary>
    /// Spawn, or enable, an instance of an prefab already pooled in one of the pools available
    /// </summary>
    /// <param name="typePrefab">Prefab to be spawned</param>
    /// <param name="spawnPos">The position of the object</param>
    /// <param name="spawnRot">The rotation of the object</param>
    /// <param name="poolType">GameObjects, ParticleSystems or SoundFX</param>
    /// <returns>Returns a reference to the object spawned</returns>
    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRot, PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRot, poolType);
    }

    // with a specific parent
    private static T SpawnObject<T>(GameObject objectToSpawn, Transform parent, Quaternion spawnRot, PoolType poolType = PoolType.GameObjects) where T : UnityEngine.Object
    {
        Debug.Log(_objectPools);

        if (!_objectPools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, parent, spawnRot, poolType);
            Debug.Log("pool foi criada");
        }

        GameObject obj = _objectPools[objectToSpawn].Get();

        if (obj != null)
        {
            // ensure we get that one (obj) to know which one we return to the pool
            if (!_cloneToPrefabMap.ContainsKey(obj))
            {
                _cloneToPrefabMap.Add(obj, objectToSpawn);
            }

            obj.transform.SetParent(parent);
            obj.transform.SetLocalPositionAndRotation(Vector3.zero, spawnRot);
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();

            if (component == null)
            {
                Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");
                return null;
            }

            return component;
        }

        return null;
    }
    
    /// <summary>
    /// Spawn, or enable, an instance of an prefab already pooled in one of the pools available
    /// </summary>
    /// <typeparam name="T">Class type of the object to be spawned/enabled</typeparam>
    /// <param name="typePrefab">Prefab to be spawned</param>
    /// <param name="spawnPos">The position of the object</param>
    /// <param name="spawnRot">The rotation of the object</param>
    /// <param name="poolType">GameObjects, ParticleSystems or SoundFX</param>
    /// <returns>Returns a reference to the object spawned</returns>
    public static T SpawnObject<T>(T typePrefab, Transform parent, Quaternion spawnRot, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, parent, spawnRot, poolType);
    }

    /// <summary>
    /// Spawn, or enable, an instance of an prefab already pooled in one of the pools available
    /// </summary>
    /// <param name="typePrefab">Prefab to be spawned</param>
    /// <param name="spawnPos">The position of the object</param>
    /// <param name="spawnRot">The rotation of the object</param>
    /// <param name="poolType">GameObjects, ParticleSystems or SoundFX</param>
    /// <returns>Returns a reference to the object spawned</returns>
    public static GameObject SpawnObject(GameObject objectToSpawn, Transform parent, Quaternion spawnRot, PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObject<GameObject>(objectToSpawn, parent, spawnRot, poolType);
    }

    /// <summary>
    /// Disable an object in an object pool
    /// </summary>
    /// <param name="obj">object to be disabled</param>
    /// <param name="poolType">pool where the object will be assigned</param>
    public static void ReturnObjectToPool(GameObject obj, PoolType poolType = PoolType.GameObjects)
    {
        string text = string.Join(", ",_cloneToPrefabMap.Select(v=>v.ToString()));
        Debug.Log(obj.name);
        Debug.Log(text);

        if (_cloneToPrefabMap.TryGetValue(obj, out GameObject prefab))
        {
            GameObject parentObject = SetParentObject(poolType);

            if (obj.transform.parent != parentObject.transform)
            {
                obj.transform.SetParent(parentObject.transform);
            }

            if (_objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }
        }
        else
        {
            Debug.LogWarning("Trying to return an object that is not pooled: " + obj.name);
        }
    }
}
