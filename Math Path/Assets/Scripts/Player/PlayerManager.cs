using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    // Karakter say�s�, yanl�� cevaptan az ise ne olacak?
    // Do�ru cevaptan ge�tikten hemen sonra yanl�� cevaba ge�erse ne olacak?
    // Ana karakterin i�inde var olmuyor

    public Transform player;
    public List<GameObject> numberOfStickman;
    [SerializeField] public TextMeshProUGUI CounterTxt;
    [SerializeField] private GameObject stickMan;

    [Range(0f, 5f)] [SerializeField] private float startHeight;
    [Range(0f, 4f)] [SerializeField] private float DistanceFactor, Radius;

    void Start()
    {
        Instance = this;
        player = transform;
        numberOfStickman = new List<GameObject>();
    }

    private void FixedUpdate()
    {
        int stickmanCount = numberOfStickman.Count;
        CounterTxt.text = stickmanCount.ToString();

        numberOfStickman.RemoveAll(nesne => nesne == null);
    }

    private void FormatStickMan()
    {
        for (int i = 0; i < numberOfStickman.Count; i++)
        {
            var x = DistanceFactor * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);
            var z = DistanceFactor * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);

            var newPos = new Vector3(x, 0.32f, z);

            numberOfStickman[i].transform.DOLocalMove(newPos, 1f).SetEase(Ease.OutBack);
        }
    }

    void MakeStickMan(int number)
    {
        for (int i = 0; i < number; i++)
        {
            GameObject newStickMan = Instantiate(stickMan, transform.position, Quaternion.identity, transform);

            // �lk stickman sorunlu; ana obje ile i� i�e do�uyor, d�zeltene kadar
            if (i == 0)
            {
                newStickMan.SetActive(false);
            }
            else
            {           
                newStickMan.SetActive(true);
            }

            numberOfStickman.Add(newStickMan); // stickman ekler
        }

        int stickmanCount = numberOfStickman.Count;
        CounterTxt.text = stickmanCount.ToString();

        FormatStickMan();
    }


    public void RemoveStickMan(int number)
    {
        for (int i = 0; i < number; i++)
        {
            if (numberOfStickman.Count > 0)
            {
                numberOfStickman.RemoveAt(numberOfStickman.Count - 1); // Son stickman objesini siler
                Destroy(numberOfStickman[numberOfStickman.Count - 1].gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gate"))
        {
            var QuizManager = other.transform.parent.gameObject.GetComponent<QuizManager>();

            other.transform.GetComponent<BoxCollider>().enabled = false;

            string textValue = other.transform.GetComponentInChildren<TextMeshProUGUI>().text; //Gate �zerindeki de�eri �eker
            int.TryParse(textValue, out int intValue);

            if (intValue == QuizManager.c)
            {
                MakeStickMan(numberOfStickman.Count + QuizManager.c);

                other.enabled = false;
            }

            else
            {
                RemoveStickMan(QuizManager.d);

                other.enabled = false;
            }
        }

        /*
        if (other.CompareTag("Wrong") && numberOfStickman.Count <= QuizManager.d)
        {
            other.transform.GetComponent<BoxCollider>().enabled = false;

            ClearAllStickman();
        }
        */

        if (other.CompareTag("Finish"))
        {
            ArrangeStickManOverlap();
        }

        if (other.CompareTag("Staircase"))
        {
            PlayerController.Instance.forwardSpeed = 0;
            PlayerController.Instance.swerveSpeed = 0;          
        }

        if (other.CompareTag("Villain"))
        {
            PlayerController.Instance.forwardSpeed = 0;
            PlayerController.Instance.swerveSpeed = 0;
        }
    }

    private void ArrangeStickManOverlap()
    {
        Vector3 playerPosition = player.position;
        playerPosition.y = 0.75f;

        // �lk Stickman'dan sonraki Stickman'ler aras�ndaki dikey ve yatay farklar
        float verticalSpacing = 2.3f; // Dikey fark
        float horizontalSpacing = 1f; // Yatay fark
        float duration = 1f; // Animasyon s�resi

        float prevStickmanY = playerPosition.y;
        bool alternate = true; // �iftler halinde d�zenlemek i�in kullan�lan bir bayrak

        foreach (GameObject stickman in numberOfStickman)
        {
            float newY = prevStickmanY + (alternate ? 0 : verticalSpacing); // Dikey pozisyonu hesapl�yoruz
            float newX = horizontalSpacing * (alternate ? 0 : 1); // Yatay pozisyonu hesapl�yoruz
            Vector3 newPos = new Vector3(newX, newY, 0); // Yatay ve dikey pozisyonlar� ayarl�yoruz
            stickman.transform.DOMoveY(newPos.y, duration).SetEase(Ease.OutBack); // Y pozisyonuna animasyonu sa�lar
            stickman.transform.DOLocalMove(newPos, duration).SetEase(Ease.OutBack); // Y pozisyonuna animasyonu sa�lar

            prevStickmanY = newY; // Yeni Stickman'�n ba�lang�� dikey pozisyonunu kaydediyoruz
            alternate = !alternate; // Her iki Stickman sonras�nda bayra�� de�i�tiriyoruz
        }
    }
}