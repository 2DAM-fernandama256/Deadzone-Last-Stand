using UnityEngine;
using UnityEngine.AI;

public class ZombieIA : MonoBehaviour
{
    [Header("Configuración")]
    public Zombie datos; // ScriptableObject con stats del zombie

    [Header("Valores por Defecto")]
    [SerializeField] private float velocidadDefault = 3.5f;
    [SerializeField] private float vidaDefault = 100f;
    [SerializeField] private float danioDefault = 10f;

    [Header("Rotación")]
    [SerializeField] private float velocidadRotacion = 5f;
    [SerializeField] private bool mirarAlJugador = true;

    

    private Transform jugador;
    private NavMeshAgent agente;
    private float vidaActual;
    private Rigidbody2D rb;


    private bool isDead = false;
    void Start()
    {
        // Configurar componentes
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.freezeRotation = true;
        }

        // Configurar NavMeshAgent para 2D
        agente = GetComponent<NavMeshAgent>();
        if (agente == null)
        {
            Debug.LogError("NavMeshAgent no encontrado en " + gameObject.name);
            return;
        }

        agente.updateRotation = true;
        agente.updateUpAxis = false;

        // Buscar jugador
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("No se encontró al jugador");
            return;
        }
        jugador = playerObj.transform;

        // Configurar stats
        if (datos == null)
        {
            Debug.LogWarning("Usando valores por defecto en " + gameObject.name);
            agente.speed = velocidadDefault;
            vidaActual = vidaDefault;
        }
        else
        {
            agente.speed = datos.velocidad;
            vidaActual = datos.vida;
        }
    }

    void Update()
    {
        if (jugador == null || agente == null) return;

        // Movimiento con NavMesh
        Vector3 destino = new Vector3(jugador.position.x, jugador.position.y, 0);
        agente.SetDestination(destino);

        // Forzar posición Z = 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        // Rotación 2D hacia el jugador
        if (mirarAlJugador)
        {
            Vector2 direccion = (Vector2)(jugador.position - transform.position);
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            Quaternion rotacion = Quaternion.AngleAxis(angulo, Vector3.forward);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacion,
                velocidadRotacion * Time.deltaTime
            );
        }
    }

    public void Configurar(Zombie datosZombie)
    {
        datos = datosZombie;
        if (agente != null)
        {
            agente.speed = datos != null ? datos.velocidad : velocidadDefault;
        }
        vidaActual = datos != null ? datos.vida : vidaDefault;
    }

    public void RecibirDanio(float cantidad)
    {
        vidaActual -= cantidad;
        if (vidaActual <= 0)
        {
            Morir();
        }
    }



    public void Morir()
    {
        if (isDead) return;
        isDead = true;

        SoltarDrop();
        Destroy(gameObject);  
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            VidaJugador vida = other.gameObject.GetComponent<VidaJugador>();
            if (vida != null)
            {
                float danio = datos != null ? datos.danio : danioDefault;
                vida.RecibirDanio(10);
            }
        }
    }

    private void SoltarDrop()
    {
        float rand = Random.value;

        if (rand < 0.3f)
        {
            VidaJugador vida = GameObject.FindGameObjectWithTag("Player")?.GetComponent<VidaJugador>();
            if (vida != null) vida.Curar(25);
            Debug.Log("cura");


        }
        else if (rand < 0.6f)
        {
            Jugador controlDisparo = GameObject.FindGameObjectWithTag("Player") ? .GetComponent<Jugador>();
            Debug.Log("Drop: +15 balas");
            controlDisparo.AniadirBalas(15); 

        }
        else if (rand < 0.8f)
        {

            if(EconomyManager.Instance.AddMoney(20))
            Debug.Log("aaaaaaaaaaaaaa");
            else
                Debug.Log("qqqqqqqqqqqqqqq");
            
        }
        // Añadir una kill
        if (KillsManager.Instance != null)
        {
            KillsManager.Instance.AddKill();
            Debug.Log("Kill añadida");
        }
    }

   
}