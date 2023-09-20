using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCameraBase[] cameras; // Kameralarýn listesi
    public string[] targets;

    public Transform followTarget;

    private int currentCameraIndex = 0;

    private void Start()
    {
        //followTarget = LevelGenerator.Instance.customSpawnPrefab.transform;
        followTarget = GameObject.Find(targets[0]).transform;
        cameras[1].Follow = followTarget;

        // Ýlk kamerayý etkinleþtir
        SwitchCamera(currentCameraIndex);
    }

    private void Update()
    {
        // Örnek bir tetikleyici kullanarak kamera geçiþini saðlayabilirsiniz
        if (Input.GetKeyDown(KeyCode.Space)) // Örneðin, "Space" tuþu ile geçiþ yapýn
        {
            // Bir sonraki kameraya geçin (dairesel olarak)
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
            SwitchCamera(currentCameraIndex);
        }
    }

    void SwitchCamera(int index)
    {
        // Tüm kameralarý devre dýþý býrak
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        // Ýstenen kamerayý etkinleþtir
        cameras[index].gameObject.SetActive(true);
    }
}
