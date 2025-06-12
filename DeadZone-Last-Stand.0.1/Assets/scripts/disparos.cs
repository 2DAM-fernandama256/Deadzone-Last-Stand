using UnityEngine;

public class Bala : MonoBehaviour
{
    private float danio;
    private float distanciaMaxima;
    private float velocidad;
    private Vector2 puntoInicial;
    private Rigidbody2D rb;
    private bool esPrefab = true;// sie es prefab no se puede eliminar ya que si no daria un error

    // Tags para diferenciar las paredes y enemigos
    private string[] tagsDestruccion = { "paredes", "enemigos" };

    //Coge los datos del arma seleccionada
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

        // Si tiene el tag "Zombie" hacemos daño
        if (objetoColisionado.CompareTag("enemigos"))
        {
            ZombieIA zombie = objetoColisionado.GetComponent<ZombieIA>();
            if (zombie != null)
            {
                zombie.RecibirDanio(danio);
            }

            DestruirBala();
            return;
        }
        // verifica si tiene el tag "paredes" o "enemigos" para destruir la bala
        foreach (string tag in tagsDestruccion)
        {
            if (string.IsNullOrEmpty(tag)) continue;

            try
            {
                if (objetoColisionado.CompareTag(tag))
                {
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