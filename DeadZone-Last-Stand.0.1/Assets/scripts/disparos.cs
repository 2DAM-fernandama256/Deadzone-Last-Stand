using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]

// esta clase sirbe para manejar la bala a que distancia se muebe con que interactua
public class Bala : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float velocidad = 20f;
    [SerializeField] private float distanciaMaxima = 10f;
    [SerializeField] private string[] tagsDestruccion = { "Pared", "Enemigo" };

    private Rigidbody2D rb;
    private Vector2 puntoInicial;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Configuración inicial
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        puntoInicial = transform.position;
    }

    public void Disparar(Vector2 direccion)
    {
        spriteRenderer.enabled = true;
        rb.linearVelocity = direccion.normalized * velocidad;
        puntoInicial = transform.position;
    }
    private void Start()
    {
        arma arma = ArmaManager.ObtenerArma();
        Debug.Log(arma.nombre);
    }

    private void Update()
    {
        // Destruir si supera la distancia máxima
        if (Vector2.Distance(puntoInicial, transform.position) > distanciaMaxima)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        VerificarColision(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        VerificarColision(collision.gameObject);
    }

    private void VerificarColision(GameObject objetoColisionado)
    {
        foreach (string tag in tagsDestruccion)
        {
            if (objetoColisionado.CompareTag(tag))
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}