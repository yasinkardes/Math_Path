using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GoldCollectible : MonoBehaviour
{
    public int goldValue = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Oyuncu alt�n� toplad���nda bu metot �a�r�l�r
            CollectGold();
        }
    }

    private void CollectGold()
    {
        GameManager.Instance.AddGold(goldValue);
        Destroy(gameObject);
    }
}
