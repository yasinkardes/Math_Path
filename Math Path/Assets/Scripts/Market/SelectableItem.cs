using UnityEngine;
using UnityEngine.UI;

public class SelectableItem : MonoBehaviour
{
    public string itemName;

    private MarketManager marketManager;

    private void Start()
    {
        itemName = gameObject.name;

        marketManager = MarketManager.Instance;
    }

    public void OnSelectButtonClicked()
    {
        // Se�ili ��eyi tekrar se�ilebilir yap
        //marketManager.MakeItemSelectableAgain(itemName);

        // Gerekli di�er i�lemler
    }
}
