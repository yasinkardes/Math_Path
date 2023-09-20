using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI goldText; // UI üzerinde altýn sayýsýný gösteren metin

    private int totalGold; // Toplam toplanan altýn sayýsý
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
        // Baþlangýçta altýn sayýsýný yükleyelim veya sýfýrlayalým
        totalGold = PlayerPrefs.GetInt(playerPrefsKey, 0);
        UpdateGoldText();
    }

    public void AddGold(int amount)
    {
        // Toplanan altýnlarý sakla ve UI üzerinde göster
        totalGold += amount;
        PlayerPrefs.SetInt(playerPrefsKey, totalGold);
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        // UI metnini güncelle
        goldText.text = "Altýn: " + totalGold;
    }

    private void OnApplicationQuit()
    {
        // Oyun kapatýldýðýnda altýn sayýsýný PlayerPrefs'e kaydedelim
        PlayerPrefs.Save();
    }
}
