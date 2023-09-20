using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Movement")]
    public float swerveSpeed = 3f; // Karakterin yatay hareket h�z�
    private float Speed; // Karakterin ana h�z de�eri
    public float forwardSpeed; // Karakterin ileri do�ru hareket h�z�
    public static float stairSpeed; // Staircase i�in gerekli
    private float stopSpeed = 0f; // Karakterin durmas�
    public float smoothFactor = 0.5f; // Hareket yumu�atma fakt�r�

    private float targetOffset = 0f; // Hedef ofset de�eri

    private string playerPrefsKey = "Speed_Upgrade";

    private void Start()
    {
        Instance = this;

        forwardSpeed = PlayerPrefs.GetFloat(playerPrefsKey, 0); // En son de�eri y�kler
        forwardSpeed = 15f;

        stairSpeed = forwardSpeed;
    }

    private void Update()
    {
        // De�eri kaydeder
        PlayerPrefs.SetFloat(playerPrefsKey, forwardSpeed);
        PlayerPrefs.Save();

        float forwardMovement = Speed * Time.deltaTime;
        transform.Translate(Vector3.forward * forwardMovement);

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            targetOffset += Input.GetAxis("Mouse X") * swerveSpeed * Time.deltaTime;
            Speed = forwardSpeed;
        }
        else
        {
            Speed = stopSpeed; // Sol t�k bas�lmad���nda h�z s�f�rla
        }

        float currentOffset = transform.position.x;
        float newOffset = Mathf.Lerp(currentOffset, targetOffset, smoothFactor);
        transform.position = new Vector3(newOffset, transform.position.y, transform.position.z);
    }
}
