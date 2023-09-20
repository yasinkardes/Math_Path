using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizationManager : MonoBehaviour
{
    public static CharacterCustomizationManager Instance;

    public List<GameObject> hats; // Tüm þapka objeleri
    public List<GameObject> customization; // Custom objeleri

    private void Start()
    {
        Instance = this;

        // Þapka objelerini "Hats" nesnesini bulup altýndaki þapka objelerini listeye ekleyin
        hats = new List<GameObject>();
        Transform hatsParent = GameObject.Find("Hats").transform;

        customization = new List<GameObject>();
        Transform custParent = GameObject.Find("Custom").transform;

        foreach (Transform hatTransform in hatsParent)
        {
            GameObject hat = hatTransform.gameObject;
            hat.SetActive(false);
            hats.Add(hat);
        }

        foreach (Transform custTransform in custParent)
        {
            GameObject cust = custTransform.gameObject;
            cust.SetActive(false);
            customization.Add(cust);
        }

        // Seçilen þapka objesini yükle ve aktif hale getir
        int selectedHatIndex = LoadSelectedHat();
        if (selectedHatIndex >= 0 && selectedHatIndex < hats.Count)
        {
            hats[selectedHatIndex].SetActive(true);
        }

        // Seçilen Custom objesini yükle ve aktif hale getir
        int selectedCustIndex = LoadSelectedCust();
        if (selectedCustIndex >= 0 && selectedCustIndex < customization.Count)
        {
            customization[selectedCustIndex].SetActive(true);
        }
    }

    public void SaveSelectedHat(int hatIndex)
    {
        PlayerPrefs.SetInt("SelectedHatIndex", hatIndex);
        PlayerPrefs.Save();
    }

    public int LoadSelectedHat()
    {
        return PlayerPrefs.GetInt("SelectedHatIndex", 0);
    }

    ///
    public void SaveSelectedCust(int custIndex)
    {
        PlayerPrefs.SetInt("SelectedCustIndex", custIndex);
        PlayerPrefs.Save();
    }

    public int LoadSelectedCust()
    {
        return PlayerPrefs.GetInt("SelectedCustIndex", 0);
    }
}
