using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ArmaManager : MonoBehaviour
{
    public static ArmaManager Instance;
    public static Dictionary<string, arma> armasJugador = new Dictionary<string, arma>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Añade esto para persistencia entre escenas
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

    // Método optimizado para cargar armas
    public void CargarArmasDesdePlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            resultado => ProcesarDatosArmas(resultado),
            error => ManejarErrorCarga(error));
    }

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

    private void InicializarArmaPorDefecto(string nombre)
    {
        armasJugador[nombre] = ObtenerArmaPorDefecto(nombre);
        Debug.LogWarning($"Arma {nombre} no encontrada en PlayFab. Inicializando por defecto.");
    }

    private void ManejarErrorCarga(PlayFabError error)
    {
        Debug.LogError("Error al cargar armas: " + error.GenerateErrorReport());
        InicializarArmasPorDefecto();
    }

    void ActivarEstrellas(string nombreArma, int nivel)
    {
        if (nivel <= 1) return; // Nivel 1 no muestra estrellas

        string nombreCuadro = $"cuadro_{nombreArma.ToLower()}"; //porlo lo escribi en minusculas
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
                estrella.gameObject.SetActive(i <= nivel);
            }
        }
    }



    // Método para devolver arma por defecto según nombre
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

    // Armas base con diferentes stats iniciales
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
                    break;
            }

            ActivarEstrellas(nombreArma, arma.nivel);

            Debug.Log($"[ArmaManager] {nombreArma} mejorada a nivel {arma.nivel}");
        }

        GuardarArmasEnPlayFab();

        return esValido;
    }



    // Costo de mejora específico por arma
    private int ObtenerCostoMejora(string nombreArma)
    {
        switch (nombreArma)
        {
            case "Pistola": return 10;
            case "Escopeta": return 20;
            case "Fusil": return 30;
            case "Francotirador": return 40;
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



    // Método para obtener datos de un arma desde otro script (ej. UI)
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
