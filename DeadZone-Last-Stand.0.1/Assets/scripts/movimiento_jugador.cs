using UnityEngine;

// Clase que maneja el control del jugador: movimiento, rotación, camara y disparo
public class Jugador : MonoBehaviour
{
    //inspector de unity

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

    //variables internas

    private Rigidbody2D rb;     
    private Camera camara;             
    private Vector2 inputMovimiento;   
    private bool estaSprintando;        
    private float tiempoProximoDisparo; 
    private Vector3 objetivoMouse;


    // Inicializamos referencias y configuraciones
    private void Awake()
    {
        // Inicializamos referencias
        rb = GetComponent<Rigidbody2D>();
        camara = Camera.main;

        // Si no se asigno un objeto para rotar, usamos el propio transform del jugador
        if (cuerpoRotador == null) cuerpoRotador = transform;
    }

    private void Update()
    {
        ProcesarInput();
        ProcesarDisparo();
    }

    // FixedUpdate se usa para física, asegurando que el movimiento sea suave y consistente
    private void FixedUpdate()
    {
        MoverJugador();
        RotarHaciaMouse();
        MoverCamara();
    }

    private void ProcesarInput()
    {
        // Capturamos movimiento 
        inputMovimiento = new Vector2(
            Input.GetAxisRaw("Horizontal"), // A/D o flechas
            Input.GetAxisRaw("Vertical")    // W/S o flechas
        ).normalized;

        // Detectamos si el jugador esta presionando Shift
        estaSprintando = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void MoverJugador()
    {
        // Determinamos velocidad normal o con sprint
        float velocidadActual = estaSprintando ? velocidadMovimiento * multiplicadorSprint : velocidadMovimiento;

        // Movimiento modificando directamente la velocidad lineal
        rb.linearVelocity = inputMovimiento * velocidadActual;
    }

    private void RotarHaciaMouse()
    {
        if (camara == null) return;

        // Convertimos la posición del mouse a coordenadas del mundo
        objetivoMouse = camara.ScreenToWorldPoint(Input.mousePosition);
        objetivoMouse.z = 0; // Z se ignora porque es un juego 2D

        // Calculamos direccion desde el jugador hacia el mouse para sacar el angulo
        Vector2 direccion = (objetivoMouse - cuerpoRotador.position).normalized;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

        // Lerp para rotación suave hacia el objetivo
        cuerpoRotador.rotation = Quaternion.Lerp(
            cuerpoRotador.rotation,
            Quaternion.Euler(0, 0, angulo),
            velocidadRotacion * Time.deltaTime
        );
    }

    private void MoverCamara()
    {
        if (camara == null) return;

        // Interpolacion lineal para seguir suavemente al jugador
        camara.transform.position = Vector3.Lerp(
            camara.transform.position, //la camara siga al jugador
            transform.position + offsetCamara, 
            suavizadoCamara * Time.deltaTime //suabizar camara 
        );
    }

    private void ProcesarDisparo()
    {
        // Si se mantiene presionado el botón de disparo y se respeta el tiempo entre disparos
        if (Input.GetButton("Fire1") && Time.time >= tiempoProximoDisparo)
        {
            tiempoProximoDisparo = Time.time + tiempoEntreDisparos;
            DispararBala();
        }
    }

    private void DispararBala()
    {
        // Verificaciones básicas de referencias
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

        // Calculamos direccion de disparo del punto de disparo al mouse
        Vector2 direccionDisparo = ((Vector2)objetivoMouse - (Vector2)puntoDisparo.position).normalized;

        // Instanciamos clonamos)la bala en el punto de disparo si la primera bala desaparece peta el juego
        GameObject bala = Instantiate(prefabBala, puntoDisparo.position, Quaternion.identity);

        // Llamamos al método Disparar del componente Bala si existe
        if (bala.TryGetComponent(out Bala componenteBala))  
        {
            componenteBala.Disparar(direccionDisparo); //sale la bala hacia el ratón
        }
        else
        {
            Debug.LogError("El prefab de bala no tiene el componente Bala");
            Destroy(bala); // Eliminamos el objeto si está mal configurado
        }
    }
}
