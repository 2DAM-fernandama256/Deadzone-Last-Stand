using UnityEngine;

public class Bala : MonoBehaviour
{
    private float danio;
    private float distanciaMaxima;
    private float velocidad;
    private Vector2 puntoInicial;
    private Rigidbody2D rb;
    private bool esPrefab = true;

    // Tags por defecto que funcionarán incluso si no están asignados en el Inspector
    private string[] tagsDestruccion = { "paredes", "enemigos" };

    public void Configurar(float danio, float distancia, float velocidad, Vector2 direccion)
    {
        this.danio = danio;
        this.distanciaMaxima = distancia;
        this.velocidad = velocidad;
        this.esPrefab = false;

        puntoInicial = transform.position;

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direccion.normalized * velocidad;
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    private void Update()
    {
        if (!esPrefab && Vector2.Distance(puntoInicial, transform.position) > distanciaMaxima)
        {
            DestruirBala();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (esPrefab) return;
        VerificarColision(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (esPrefab) return;
        VerificarColision(collision.gameObject);
    }

    private void VerificarColision(GameObject objetoColisionado)
    {
        if (objetoColisionado == null) return;

        foreach (string tag in tagsDestruccion)
        {
            if (string.IsNullOrEmpty(tag)) continue;

            try
            {
                if (objetoColisionado.CompareTag(tag))
                {
                    if (tag == "Enemigo")
                    {
                        //var enemigo = objetoColisionado.GetComponent<Enemigo>();
                       // if (enemigo != null)
                        //{
                       //     enemigo.RecibirDanio(danio);
                        //}
                    }
                    DestruirBala();
                    return;
                }
            }
            catch (UnityException)
            {
                Debug.LogWarning($"El tag '{tag}' no está definido en Project Settings");
                continue;
            }
        }
    }

    private void DestruirBala()
    {
        if (!esPrefab)
        {
            Destroy(gameObject);
        }
    }
}