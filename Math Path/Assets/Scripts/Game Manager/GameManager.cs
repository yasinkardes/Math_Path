using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI goldText; // UI �zerinde alt�n say�s�n� g�steren metin

    private int totalGold; // Toplam toplanan alt�n say�s�
    private string playerPrefsKey = "TotalGold";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Ba�lang��ta alt�n say�s�n� y�kleyelim veya s�f�rlayal�m
        totalGold = PlayerPrefs.GetInt(playerPrefsKey, 0);
        UpdateGoldText();
    }

    public void AddGold(int amount)
    {
        // Toplanan alt�nlar� sakla ve UI �zerinde g�ster
        totalGold += amount;
        PlayerPrefs.SetInt(playerPrefsKey, totalGold);
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        // UI metnini g�ncelle
        goldText.text = "Alt�n: " + totalGold;
    }

    private void OnApplicationQuit()
    {
        // Oyun kapat�ld���nda alt�n say�s�n� PlayerPrefs'e kaydedelim
        PlayerPrefs.Save();
    }
}
