using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance;

    public GameObject groundPrefab; // Zemin malzemesi, yoldur asl�nda
    public GameObject finishPrefab; // Biti� noktas�
    public GameObject[] obstaclePrefabs; // D��man ve engel prefablar�
    public GameObject desiredObstaclePrefab; // Daha s�k g�rmek istedi�imiz engel prefab�
    private int desiredObstacleSpawnCount; // �stedi�imiz engelin ka� kere ��kt���n� sayar
    public int desiredObstacleSpawnLimit; // �stedi�imiz engelin maksimum ka� kere ��kabilece�ini belirler
    private List<GameObject> nonConsecutiveDesiredObstacles; // Ard���k olarak istedi�imiz engelden ka��n�ld���n� takip eder
    public GameObject customSpawnPrefab; // �zel bir nesne prefab�
    public Vector3 customSpawnPosition; // �zel nesnenin nerede spawn edilece�ini belirler

    public float groundWidth = 15f; // Yolun geni�li�i
    public float groundHeight = 0.2f; // Yolun y�ksekli�i
    public float spawnInterval; // Engel spawn aral���
    public float spawnDistance; // Engel spawn mesafesi
    public int obstacleCount; // Olu�turulacak engel say�s�

    private float spawnTimer = 0f;
    private GameObject currentGround;
    private List<GameObject> spawnedObstacles;
    private bool spawningEnabled = true;

    private bool isObstacleRotationSet = false; // Engellerin Y rotasyonunun d�zenlenip d�zenlenmedi�ini takip eder
    private bool lastSpawnedWasEnemy = false; // Son spawn edilen nesnenin d��man m� oldu�unu belirler
    private bool lastSpawnedWasGate = false; // Son spawn edilen nesnenin kap� (gate) m� oldu�unu belirler
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
        // E�er mevcut yol varsa yok et
        if (currentGround != null)
        {
            Destroy(currentGround);
        }

        // Yeni yol olu�tur
        currentGround = Instantiate(groundPrefab, position, Quaternion.Euler(0, 90f, 0));
        currentGround.transform.localScale = new Vector3(groundWidth, groundHeight, 1f);
    }

    private void SpawnObstacles()
    {
        spawnedObstacles = new List<GameObject>();

        // Engellerin spawn edilece�i x pozisyonlar�n� belirle
        float startX = -spawnDistance / 2f;
        float endX = spawnDistance / 2f;
        float intervalX = (endX - startX) / (obstacleCount - 1);

        int consecutiveEnemiesCount = 0; // Ard���k d��man say�s�n� tutar

        // Belirli say�da engel olu�tur
        for (int i = 0; i < obstacleCount; i++)
        {
            float spawnX = startX + intervalX * i;
            Vector3 spawnPosition = currentGround.transform.position + currentGround.transform.right * spawnX;

            GameObject obstacle;
            do
            {
                obstacle = SpawnObstacleAtPosition(spawnPosition);

                // Spawn edilen engelin d��man olup olmad���n� ve son spawn edilenlerin d��man m� oldu�unu kontrol et
                if (obstacle.CompareTag("Enemy"))
                {
                    consecutiveEnemiesCount++;
                }
                else
                {
                    consecutiveEnemiesCount = 0; // D��man olmayan bir nesne spawn edildiyse s�f�rla
                }

                if (consecutiveEnemiesCount >= 2)
                {
                    // Ard���k olarak 2 d��man varsa farkl� bir engel prefab� se�
                    Destroy(obstacle);
                }
                else
                {
                    // Son spawn edilen nesnenin t�r�ne ba�l� olarak bayraklar� g�ncelle
                    lastSpawnedObstacle = obstacle;
                    break;
                }
            }
            while (true);

            spawnedObstacles.Add(obstacle);
        }

        // Engellerin Y rotasyonu d�zenlendi�ini i�aretleyelim
        isObstacleRotationSet = true;

        spawningEnabled = false; // Engel spawn�n� durdur
    }

    private void SpawnObstacle()
    {
        Vector3 spawnPosition = currentGround.transform.position + currentGround.transform.forward * spawnDistance;

        GameObject obstacle;

        do
        {
            obstacle = SpawnObstacleAtPosition(spawnPosition);

            // Spawn edilen engelin d��man olup olmad���n� ve son spawn edilen nesnenin d��man m� oldu�unu kontrol et
            if (obstacle.CompareTag("Enemy") && lastSpawnedObstacle != null && lastSpawnedObstacle.CompareTag("Enemy"))
            {
                // E�er bu nesne d��man ve son spawn edilen de d��man ise farkl� bir engel prefab� se�
                Destroy(obstacle);
            }
            else
            {
                // Son spawn edilen nesnenin t�r�ne ba�l� olarak bayraklar� g�ncelle
                lastSpawnedObstacle = obstacle;
                break;
            }
        }
        while (true);

        spawnedObstacles.Add(obstacle);

        // Engellerin aras�ndaki bo�lu�u ayarla
        float intervalZ = spawnDistance / (obstacleCount - 1);
        for (int i = 0; i < obstacleCount; i++)
        {
            float spawnZ = intervalZ * i;
            spawnedObstacles[i].transform.position = currentGround.transform.position + currentGround.transform.right * spawnedObstacles[i].transform.position.x
                + currentGround.transform.forward * spawnZ;

            // Engellerin Y rotasyonunu manuel olarak d�zelt
            float desiredYRotation = 45f; // Engellerin istedi�in Y rotasyon a��s�n� buraya yaz (�rn. 45 derece)
            spawnedObstacles[i].transform.rotation = Quaternion.Euler(spawnedObstacles[i].transform.rotation.eulerAngles.x, desiredYRotation, spawnedObstacles[i].transform.rotation.eulerAngles.z);
        }

        spawningEnabled = false; // Engel spawn�n� durdur
    }

    private void SpawnCustomObject()
    {
        Instantiate(customSpawnPrefab, customSpawnPosition, Quaternion.identity);
    }

    public void StartNewLevel()
    {
        Debug.Log("Yeni seviye ba�lad�!"); // Debug mesaj� ekle

        spawningEnabled = true; // Engel spawn�n� yeniden etkinle�tir
        GenerateGround(transform.position, Quaternion.identity);
        SpawnObstacles();
        SpawnCustomObject();

        // Biti� nesnesini olu�tur ve belirtilen konuma yerle�tir
        Vector3 finishPosition = new Vector3(0f, 0.8f, 76f);
        Instantiate(finishPrefab, finishPosition, Quaternion.identity);
    }

    private bool IsObstaclePrefabAllowed(GameObject obstaclePrefab)
    {
        // Engelin etiketini kontrol et, e�er "Enemy" ise ard���k spawn'� engelle
        if (obstaclePrefab.CompareTag("Enemy"))
        {
            if (nonConsecutiveDesiredObstacles.Count > 0)
            {
                return false;
            }
        }

        // Gate prefab�n�n spawn s�n�rlamas�n� yap
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

            // E�er son spawn edilen nesne bir "Gate" ise ard���k spawn'� engelle
            if (spawnedObstacles.Count > 0 && spawnedObstacles[spawnedObstacles.Count - 1].CompareTag("Gate"))
            {
                return false;
            }

            // E�er son spawn edilen nesne bir "Enemy" ise ard���k spawn'� engelle
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

        bool enemyPrefabSpawned = false; // D��man prefab�n�n spawn edilip edilmedi�ini kontrol etmek i�in
        bool gatePrefabSpawned = false;  // Kap� (Gate) prefab�n�n spawn edilip edilmedi�ini kontrol etmek i�in

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
            Debug.LogWarning("�zin verilen engel prefab� bulunamad�. Rastgele bir prefab kullan�l�yor.");
            allowedObstaclePrefabs.AddRange(obstaclePrefabs);
        }

        // E�er bir �nceki nesne bir d��man ise, sadece d��man olmayan prefablardan se�enekleri g�z �n�nde bulundur
        if (lastSpawnedWasEnemy)
        {
            allowedObstaclePrefabs.RemoveAll(prefab => prefab.CompareTag("Enemy"));
        }

        // E�er bir �nceki nesne bir kap� (Gate) ise, sadece kap� olmayan prefablardan se�enekleri g�z �n�nde bulundur
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
            Debug.LogWarning("D��man prefab� hari� izin verilen engel prefab� bulunamad�. Rastgele bir prefab kullan�l�yor.");
            allowedObstaclePrefabs.AddRange(obstaclePrefabs);
        }

        return allowedObstaclePrefabs[Random.Range(0, allowedObstaclePrefabs.Count)];
    }
}
