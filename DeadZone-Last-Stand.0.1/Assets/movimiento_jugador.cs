using UnityEngine;

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
    [SerializeField] private float tiempoEntreDisparos = 0.3f;

    private Rigidbody2D rb;
    private Camera camara;
    private Vector2 inputMovimiento;
    private bool estaSprintando;
    private float tiempoProximoDisparo;
    private Vector3 objetivoMouse;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        camara = Camera.main;
        if (cuerpoRotador == null) cuerpoRotador = transform;
    }

    private void Update()
    {
        ProcesarInput();
        ProcesarDisparo();
    }

    private void FixedUpdate()
    {
        MoverJugador();
        RotarHaciaMouse();
        MoverCamara();
    }

    private void ProcesarInput()
    {
        // Movimiento WASD
        inputMovimiento = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        // Sprint
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

    private void ProcesarDisparo()
    {
        if (Input.GetButton("Fire1") && Time.time >= tiempoProximoDisparo)
        {
            tiempoProximoDisparo = Time.time + tiempoEntreDisparos;
            DispararBala();
        }
    }

    private void DispararBala()
    {
        if (prefabBala == null)
        {
            Debug.LogError("Prefab de bala no asignado en el Inspector");
            return;
        }

        if (puntoDisparo == null)
        {
            Debug.LogError("Punto de disparo no asignado");
            return;
        }

        if (camara == null)
        {
            camara = Camera.main;
            if (camara == null)
            {
                Debug.LogError("No se encontró la cámara principal");
                return;
            }
        }

        Vector2 direccionDisparo = ((Vector2)objetivoMouse - (Vector2)puntoDisparo.position).normalized;
        GameObject bala = Instantiate(prefabBala, puntoDisparo.position, Quaternion.identity);

        if (bala.TryGetComponent(out Bala componenteBala))
        {
            componenteBala.Disparar(direccionDisparo);
        }
        else
        {
            Debug.LogError("El prefab de bala no tiene el componente Bala");
            Destroy(bala);
        }
    }
}