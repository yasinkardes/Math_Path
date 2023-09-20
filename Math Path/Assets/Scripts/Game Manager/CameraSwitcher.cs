using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCameraBase[] cameras; // Kameralar�n listesi
    public string[] targets;

    public Transform followTarget;

    private int currentCameraIndex = 0;

    private void Start()
    {
        //followTarget = LevelGenerator.Instance.customSpawnPrefab.transform;
        followTarget = GameObject.Find(targets[0]).transform;
        cameras[1].Follow = followTarget;

        // �lk kameray� etkinle�tir
        SwitchCamera(currentCameraIndex);
    }

    private void Update()
    {
        // �rnek bir tetikleyici kullanarak kamera ge�i�ini sa�layabilirsiniz
        if (Input.GetKeyDown(KeyCode.Space)) // �rne�in, "Space" tu�u ile ge�i� yap�n
        {
            // Bir sonraki kameraya ge�in (dairesel olarak)
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
            SwitchCamera(currentCameraIndex);
        }
    }

    void SwitchCamera(int index)
    {
        // T�m kameralar� devre d��� b�rak
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        // �stenen kameray� etkinle�tir
        cameras[index].gameObject.SetActive(true);
    }
}
