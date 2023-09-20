using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private Rigidbody rb;

    public float pushForce = 10f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Karakter hareket kodlarý burada...
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Karþý karakterin rigidbody'sine kuvvet uygulayarak itme iþlemini gerçekleþtir
            Rigidbody otherRb = collision.collider.GetComponent<Rigidbody>();
            Vector3 pushDirection = transform.position - collision.transform.position;
            pushDirection.y = 0; // Dikey yönde itme olmasýný istemiyorsak
            otherRb.AddForce(-pushDirection.normalized * pushForce, ForceMode.Impulse);
        }
    }
}
