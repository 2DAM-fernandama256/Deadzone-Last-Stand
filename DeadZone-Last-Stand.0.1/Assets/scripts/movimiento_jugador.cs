using Unity.Burst.Intrinsics;
using UnityEngine;
using TMPro;

public class Jugador : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;
    [SerializeField] private float multiplicadorSprint = 2f;

    [Header("Rotación")]
    [SerializeField] private float velocidadRotacion = 15f;
    [SerializeField] private Transform cuerpoRotador;

    [Header("Cámara")]
    [SerializeField] private float suavizadoCamara = 5f;
    [SerializeField] private Vector3 offsetCamara = new Vector3(0, 0, -10);

    [Header("Disparo")]
    [SerializeField] private GameObject prefabBala;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float tiempoEntreDisparosBase = 0.3f; // Tiempo base entre disparos

    private Rigidbody2D rb;
    private Camera camara;
    private Vector2 inputMovimiento;
    private bool estaSprintando;
    private Vector3 objetivoMouse;
    private arma armaActual;
    private float tiempoProximoDisparo;
    private float tiempoActualEntreDisparos;
    public TextMeshProUGUI mensajeDisparoTexto;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        camara = Camera.main;
        if (cuerpoRotador == null) cuerpoRotador = transform;
    }

    private void Start()
    {
        armaActual = ArmaManager.ObtenerArma();
        tiempoActualEntreDisparos = tiempoEntreDisparosBase;
        Debug.Log("Arma equipada: " + armaActual.nombre);
        string mensaje = $"{armaActual.balasActuales}/{armaActual.totalBalas}";
        mensajeDisparoTexto.text = mensaje;
    }

    private void Update()
    {
        ProcesarInput();

        if (Input.GetButton("Fire1") && Time.time >= tiempoProximoDisparo)
        {
            Disparar();
            tiempoProximoDisparo = Time.time + tiempoActualEntreDisparos;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            armaActual.Recargar();
            Debug.Log("Recargando...");
        }
    }

    private void FixedUpdate()
    {
        MoverJugador();
        RotarHaciaMouse();
        MoverCamara();
    }

    private void ProcesarInput()
    {
        inputMovimiento = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        estaSprintando = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void MoverJugador()
    {
        float velocidadActual = estaSprintando ? velocidadMovimiento * multiplicadorSprint : velocidadMovimiento;
        rb.linearVelocity = inputMovimiento * velocidadActual;
    }

    private void RotarHaciaMouse()
    {
        if (camara == null) return;

        objetivoMouse = camara.ScreenToWorldPoint(Input.mousePosition);
        objetivoMouse.z = 0;

        Vector2 direccion = (objetivoMouse - cuerpoRotador.position).normalized;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

        cuerpoRotador.rotation = Quaternion.Lerp(
            cuerpoRotador.rotation,
            Quaternion.Euler(0, 0, angulo),
            velocidadRotacion * Time.deltaTime
        );
    }

    private void MoverCamara()
    {
        if (camara == null) return;

        camara.transform.position = Vector3.Lerp(
            camara.transform.position,
            transform.position + offsetCamara,
            suavizadoCamara * Time.deltaTime
        );
    }

    private void Disparar()
    {
        if (armaActual.Disparar())
        {
            GameObject balaGO = Instantiate(prefabBala, puntoDisparo.position, cuerpoRotador.rotation);
            Bala bala = balaGO.GetComponent<Bala>();

            Vector2 direccionDisparo = (objetivoMouse - puntoDisparo.position).normalized;
            float velocidad = 15f + (armaActual.nivel * 2); // Velocidad base + bonus por nivel

            bala.Configurar(armaActual.danio, armaActual.distancia, velocidad, direccionDisparo);

            // Reducir tiempo entre disparos según nivel (mínimo 0.1 segundos)
            tiempoActualEntreDisparos = Mathf.Max(0.1f, tiempoEntreDisparosBase * Mathf.Pow(0.9f, armaActual.nivel - 1));

            string mensaje = $"{armaActual.balasActuales}/{armaActual.totalBalas}";
            mensajeDisparoTexto.text = mensaje;
        }
        else
        {
            Debug.LogWarning("No hay balas disponibles para disparar.");
        }
    }

    public void AniadirBalas(int cantidad)
    {
        if (cantidad <= 0)
        {
            Debug.LogWarning("Intentaste añadir una cantidad no válida de balas.");
            return;
        }

        armaActual.totalBalas += cantidad;

        // Asegúrate de que balas actuales no excedan las totales
        armaActual.balasActuales = Mathf.Min(armaActual.balasActuales, armaActual.totalBalas);

        // Actualizar UI
        if (mensajeDisparoTexto != null)
        {
            string mensaje = $"{armaActual.balasActuales}/{armaActual.totalBalas}";
            mensajeDisparoTexto.text = mensaje;
        }

        Debug.Log($"Se añadieron {cantidad} balas. Total: {armaActual.totalBalas}");
    }

}