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
        // Seçili öðeyi tekrar seçilebilir yap
        //marketManager.MakeItemSelectableAgain(itemName);

        // Gerekli diðer iþlemler
    }
}
