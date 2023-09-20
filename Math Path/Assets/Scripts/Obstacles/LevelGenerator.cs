using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance;

    public GameObject groundPrefab; // Zemin malzemesi, yoldur aslýnda
    public GameObject finishPrefab; // Bitiþ noktasý
    public GameObject[] obstaclePrefabs; // Düþman ve engel prefablarý
    public GameObject desiredObstaclePrefab; // Daha sýk görmek istediðimiz engel prefabý
    private int desiredObstacleSpawnCount; // Ýstediðimiz engelin kaç kere çýktýðýný sayar
    public int desiredObstacleSpawnLimit; // Ýstediðimiz engelin maksimum kaç kere çýkabileceðini belirler
    private List<GameObject> nonConsecutiveDesiredObstacles; // Ardýþýk olarak istediðimiz engelden kaçýnýldýðýný takip eder
    public GameObject customSpawnPrefab; // Özel bir nesne prefabý
    public Vector3 customSpawnPosition; // Özel nesnenin nerede spawn edileceðini belirler

    public float groundWidth = 15f; // Yolun geniþliði
    public float groundHeight = 0.2f; // Yolun yüksekliði
    public float spawnInterval; // Engel spawn aralýðý
    public float spawnDistance; // Engel spawn mesafesi
    public int obstacleCount; // Oluþturulacak engel sayýsý

    private float spawnTimer = 0f;
    private GameObject currentGround;
    private List<GameObject> spawnedObstacles;
    private bool spawningEnabled = true;

    private bool isObstacleRotationSet = false; // Engellerin Y rotasyonunun düzenlenip düzenlenmediðini takip eder
    private bool lastSpawnedWasEnemy = false; // Son spawn edilen nesnenin düþman mý olduðunu belirler
    private bool lastSpawnedWasGate = false; // Son spawn edilen nesnenin kapý (gate) mý olduðunu belirler
    private GameObject lastSpawnedObstacle; // Son spawn edilen nesneyi saklar

    private void Start()
    {
        Instance = this;

        nonConsecutiveDesiredObstacles = new List<GameObject>();

        StartNewLevel();
    }

    private void Update()
    {
        if (!spawningEnabled)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnedObstacles.Count < obstacleCount && spawnTimer >= spawnInterval)
        {
            SpawnObstacle();
            spawnTimer = 0f;
        }
    }

    private void GenerateGround(Vector3 position, Quaternion rotation)
    {
        // Eðer mevcut yol varsa yok et
        if (currentGround != null)
        {
            Destroy(currentGround);
        }

        // Yeni yol oluþtur
        currentGround = Instantiate(groundPrefab, position, Quaternion.Euler(0, 90f, 0));
        currentGround.transform.localScale = new Vector3(groundWidth, groundHeight, 1f);
    }

    private void SpawnObstacles()
    {
        spawnedObstacles = new List<GameObject>();

        // Engellerin spawn edileceði x pozisyonlarýný belirle
        float startX = -spawnDistance / 2f;
        float endX = spawnDistance / 2f;
        float intervalX = (endX - startX) / (obstacleCount - 1);

        int consecutiveEnemiesCount = 0; // Ardýþýk düþman sayýsýný tutar

        // Belirli sayýda engel oluþtur
        for (int i = 0; i < obstacleCount; i++)
        {
            float spawnX = startX + intervalX * i;
            Vector3 spawnPosition = currentGround.transform.position + currentGround.transform.right * spawnX;

            GameObject obstacle;
            do
            {
                obstacle = SpawnObstacleAtPosition(spawnPosition);

                // Spawn edilen engelin düþman olup olmadýðýný ve son spawn edilenlerin düþman mý olduðunu kontrol et
                if (obstacle.CompareTag("Enemy"))
                {
                    consecutiveEnemiesCount++;
                }
                else
                {
                    consecutiveEnemiesCount = 0; // Düþman olmayan bir nesne spawn edildiyse sýfýrla
                }

                if (consecutiveEnemiesCount >= 2)
                {
                    // Ardýþýk olarak 2 düþman varsa farklý bir engel prefabý seç
                    Destroy(obstacle);
                }
                else
                {
                    // Son spawn edilen nesnenin türüne baðlý olarak bayraklarý güncelle
                    lastSpawnedObstacle = obstacle;
                    break;
                }
            }
            while (true);

            spawnedObstacles.Add(obstacle);
        }

        // Engellerin Y rotasyonu düzenlendiðini iþaretleyelim
        isObstacleRotationSet = true;

        spawningEnabled = false; // Engel spawnýný durdur
    }

    private void SpawnObstacle()
    {
        Vector3 spawnPosition = currentGround.transform.position + currentGround.transform.forward * spawnDistance;

        GameObject obstacle;

        do
        {
            obstacle = SpawnObstacleAtPosition(spawnPosition);

            // Spawn edilen engelin düþman olup olmadýðýný ve son spawn edilen nesnenin düþman mý olduðunu kontrol et
            if (obstacle.CompareTag("Enemy") && lastSpawnedObstacle != null && lastSpawnedObstacle.CompareTag("Enemy"))
            {
                // Eðer bu nesne düþman ve son spawn edilen de düþman ise farklý bir engel prefabý seç
                Destroy(obstacle);
            }
            else
            {
                // Son spawn edilen nesnenin türüne baðlý olarak bayraklarý güncelle
                lastSpawnedObstacle = obstacle;
                break;
            }
        }
        while (true);

        spawnedObstacles.Add(obstacle);

        // Engellerin arasýndaki boþluðu ayarla
        float intervalZ = spawnDistance / (obstacleCount - 1);
        for (int i = 0; i < obstacleCount; i++)
        {
            float spawnZ = intervalZ * i;
            spawnedObstacles[i].transform.position = currentGround.transform.position + currentGround.transform.right * spawnedObstacles[i].transform.position.x
                + currentGround.transform.forward * spawnZ;

            // Engellerin Y rotasyonunu manuel olarak düzelt
            float desiredYRotation = 45f; // Engellerin istediðin Y rotasyon açýsýný buraya yaz (örn. 45 derece)
            spawnedObstacles[i].transform.rotation = Quaternion.Euler(spawnedObstacles[i].transform.rotation.eulerAngles.x, desiredYRotation, spawnedObstacles[i].transform.rotation.eulerAngles.z);
        }

        spawningEnabled = false; // Engel spawnýný durdur
    }

    private void SpawnCustomObject()
    {
        Instantiate(customSpawnPrefab, customSpawnPosition, Quaternion.identity);
    }

    public void StartNewLevel()
    {
        Debug.Log("Yeni seviye baþladý!"); // Debug mesajý ekle

        spawningEnabled = true; // Engel spawnýný yeniden etkinleþtir
        GenerateGround(transform.position, Quaternion.identity);
        SpawnObstacles();
        SpawnCustomObject();

        // Bitiþ nesnesini oluþtur ve belirtilen konuma yerleþtir
        Vector3 finishPosition = new Vector3(0f, 0.8f, 76f);
        Instantiate(finishPrefab, finishPosition, Quaternion.identity);
    }

    private bool IsObstaclePrefabAllowed(GameObject obstaclePrefab)
    {
        // Engelin etiketini kontrol et, eðer "Enemy" ise ardýþýk spawn'ý engelle
        if (obstaclePrefab.CompareTag("Enemy"))
        {
            if (nonConsecutiveDesiredObstacles.Count > 0)
            {
                return false;
            }
        }

        // Gate prefabýnýn spawn sýnýrlamasýný yap
        if (obstaclePrefab.CompareTag("Gate"))
        {
            int gateSpawnCount = 0;

            foreach (var spawnedObstacle in spawnedObstacles)
            {
                if (spawnedObstacle.CompareTag("Gate"))
                {
                    gateSpawnCount++;
                }
            }

            if (gateSpawnCount >= 2)
            {
                return false;
            }

            // Eðer son spawn edilen nesne bir "Gate" ise ardýþýk spawn'ý engelle
            if (spawnedObstacles.Count > 0 && spawnedObstacles[spawnedObstacles.Count - 1].CompareTag("Gate"))
            {
                return false;
            }

            // Eðer son spawn edilen nesne bir "Enemy" ise ardýþýk spawn'ý engelle
            if (spawnedObstacles.Count > 0 && spawnedObstacles[spawnedObstacles.Count - 1].CompareTag("Enemy"))
            {
                return false;
            }
        }

        return true;
    }

    private GameObject SpawnObstacleAtPosition(Vector3 position)
    {
        List<GameObject> allowedObstaclePrefabs = new List<GameObject>();

        bool enemyPrefabSpawned = false; // Düþman prefabýnýn spawn edilip edilmediðini kontrol etmek için
        bool gatePrefabSpawned = false;  // Kapý (Gate) prefabýnýn spawn edilip edilmediðini kontrol etmek için

        foreach (GameObject prefab in obstaclePrefabs)
        {
            if (IsObstaclePrefabAllowed(prefab))
            {
                allowedObstaclePrefabs.Add(prefab);

                if (prefab.CompareTag("Enemy"))
                {
                    enemyPrefabSpawned = true;
                }
                else if (prefab.CompareTag("Gate"))
                {
                    gatePrefabSpawned = true;
                }
            }
        }

        if (allowedObstaclePrefabs.Count == 0)
        {
            Debug.LogWarning("Ýzin verilen engel prefabý bulunamadý. Rastgele bir prefab kullanýlýyor.");
            allowedObstaclePrefabs.AddRange(obstaclePrefabs);
        }

        // Eðer bir önceki nesne bir düþman ise, sadece düþman olmayan prefablardan seçenekleri göz önünde bulundur
        if (lastSpawnedWasEnemy)
        {
            allowedObstaclePrefabs.RemoveAll(prefab => prefab.CompareTag("Enemy"));
        }

        // Eðer bir önceki nesne bir kapý (Gate) ise, sadece kapý olmayan prefablardan seçenekleri göz önünde bulundur
        if (lastSpawnedWasGate)
        {
            allowedObstaclePrefabs.RemoveAll(prefab => prefab.CompareTag("Gate"));
        }

        GameObject randomPrefab = allowedObstaclePrefabs[Random.Range(0, allowedObstaclePrefabs.Count)];
        return Instantiate(randomPrefab, position, Quaternion.identity);
    }

    private GameObject GetRandomObstaclePrefabExceptEnemy()
    {
        List<GameObject> allowedObstaclePrefabs = new List<GameObject>();

        foreach (GameObject prefab in obstaclePrefabs)
        {
            if (IsObstaclePrefabAllowed(prefab) && !prefab.CompareTag("Enemy"))
            {
                allowedObstaclePrefabs.Add(prefab);
            }
        }

        if (allowedObstaclePrefabs.Count == 0)
        {
            Debug.LogWarning("Düþman prefabý hariç izin verilen engel prefabý bulunamadý. Rastgele bir prefab kullanýlýyor.");
            allowedObstaclePrefabs.AddRange(obstaclePrefabs);
        }

        return allowedObstaclePrefabs[Random.Range(0, allowedObstaclePrefabs.Count)];
    }
}
