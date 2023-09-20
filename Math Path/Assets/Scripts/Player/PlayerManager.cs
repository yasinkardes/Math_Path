using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    // Karakter sayýsý, yanlýþ cevaptan az ise ne olacak?
    // Doðru cevaptan geçtikten hemen sonra yanlýþ cevaba geçerse ne olacak?
    // Ana karakterin içinde var olmuyor

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

            // Ýlk stickman sorunlu; ana obje ile iç içe doðuyor, düzeltene kadar
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

            string textValue = other.transform.GetComponentInChildren<TextMeshProUGUI>().text; //Gate üzerindeki deðeri çeker
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

        // Ýlk Stickman'dan sonraki Stickman'ler arasýndaki dikey ve yatay farklar
        float verticalSpacing = 2.3f; // Dikey fark
        float horizontalSpacing = 1f; // Yatay fark
        float duration = 1f; // Animasyon süresi

        float prevStickmanY = playerPosition.y;
        bool alternate = true; // Çiftler halinde düzenlemek için kullanýlan bir bayrak

        foreach (GameObject stickman in numberOfStickman)
        {
            float newY = prevStickmanY + (alternate ? 0 : verticalSpacing); // Dikey pozisyonu hesaplýyoruz
            float newX = horizontalSpacing * (alternate ? 0 : 1); // Yatay pozisyonu hesaplýyoruz
            Vector3 newPos = new Vector3(newX, newY, 0); // Yatay ve dikey pozisyonlarý ayarlýyoruz
            stickman.transform.DOMoveY(newPos.y, duration).SetEase(Ease.OutBack); // Y pozisyonuna animasyonu saðlar
            stickman.transform.DOLocalMove(newPos, duration).SetEase(Ease.OutBack); // Y pozisyonuna animasyonu saðlar

            prevStickmanY = newY; // Yeni Stickman'ýn baþlangýç dikey pozisyonunu kaydediyoruz
            alternate = !alternate; // Her iki Stickman sonrasýnda bayraðý deðiþtiriyoruz
        }
    }
}