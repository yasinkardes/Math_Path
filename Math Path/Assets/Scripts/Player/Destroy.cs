using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Destroy : MonoBehaviour
{
    public float forwardSpeed;

    private Rigidbody rb;


    private void Start()
    {
        forwardSpeed = 5f;
        rb = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        if (PlayerController.Instance.forwardSpeed == 0)
        {
            //gameObject.transform.parent = null;
            transform.Translate(Vector3.forward * (forwardSpeed * Time.deltaTime));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(this.gameObject);
        }

        if (other.CompareTag("Staircase") || other.CompareTag("Villain"))
        {
            forwardSpeed = 0;
        }
    }
}
