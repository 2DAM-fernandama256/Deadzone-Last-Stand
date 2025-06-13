using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VidaJugador : MonoBehaviour
{
    public int vidaMaxima = 100;
    private int vidaActual;

    public GameObject panelFin;

    public Slider barraVida;

    void Start()
    {
        vidaActual = vidaMaxima;

        if (barraVida != null)
        {
            barraVida.minValue = 0;
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaActual;
            barraVida.interactable = false; // no muebe la barra con raton
            barraVida.wholeNumbers = true;  // no decimales en la barra
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

    public void ActualizarBarra()
    {
        if (barraVida != null)
        {
            barraVida.value = vidaActual;
        }
    }

    public void Morir()
    {
        Debug.Log("¡Jugador muerto!");

        // Guardar récord de kills 
        if (KillsManager.Instance != null)
        {
            KillsManager.Instance.CompruebaGuardaBestKills();
        }

       

        panelFin.SetActive(true);
    }
    public void volver()
    {
        // Cambiar a la escena "MenuInicial"
        SceneManager.LoadScene("MenuInicial");
        Destroy(gameObject);
    }
}
