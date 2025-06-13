using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ArmaManager : MonoBehaviour
{
    public static ArmaManager Instance;
    public static Dictionary<string, arma> armasJugador = new Dictionary<string, arma>();

    //persistencia entre escenas
    public AudioSource audioerror;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            Debug.Log("[ArmaManager] Instance creada");
        }
        else if (Instance != this)
        {
            Debug.Log("[ArmaManager] Instance ya existe, destruyendo objeto duplicado");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("[ArmaManager] Inicio: cargando armas desde PlayFab...");
        CargarArmasDesdePlayFab();
    }

    // Metodo optimizado para cargar armas
    public void CargarArmasDesdePlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            resultado => ProcesarDatosArmas(resultado),
            error => ManejarErrorCarga(error));
    }

    //carga el arma desde playfab o si no la tiene la introduce
    private void ProcesarDatosArmas(GetUserDataResult resultado)
    {
        armasJugador.Clear();
        bool huboCambios = false;

        string[] nombresArmas = { "Pistola", "Escopeta", "Fusil", "Francotirador" };

        foreach (string nombre in nombresArmas)
        {
            if (resultado.Data != null && resultado.Data.TryGetValue(nombre, out var itemData))
            {
                CargarArmaDesdeData(nombre, itemData.Value);
            }
            else
            {
                InicializarArmaPorDefecto(nombre);
                huboCambios = true;
            }
        }

        if (huboCambios) GuardarArmasEnPlayFab();
    }

    //lee el value del arma cargada 
    private void CargarArmaDesdeData(string nombre, string data)
    {
        string[] valores = data.Split(',');

        if (valores.Length == 5)
        {
            var armaCargada = new arma(
                nombre,
                int.Parse(valores[0]),
                float.Parse(valores[1]),
                float.Parse(valores[2]),
                int.Parse(valores[3]),
                int.Parse(valores[4])
            );

            armasJugador[nombre] = armaCargada;
            ActivarEstrellas(nombre, armaCargada.nivel);
        }
    }
    //añade al diccionario el arma por defecto si no la tiene
    private void InicializarArmaPorDefecto(string nombre)
    {
        armasJugador[nombre] = ObtenerArmaPorDefecto(nombre);
        Debug.LogWarning($"Arma {nombre} no encontrada en PlayFab. Inicializando por defecto.");
    }

    // Maneja errores al cargar armas desde PlayFab
    private void ManejarErrorCarga(PlayFabError error)
    {
        Debug.LogError("Error al cargar armas: " + error.GenerateErrorReport());
        InicializarArmasPorDefecto();
    }

    // Activa las estrellas según el nivel del arma
    void ActivarEstrellas(string nombreArma, int nivel)
    {
        if (nivel <= 1) return; // Nivel 1 no muestra estrellas

        string nombreCuadro = $"cuadro_{nombreArma.ToLower()}"; //porlo lo escribi en minusculas para evitar errores
        string prefijoEstrella = "";

        switch (nombreArma)
        {
            case "Pistola": prefijoEstrella = "PistolaS"; break;
            case "Francotirador": prefijoEstrella = "FrancotiradorS"; break;
            case "Escopeta": prefijoEstrella = "EscopetaS"; break;
            case "Fusil": prefijoEstrella = "FusilS"; break;
            default: return;
        }

        GameObject cuadro = GameObject.Find(nombreCuadro);
        if (cuadro == null)
        {
            Debug.LogWarning($"No se encontró el GameObject {nombreCuadro}");
            return;
        }

        for (int i = 2; i <= 4; i++) // Nivel 2 a 4
        {
            Transform estrella = cuadro.transform.Find($"{prefijoEstrella}{i}");
            if (estrella != null)
            {
                estrella.gameObject.SetActive(i <= nivel); //actiba las estrellas
            }
        }
    }



    // Metodo para devolver un arma por defecto según nombre
    private arma ObtenerArmaPorDefecto(string nombre)
    {
        switch (nombre)
        {
            case "Pistola":
                return new arma("Pistola", 1, 10, 10, 12, 60);
            case "Escopeta":
                return new arma("Escopeta", 1, 25, 6, 6, 30);
            case "Fusil":
                return new arma("Fusil", 1, 15, 15, 30, 120);
            case "Francotirador":
                return new arma("Francotirador", 1, 50, 30, 5, 20);
            default:
                Debug.LogWarning($"Arma por defecto no definida para {nombre}, usando valores genéricos.");
                return new arma(nombre, 1, 10, 10, 10, 50);
        }
    }

    // iniciaq todas las armas con valores por defecto
    private void InicializarArmasPorDefecto()
    {
        armasJugador["Pistola"] = new arma("Pistola", 1, 10, 10, 12, 60);
        armasJugador["Escopeta"] = new arma("Escopeta", 1, 25, 6, 6, 30);
        armasJugador["Fusil"] = new arma("Fusil", 1, 15, 15, 30, 120);
        armasJugador["Francotirador"] = new arma("Francotirador", 1, 50, 30, 5, 20);

        Debug.Log("[ArmaManager] Armas inicializadas con valores por defecto.");
    }

    // Mejora específica por tipo de arma
    public bool MejorarArma(string nombreArma)
    {
        bool esValido = true;

        if (!armasJugador.ContainsKey(nombreArma))
        {
            Debug.LogWarning("[ArmaManager] Arma no encontrada: " + nombreArma);
            esValido = false;
        }

        arma arma = armasJugador.ContainsKey(nombreArma) ? armasJugador[nombreArma] : null;

        if (esValido && arma.nivel >= 4)
        {
            Debug.Log("[ArmaManager] El arma ya está al nivel máximo: " + nombreArma);
            esValido = false;
        }

        int costo = ObtenerCostoMejora(nombreArma);

        if (esValido && !EconomyManager.Instance.SpendMoney(costo))
        {
            Debug.Log("[ArmaManager] No hay suficiente dinero para mejorar " + nombreArma);
            esValido = false;
        }
        //mejora por cada arma
        if (esValido)
        {
            arma.nivel++;

            switch (nombreArma)
            {
                case "Pistola":
                    arma.danio += 3f;
                    arma.cargador += 2;
                    break;

                case "Escopeta":
                    arma.danio += 6f;
                    arma.distancia += 1f;
                    break;

                case "Fusil":
                    arma.danio += 4f;
                    arma.distancia += 2f;
                    arma.cargador += 5;
                    break;

                case "Francotirador":
                    arma.danio += 10f;
                    arma.cargador += 1;

                    break;
            }

            ActivarEstrellas(nombreArma, arma.nivel);

            Debug.Log($"[ArmaManager] {nombreArma} mejorada a nivel {arma.nivel}");


        }
        if (!esValido && audioerror != null)
        {
            audioerror.Play();
        }

        GuardarArmasEnPlayFab();

        return esValido;
    }



    // Costo de mejora de armas
    private int ObtenerCostoMejora(string nombreArma)
    {
        switch (nombreArma)
        {
            case "Pistola": return 50;
            case "Escopeta": return 60;
            case "Fusil": return 120;
            case "Francotirador": return 80;
            default:
                Debug.LogWarning("[ArmaManager] Nombre de arma desconocido para costo de mejora: " + nombreArma);
                return 999;
        }
    }

        
    

    // Guarda las armas en PlayFab como strings
    public void GuardarArmasEnPlayFab()
    {
        var datos = new Dictionary<string, string>();

        foreach (var par in armasJugador)
        {
            arma a = par.Value;
            string valor = $"{a.nivel},{a.danio},{a.distancia},{a.cargador},{a.totalBalas}";
            datos.Add(par.Key, valor);
            Debug.Log($"[ArmaManager] Preparando para guardar: {par.Key} = {valor}");
        }

        var request = new UpdateUserDataRequest
        {
            Data = datos
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("[ArmaManager] Armas guardadas en PlayFab correctamente."),
            error => Debug.LogError("[ArmaManager] Error al guardar armas: " + error.GenerateErrorReport()));
    }



    // Metodo para obtener datos del arma desde otra escena
    public static arma ObtenerArma()    
    {
        string nombre = SeleccionarArmaUI.armaSeleccionada;
        Debug.Log("[ArmaManager] Solicitud de arma: " + nombre);
        if (armasJugador.ContainsKey(nombre))
        {
            return armasJugador[nombre];
        }
        else
        {
            Debug.LogWarning("[ArmaManager] Solicitud de arma inexistente: " + nombre);
            return null;
        }
    }
}
