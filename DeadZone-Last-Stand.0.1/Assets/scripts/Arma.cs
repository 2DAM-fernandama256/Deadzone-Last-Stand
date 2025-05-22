using UnityEngine;

public class arma
{
    public string nombre;      // Nombre del arma 
    public int nivel;      // Nivel actual de mejora 
    public float danio;          // Daño base del arma
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

    // Método para recargar el arma
    public void Recargar()
    {
        int needed = cargador - balasActuales;
        int available = Mathf.Min(needed, totalBalas);

        balasActuales += available;
        totalBalas -= available;
    }

    // Método para disparar (reduce la munición)
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
