using UnityEngine;
using UnityEngine.UI;

public class VidaJugador : MonoBehaviour
{
    public int vidaMaxima = 100;
    private int vidaActual;

    public Slider barraVida; // Opcional

    void Start()
    {
        vidaActual = vidaMaxima;
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
            barraVida.value = (float)vidaActual / vidaMaxima;
        }
    }

    void Morir()
    {
        Debug.Log("¡Jugador muerto!");
        // Lógica de fin de juego
    }
}
