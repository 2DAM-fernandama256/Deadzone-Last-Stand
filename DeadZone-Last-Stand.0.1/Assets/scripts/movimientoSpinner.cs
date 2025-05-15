using UnityEngine;

public class movimientoSpinner : MonoBehaviour
{
    [Header("Configuraci�n B�sica")]
    [Tooltip("Velocidad de rotaci�n en grados/segundo")]
    public float rotationSpeed = 200f;

    [Tooltip("Si est� activo, el spinner cambia de direcci�n ocasionalmente")]
    public bool randomDirectionChanges = false;

    private float currentSpeed;
    private float direction = 1f;

    void Update()
    {
        // Rotaci�n constante
        transform.Rotate(0, 0, -rotationSpeed * direction * Time.deltaTime);

        // Cambio de direcci�n aleatorio (opcional)
        if (randomDirectionChanges && Random.Range(0, 1000) < 2)
        {
            direction *= -1f;
        }
    }

    // M�todo p�blico para cambiar direcci�n manualmente
    public void ChangeDirection()
    {
        direction *= -1f;
    }

    // M�todo para ajustar velocidad
    public void SetSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
    }
}