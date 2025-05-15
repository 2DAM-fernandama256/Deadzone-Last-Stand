using UnityEngine;

public class arma
{
    public string nombre;      // Nombre del arma (Pistola, Escopeta, etc.)
    public int nivel;      // Nivel actual de mejora (1-4)
    public float danio;          // Da�o base del arma
    public float distancia;  // Alcance m�ximo de las balas
    public int cargador;      // Tama�o del cargador (balas por carga)
    public int balasActuales;       // Balas actuales en el cargador
    public int totalBalas;         // Balas totales disponibles (en reserva)

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
        this.totalBalas = totalBalas;        // Munici�n total inicial
    }

    // M�todo para recargar el arma
    public void Recargar()
    {
        int needed = cargador - balasActuales;
        int available = Mathf.Min(needed, totalBalas);

        balasActuales += available;
        totalBalas -= available;
    }

    // M�todo para disparar (reduce la munici�n)
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
