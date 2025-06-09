using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VidaJugador : MonoBehaviour
{
    public int vidaMaxima = 100;
    private int vidaActual;

    public Slider barraVida; // Opcional

    void Start()
    {
        vidaActual = vidaMaxima;

        if (barraVida != null)
        {
            barraVida.minValue = 0;
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaActual;
            barraVida.interactable = false; // Esto asegura que no puedes moverla
            barraVida.wholeNumbers = true;  // Para que no muestre decimales
        }

        ActualizarBarra();
    }

    public void RecibirDanio(int cantidad)
    {
        vidaActual -= cantidad;
        vidaActual = Mathf.Max(0, vidaActual);
        ActualizarBarra();

        Debug.Log("¡El jugador recibió daño! Vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void Curar(int cantidad)
    {
        vidaActual += cantidad;
        vidaActual = Mathf.Min(vidaActual, vidaMaxima);
        ActualizarBarra();

        Debug.Log("¡Jugador curado! Vida actual: " + vidaActual);
    }

    void ActualizarBarra()
    {
        if (barraVida != null)
        {
            barraVida.value = vidaActual;
        }
    }

    void Morir()
    {
        Debug.Log("¡Jugador muerto!");

        // Guardar récord de kills 
        if (KillsManager.Instance != null)
        {
            KillsManager.Instance.CheckAndSaveBestKills();
        }

        Destroy(gameObject);

        // Cambiar a la escena "MenuInicial"
        SceneManager.LoadScene("MenuInicial");
    }
}
