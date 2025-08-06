using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
// tung loai enemy trong 1 dot wave
public class WaveEntry
{
    [Header("moi loai toi da 10")]
    public string enemyTag; // tag dinh danh enemy (dung de lay tu ObjectPooler)
    public int count; // so luong enemy
    public WaypointPath path; // duong di cu the cho loai enemy nay
}

[System.Serializable]
// nhieu loai enemy trong moi dot wave
public class Wave
{
    public WaveEntry[] enemies; // mang cac loai enemy trong 1 wave
    public float spawnInterval = 1f;// khoang thoi gian giua moi spawm
}

[System.Serializable]
class SpawnData
{
    public string tag;// tag dinh danh enemy (dung de lay tu ObjectPooler)
    public WaypointPath path;// WaypointPath cho enemy
    public SpawnData(string tag, WaypointPath path)
    {
        this.tag = tag;
        this.path = path;
    }
}

public class WaveSpawner : MonoBehaviour
{   
    public Wave[] waves;// danh sach cac wave (gan tren Inspector)
    public Transform spawnPoint;
    public float timeBetweenWaves = 4f;// thoi gian giua cac wave
    public int currentWaveIndex = 0;
    public TextMeshProUGUI textWaves;
    public TextMeshProUGUI textCountdown;
    public bool allWavesCompleted = false;
    public GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        textWaves.text = "Wave: " + (currentWaveIndex + 1) + "/" + waves.Length;
        StartCoroutine(SpawnWaves());//Goi coroutine SpawnWaves()
    }

    // ham kiem tra enemy con song
    bool AreAllEnemiesInactive()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");// Tim tag "Enemy"

        foreach (GameObject enemy in allEnemies)
        {
            // neu van con enemy trong scene
            if (enemy.activeInHierarchy)
                return false;
        }

        return true;
    }


    // sinh dot wave 
    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(5f); // cho luc dau game
        textCountdown.text = "Game begin!";

        while (currentWaveIndex < waves.Length)
        {
            textWaves.text = "Wave: " + (currentWaveIndex + 1) + "/" + waves.Length;

            yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            currentWaveIndex++;

            // Cho khi tat ca enemy bi tat
            yield return new WaitUntil(() => AreAllEnemiesInactive());

            // Bat dau dem nguoc khi khong con enemy
            float countdown = timeBetweenWaves;
            while (countdown > 0)
            {
                if (currentWaveIndex == waves.Length)
                {
                    textCountdown.text = "All Waves completed!";
                }
                else
                {
                    textCountdown.text = "Next wave in: " + Mathf.Ceil(countdown).ToString() + "s";
                }
                
                countdown -= Time.deltaTime;
                yield return null;
            }
            textCountdown.text = ""; // Xoa sau khi countdown ket thuc

        }

        allWavesCompleted = true;

        gameManager.WinLevel();
    }


    IEnumerator SpawnWave(Wave wave)
    {
        List<SpawnData> spawnQueue = new List<SpawnData>();

        foreach (var entry in wave.enemies)
        {
            int maxCount = Mathf.Min(entry.count, 10);// moi loai enemy chi duoc toi da 10
            for (int i = 0; i < maxCount; i++)
            {
                spawnQueue.Add(new SpawnData(entry.enemyTag, entry.path));// them tag va path cua enemy vao danh sach cho
            }
        }

        Shuffle(spawnQueue);// xao tron danh sach de khong spawn theo thu tu co dinh

        // Spawn tung enemy trong danh sach da xao tron
        foreach (var spawn in spawnQueue)
        {
            GameObject obj = ObjectPooler.Instance.SpawnFromPool(spawn.tag, spawnPoint.position, Quaternion.identity);
            if (obj != null)
            {
                EnemyMovement move = obj.GetComponent<EnemyMovement>();// Lay EnemyMovement cua object de thiet lap duong di (path) rieng.
                if (move != null)
                {
                    move.path = spawn.path;
                    move.ResetPath(); // reset lai trang thai di chuyen
                }
                
                yield return new WaitForSeconds(wave.spawnInterval);// doi truoc khi spawn tiep
            }
        }
    }

    //tron ngau nhien danh sach
    void Shuffle(List<SpawnData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            var temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}

