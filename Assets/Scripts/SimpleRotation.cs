using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    public float rotationSpeed = -200f; // Velocidade do giro
    public bool isRotating { get; set; }
    
    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
}