using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float myRotation;
    void Start()
    {
        // Rotasyonu de�i�tirmek i�in Transform bile�enini kullanma
        transform.Rotate(Vector3.up * myRotation); // y ekseni etraf�nda d�nd�r
    }
}
