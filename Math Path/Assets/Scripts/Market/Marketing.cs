using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Marketing : MonoBehaviour
{
    // UI Canvas, Script ismi deðiþtir

    // UI
    public Image item_Screen, skins_Screen, color_Screen, something_Screen;
    public GameObject main_Canvas;
    public TextMeshProUGUI speed_Text;


    public void Update()
    {

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            main_Canvas.SetActive(false);
        }
    }
}
