using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float myRotation;
    void Start()
    {
        // Rotasyonu deðiþtirmek için Transform bileþenini kullanma
        transform.Rotate(Vector3.up * myRotation); // y ekseni etrafýnda döndür
    }
}
