using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ObjectSpawner : MonoBehaviour
{
    //Son Villain yok olunca ana karakterden bir eksilsin

    public GameObject objectToSpawn; // Spawn edilecek obje
    public int numberOfObjects; // Dairesel düzen içinde kaç obje oluþturulacaðý
    public float radius = 5f; // Dairenin yarýçapý
    private int hasSpawned = 0;

    // Radius
    private float heightIncrement = 0.0f; // Yükseklik farký olmayacak þekilde sýfýr olarak ayarlanmýþtýr.
    public float animationDuration = 1f;

    private void Update()
    {
        CalculateSize();

    }

    void SpawnSpiralObjectsWithAnimation()
    {
        float totalHeight = 0.0f; // Yükseklik farkýný saklamak için kullanýlýr.

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfObjects;
            Vector3 spawnPosition = transform.position + new Vector3(Mathf.Cos(angle), totalHeight, Mathf.Sin(angle)) * radius;

            // Objeyi instantiate etmeden önce animasyonu kullanarak belirtilen pozisyona taþýmak.
            GameObject spawnedObject = Instantiate(objectToSpawn, transform.position, Quaternion.identity);

            // Oluþturulan nesneyi ana objenin child'ý olarak ayarlanmasý
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

    // Spawn edilen objeleri * saniye aralýkla yok eden fonksiyon
    IEnumerator DestroySpawnedObjects()
    {
        yield return new WaitForSeconds(0.5f); // 0.5 saniye bekleyin

        // Ana objenin altýndaki tüm çocuklarý alýn ve yok edin
        List<Transform> childTransforms = new List<Transform>(transform.GetComponentsInChildren<Transform>());

        for (int i = 1; i < childTransforms.Count; i++) // Ýlk eleman kendisi olduðu için 1'den baþlat
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

        // Tüm child objeler yok edildikten sonra ana objeyi de yok edin
        Destroy(gameObject);
    }
}
