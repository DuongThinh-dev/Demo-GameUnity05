using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;// singleton

    [System.Serializable]
    public class PoolItem
    {
        public string tag;// tag
        public GameObject prefab;// prefab
        public int size = 10;// so luong object
    }

    public List<PoolItem> itemsToPool;// danh sach object

    private Dictionary<string, Queue<GameObject>> poolDict;// tu dien hang doi pool 

    void Awake()
    {
        Instance = this;
        poolDict = new Dictionary<string, Queue<GameObject>>();
        // them object vao pool 
        foreach (var item in itemsToPool)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < item.size; i++)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            poolDict[item.tag] = pool;// luu queue theo tag
        }
    }

    //lay object ra khoi pool
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDict.ContainsKey(tag))// kiem tra tag ton tai khong
        {
            Debug.LogWarning("No pool with tag: " + tag);
            return null;
        }

        Queue<GameObject> pool = poolDict[tag];

        // lay object ra khoi pool
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)// tim object dang khong su dung
            {
                obj.SetActive(true);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                return obj;
            }
        }

        Debug.LogWarning($"Pool '{tag}' is exhausted (max 10 used)");
        return null;
    }
}

