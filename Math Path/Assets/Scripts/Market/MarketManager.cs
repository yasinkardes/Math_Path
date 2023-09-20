using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MarketManager : MonoBehaviour
{
    // Obje Eklemek ��in : CharacterCustomizationManager, Market Manager'e objeyi ekle + Objenin koduna objenin ad�n� ekle
    //Default objenin rengi de�i�miyor

    public static MarketManager Instance;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI valueText;
    public List<MarketItem> marketItems;

    public Color[] availableColors; // Kullan�labilir renklerin listesi
    private GameObject lastSelectedHatObj;
    private Renderer lastSelectedHatRenderer;

    public int totalGold;
    private string playerPrefsKey = "TotalGold";
    private string purchasedItemsKey = "PurchasedItems";

    public List<string> purchasedItems;
    private List<GameObject> inactiveObjects = new List<GameObject>();

    private Dictionary<string, bool> selectableItems = new Dictionary<string, bool>();

    int selectedChildIndex;

    public void Start()
    {
        Instance = this;
        totalGold = PlayerPrefs.GetInt(playerPrefsKey, 0);

        LoadOwnedItems();
        LoadCustomizationData();
        UpdateGoldText();
        LoadInactiveObjects();
        LoadActiveObjects();

        // Sadece "Red" ad�n� purchasedItems listesine eklemek
        //purchasedItems.Add("Red");

        foreach (MarketItem item in marketItems)
        {
            item.isSelectable = true; // T�m ��eleri ba�lang��ta se�ilebilir yap
        }
    }

    private void LoadOwnedItems()
    {
        purchasedItems = new List<string>(PlayerPrefs.GetString(purchasedItemsKey, "").Split(','));
    }

    private void SaveOwnedItems()
    {
        PlayerPrefs.SetString(purchasedItemsKey, string.Join(",", purchasedItems.ToArray()));
        PlayerPrefs.Save();
    }

    public void BuySpeed(string itemName)
    {
        MarketItem item = marketItems.Find(i => i.itemName == itemName);

        if (item != null)
        {
            if (!purchasedItems.Contains(item.itemName))
            {
                int requiredGold = CalculateRequiredGold(item.price);

                if (totalGold >= requiredGold)
                {
                    totalGold -= requiredGold;
                    //purchasedItems.Add(item.itemName); // Tek seferlik almak i�in
                    PlayerController.Instance.forwardSpeed += CalculateSpeedIncrease();

                    // CalculateRequiredGold fonksiyonunu burada �a��rarak requiredGold miktar�n� g�ncelle
                    requiredGold = CalculateRequiredGold(item.price);

                    // requiredGold miktar�n� marketItems listesindeki ilgili item'a da yans�t
                    MarketItem speedItem = marketItems.Find(i => i.itemName == itemName);
                    if (speedItem != null)
                    {
                        speedItem.price = requiredGold;
                    }

                    PlayerPrefs.SetInt(playerPrefsKey, totalGold);
                    SaveOwnedItems();
                    UpdateGoldText();
                }
            }
        }
    }

    public int CalculateRequiredGold(int basePrice)
    {
        int purchasedCount = purchasedItems.Count;
        int requiredGold = basePrice + (purchasedCount * 5);
        return requiredGold;
    }

    private float CalculateSpeedIncrease()
    {
        int purchasedCount = purchasedItems.Count;
        float speedIncrease = purchasedCount * 3;
        return speedIncrease;
    }
    public void BuyItem(int itemIndex)
    {
        if (itemIndex >= 0 && itemIndex < marketItems.Count)
        {
            MarketItem item = marketItems[itemIndex];
            if (totalGold >= item.price && item.isSelectable)
            {
                // ��eyi sat�n al ve listede sakla
                totalGold -= item.price;
                purchasedItems.Add(item.itemName);

                item.isSelectable = true; // ��eyi se�ilemez yap

                PlayerPrefs.SetInt(playerPrefsKey, totalGold);
                SaveOwnedItems();
                UpdateGoldText();
            }
            else if (!item.isSelectable)
            {
                valueText.text = "Sat�n al�nd�: " + item.itemName;
            }
            else
            {
                valueText.text = "Yetersiz alt�n!";
            }
        }
    }
    public void BuyHat(string hatName)
    {
        GameObject hatObj = CharacterCustomizationManager.Instance.hats.Find(hat => hat.name == hatName);

        if (hatObj != null)
        {
            MarketItem item = marketItems.Find(i => i.itemName == hatName);

            if (item != null)
            {
                if (totalGold >= item.price && item.isSelectable)
                {
                    totalGold -= item.price;
                    purchasedItems.Add(item.itemName);

                    item.isSelectable = true; // ��eyi tekrar se�ilebilir yap

                    PlayerPrefs.SetInt(playerPrefsKey, totalGold);
                    SaveOwnedItems();
                    UpdateGoldText();

                    // En son se�ilen Hat objesini atama / BuyColor i�in
                    lastSelectedHatObj = hatObj;

                    // Hat objesinin Renderer componentini al�n
                    lastSelectedHatRenderer = lastSelectedHatObj.GetComponent<Renderer>();


                    foreach (GameObject hat in CharacterCustomizationManager.Instance.hats)
                    {
                        hat.SetActive(false);
                    }

                    hatObj.SetActive(true);
                    CharacterCustomizationManager.Instance.SaveSelectedHat(hatObj.transform.GetSiblingIndex());
                }
                else if (purchasedItems.Contains(item.itemName))
                {
                    //valueText.text = "Sat�n al�nd�: " + item.itemName;
                }
                else
                {
                    //valueText.text = "Yetersiz alt�n!";
                }
            }
        }
    }
    public void BuyCustomization(string custName)
    {
        GameObject custObj = CharacterCustomizationManager.Instance.customization.Find(cust => cust.name == custName);

        if (custObj != null)
        {
            MarketItem item = marketItems.Find(i => i.itemName == custName);

            if (item != null)
            {
                if (totalGold >= item.price && item.isSelectable)
                {
                    totalGold -= item.price;
                    purchasedItems.Add(item.itemName);


                    item.isSelectable = true; // ��eyi tekrar se�ilebilir yap

                    PlayerPrefs.SetInt(playerPrefsKey, totalGold);
                    SaveOwnedItems();
                    UpdateGoldText();

                    foreach (GameObject cust in CharacterCustomizationManager.Instance.customization)
                    {
                        cust.SetActive(false);
                    }

                    custObj.SetActive(true);
                    CharacterCustomizationManager.Instance.SaveSelectedCust(custObj.transform.GetSiblingIndex());
                }
                else if (purchasedItems.Contains(item.itemName))
                {
                    //valueText.text = "Sat�n al�nd�: " + item.itemName;
                }
                else
                {
                    // valueText.text = "Yetersiz alt�n!";
                }
            }
        }
    }
    public void BuyColor()
    {
        if (lastSelectedHatRenderer != null)
        {
            // Rastgele bir renk se�
            Color randomColor = availableColors[Random.Range(0, availableColors.Length)];

            // Objeyi yeni renge boyama
            lastSelectedHatRenderer.material.color = randomColor;

            // Bu k�sm� ekleyerek default objenin rengini de�i�tirebilirsiniz
            if (lastSelectedHatObj != null)
            {
                Renderer defaultObjRenderer = lastSelectedHatObj.GetComponent<Renderer>();
                if (defaultObjRenderer != null)
                {
                    defaultObjRenderer.material.color = randomColor;
                }
            }
        }
    }

    public void MakeItemSelectableAgain(string itemName)
    {
        MarketItem item = marketItems.Find(i => i.itemName == itemName);
        if (item != null)
        {
            item.isSelectable = true;
        }
    }

    public void LoadCustomizationData()
    {
        string jsonData = System.IO.File.ReadAllText(Application.persistentDataPath + "/customizationData.json");
        if (!string.IsNullOrEmpty(jsonData))
        {
            CharacterCustomizationData data = JsonUtility.FromJson<CharacterCustomizationData>(jsonData);
            selectedChildIndex = data.selectedHatIndex;
        }
    }

    public void SaveInactiveObjects()
    {
        inactiveObjects.Clear();
        foreach (GameObject hatObj in CharacterCustomizationManager.Instance.hats)
        {
            if (!hatObj.activeSelf)
            {
                inactiveObjects.Add(hatObj);
            }
        }

        List<string> inactiveObjectNames = new List<string>();
        foreach (GameObject obj in inactiveObjects)
        {
            inactiveObjectNames.Add(obj.name);
        }
        string jsonData = JsonUtility.ToJson(new InactiveObjectsData { inactiveObjectNames = inactiveObjectNames.ToArray() });
        PlayerPrefs.SetString("InactiveObjects", jsonData);
        PlayerPrefs.Save();
    }

    public void LoadInactiveObjects()
    {
        string jsonData = PlayerPrefs.GetString("InactiveObjects", "");
        if (!string.IsNullOrEmpty(jsonData))
        {
            InactiveObjectsData data = JsonUtility.FromJson<InactiveObjectsData>(jsonData);
            foreach (string objectName in data.inactiveObjectNames)
            {
                GameObject obj = GameObject.Find(objectName);
                if (obj != null)
                {
                    obj.SetActive(false);
                    inactiveObjects.Add(obj);
                }
            }
        }
    }

    public void SaveActiveObjects(string activeObjectName)
    {
        PlayerPrefs.SetString("ActiveObjects", activeObjectName);
        PlayerPrefs.Save();
    }

    public void LoadActiveObjects()
    {
        string activeObjectName = PlayerPrefs.GetString("ActiveObjects", "");
        if (!string.IsNullOrEmpty(activeObjectName))
        {
            GameObject obj = GameObject.Find(activeObjectName);
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    public bool IsItemOwned(string itemName)
    {
        return purchasedItems.Contains(itemName);
    }

    public void UpdateGoldText()
    {
        goldText.text = "Alt�n: " + totalGold.ToString();
    }

    [System.Serializable]
    public class InactiveObjectsData
    {
        public string[] inactiveObjectNames;
    }
}
