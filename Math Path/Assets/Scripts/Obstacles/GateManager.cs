using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GateManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI gateNo;
    public int randomNumber;
    public bool multiply;

    void Start()
    {
        if (multiply)
        {
            randomNumber = Random.Range(1, 6);
            gateNo.text = "X" + randomNumber;
        }
        else
        {
            randomNumber = Random.Range(1, 61);

            if (randomNumber % 2 != 0)
                randomNumber += 1;

            gateNo.text = randomNumber.ToString();
        }
    }
}
