using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ObjectSpawner : MonoBehaviour
{
    //Son Villain yok olunca ana karakterden bir eksilsin

    public GameObject objectToSpawn; // Spawn edilecek obje
    public int numberOfObjects; // Dairesel d�zen i�inde ka� obje olu�turulaca��
    public float radius = 5f; // Dairenin yar��ap�
    private int hasSpawned = 0;

    // Radius
    private float heightIncrement = 0.0f; // Y�kseklik fark� olmayacak �ekilde s�f�r olarak ayarlanm��t�r.
    public float animationDuration = 1f;

    private void Update()
    {
        CalculateSize();

    }

    void SpawnSpiralObjectsWithAnimation()
    {
        float totalHeight = 0.0f; // Y�kseklik fark�n� saklamak i�in kullan�l�r.

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfObjects;
            Vector3 spawnPosition = transform.position + new Vector3(Mathf.Cos(angle), totalHeight, Mathf.Sin(angle)) * radius;

            // Objeyi instantiate etmeden �nce animasyonu kullanarak belirtilen pozisyona ta��mak.
            GameObject spawnedObject = Instantiate(objectToSpawn, transform.position, Quaternion.identity);

            // Olu�turulan nesneyi ana objenin child'� olarak ayarlanmas�
            spawnedObject.transform.SetParent(transform);

            spawnedObject.transform.DOMove(spawnPosition, animationDuration)
                .OnComplete(() => StartCoroutine(DestroySpawnedObjects()));

            totalHeight += heightIncrement;
        }
    }


    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (hasSpawned == 0)
            {
                SpawnSpiralObjectsWithAnimation();
                hasSpawned = 1;
            }
        }
    }

    void CalculateSize()
    {
        int totalCount = PlayerManager.Instance.numberOfStickman.Count - 3;
        numberOfObjects = Random.Range(1, totalCount);
    }

    // Spawn edilen objeleri * saniye aral�kla yok eden fonksiyon
    IEnumerator DestroySpawnedObjects()
    {
        yield return new WaitForSeconds(0.5f); // 0.5 saniye bekleyin

        // Ana objenin alt�ndaki t�m �ocuklar� al�n ve yok edin
        List<Transform> childTransforms = new List<Transform>(transform.GetComponentsInChildren<Transform>());

        for (int i = 1; i < childTransforms.Count; i++) // �lk eleman kendisi oldu�u i�in 1'den ba�lat
        {
            if (i >= PlayerManager.Instance.numberOfStickman.Count)
            {
                Debug.Log("La noluyo");
                break;
            }

            Destroy(PlayerManager.Instance.numberOfStickman[i].gameObject);
            Destroy(childTransforms[i].gameObject);

            yield return new WaitForSeconds(0.3f); // 0.3 saniye bekle ve bir sonraki objeyi yok et
        }

        // T�m child objeler yok edildikten sonra ana objeyi de yok edin
        Destroy(gameObject);
    }
}
