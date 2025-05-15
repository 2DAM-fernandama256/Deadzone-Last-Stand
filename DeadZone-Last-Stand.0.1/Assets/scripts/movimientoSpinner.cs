using UnityEngine;

public class movimientoSpinner : MonoBehaviour
{
    [Header("Configuración Básica")]
    [Tooltip("Velocidad de rotación en grados/segundo")]
    public float rotationSpeed = 200f;

    [Tooltip("Si está activo, el spinner cambia de dirección ocasionalmente")]
    public bool randomDirectionChanges = false;

    private float currentSpeed;
    private float direction = 1f;

    void Update()
    {
        // Rotación constante
        transform.Rotate(0, 0, -rotationSpeed * direction * Time.deltaTime);

        // Cambio de dirección aleatorio (opcional)
        if (randomDirectionChanges && Random.Range(0, 1000) < 2)
        {
            direction *= -1f;
        }
    }

    // Método público para cambiar dirección manualmente
    public void ChangeDirection()
    {
        direction *= -1f;
    }

    // Método para ajustar velocidad
    public void SetSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
    }
}