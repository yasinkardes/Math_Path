using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Movement")]
    public float swerveSpeed = 3f; // Karakterin yatay hareket hýzý
    private float Speed; // Karakterin ana hýz deðeri
    public float forwardSpeed; // Karakterin ileri doðru hareket hýzý
    public static float stairSpeed; // Staircase için gerekli
    private float stopSpeed = 0f; // Karakterin durmasý
    public float smoothFactor = 0.5f; // Hareket yumuþatma faktörü

    private float targetOffset = 0f; // Hedef ofset deðeri

    private string playerPrefsKey = "Speed_Upgrade";

    private void Start()
    {
        Instance = this;

        forwardSpeed = PlayerPrefs.GetFloat(playerPrefsKey, 0); // En son deðeri yükler
        forwardSpeed = 15f;

        stairSpeed = forwardSpeed;
    }

    private void Update()
    {
        // Deðeri kaydeder
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
            Speed = stopSpeed; // Sol týk basýlmadýðýnda hýz sýfýrla
        }

        float currentOffset = transform.position.x;
        float newOffset = Mathf.Lerp(currentOffset, targetOffset, smoothFactor);
        transform.position = new Vector3(newOffset, transform.position.y, transform.position.z);
    }
}
