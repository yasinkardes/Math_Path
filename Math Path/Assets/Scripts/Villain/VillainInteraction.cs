using UnityEngine;

public class VillainInteraction : MonoBehaviour
{
    public Transform villain; // Villain objesi
    public float moveSpeed = 5f; // Player hareket h�z�
    public float stoppingDistance; // Player'�n Villain'a ne kadar yakla�t���n�zda durmas� gerekti�i mesafe

    private bool isMoving = false;
    public bool isEnabled = true;


    private void FixedUpdate()
    {
        villain = GameObject.Find("Villain").transform;

        enabled = false;
    }
    private void Update()
    {
        ToVillain();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Villain")) // Trigger objesinin etiketi
        {
            isMoving = true;
        }
    }

    void ToVillain()
    {
        if (isMoving)
        {
            float distance = Vector3.Distance(transform.position, villain.position);

            if (distance > stoppingDistance)
            {
                Vector3 direction = (villain.position - transform.position).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
            else
            {
                isMoving = false;
            }
        }
    }
}