using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI Quiz;
    [SerializeField] public TextMeshProUGUI correctAnswer;
    [SerializeField] public TextMeshProUGUI wrongAnswer;

    public int a, b, c, d;
    public int e, f;

    public int selectedValue, selectedValue2;

    void Start()
    {
        a = Random.Range(1, 10);
        b = Random.Range(1, 10);

        c = a + b; // Doðru
        d = a + b + Random.Range(-4, 4); // Yanlýþ

        int randomValue = Random.Range(0, 2);
        selectedValue = (randomValue == 0) ? c : d;
        selectedValue2 = (randomValue == 1) ? c : d;

        e = selectedValue;
        f = selectedValue2;

        /*
        while (e == f)
        {
            e = selectedValue;
            f = selectedValue2;
        }
        */

        Quiz.text = a + "+" + b + " = ?".ToString();

        correctAnswer.text = e.ToString();
        wrongAnswer.text = f.ToString();
    }
}
