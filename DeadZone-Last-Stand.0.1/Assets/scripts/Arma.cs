using UnityEngine;

public class arma
{
    public string nombre;     
    public int nivel;      
    public float danio;          
    public float distancia;  // Alcance máximo de las balas
    public int cargador;      // Tamaño del cargador 
    public int balasActuales;       // Balas actuales en el cargador
    public int totalBalas;         // Balas totales disponibles 

    // Constructor para inicializar el arma con valores por defecto
    public arma(string nombre, int nivel, float danio,
                     float distancia, int balas, int totalBalas)
    {
        this.nombre = nombre;
        this.nivel = nivel;
        this.danio = danio;
        this.distancia = distancia;
        cargador = balas;
        balasActuales = balas;  // El cargador comienza lleno
        this.totalBalas = totalBalas;        // Munición total inicial
    }

    // Metodo para recargar el arma
    public void Recargar()
    {
        int needed = cargador - balasActuales;
        int available = Mathf.Min(needed, totalBalas);

        balasActuales += available;
        totalBalas -= available;
    }

    // Metodo para disparar (reduce la municion)
    public bool Disparar()
    {
        if (balasActuales > 0)
        {
            balasActuales--;
            return true;  // Disparo exitoso
        }
        return false;     // No hay balas
    }
}
