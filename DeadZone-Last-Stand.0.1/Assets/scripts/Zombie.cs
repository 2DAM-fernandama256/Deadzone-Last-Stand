using UnityEngine;

public class Zombie
{
    public string nombre;
    public int vida;
    public float velocidad;
    public int danio;
    public float distanciaAtaque;
    public float tiempoEntreAtaques;
    

    public Zombie(
        string nombre,
        int vida,
        float velocidad,
        int danio,
        float distanciaAtaque,
        float tiempoEntreAtaques
    )
    {
        this.nombre = nombre;
        this.vida = vida;
        this.velocidad = velocidad;
        this.danio = danio;
        this.distanciaAtaque = distanciaAtaque;
        this.tiempoEntreAtaques = tiempoEntreAtaques;
        
    }
}
