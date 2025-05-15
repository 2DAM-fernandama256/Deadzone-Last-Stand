using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class ArmaManager : MonoBehaviour
{
    public static ArmaManager Instance;

    public Dictionary<string, arma> armasJugador = new Dictionary<string, arma>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[ArmaManager] Instance creada");
        }
        else
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

    // Carga las armas desde PlayFab o inicializa por defecto si no existen
    public void CargarArmasDesdePlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            resultado =>
            {
                armasJugador.Clear();

                bool huboCambios = false;

                foreach (string nombre in new[] { "Pistola", "Escopeta", "Fusil", "Francotirador" })
                {
                    if (resultado.Data != null && resultado.Data.ContainsKey(nombre))
                    {
                        string[] valores = resultado.Data[nombre].Value.Split(',');

                        arma armaCargada = new arma(
                            nombre,
                            int.Parse(valores[0]),   // nivel
                            float.Parse(valores[1]), // daño
                            float.Parse(valores[2]), // distancia
                            int.Parse(valores[3]),   // cargador
                            int.Parse(valores[4])    // totalBalas
                        );

                        armasJugador[nombre] = armaCargada;
                        Debug.Log($"Arma {nombre} cargada desde PlayFab.");
                    }
                    else
                    {
                        // No existe el arma en PlayFab, inicializar por defecto
                        arma armaDefecto = ObtenerArmaPorDefecto(nombre);
                        armasJugador[nombre] = armaDefecto;
                        Debug.LogWarning($"Arma {nombre} no encontrada en PlayFab. Inicializando por defecto.");
                        huboCambios = true;
                    }
                }

                if (huboCambios)
                {
                    Debug.Log("Se guardarán las armas que no existían en PlayFab con valores por defecto.");
                    GuardarArmasEnPlayFab();
                }
                else
                {
                    Debug.Log("Todas las armas cargadas correctamente desde PlayFab.");
                }
            },
            error =>
            {
                Debug.LogError("Error al cargar armas: " + error.GenerateErrorReport());
                InicializarArmasPorDefecto();
            });
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
        if (!armasJugador.ContainsKey(nombreArma))
        {
            Debug.LogWarning("[ArmaManager] Arma no encontrada: " + nombreArma);
            return false;
        }

        arma arma = armasJugador[nombreArma];

        if (arma.nivel >= 4)
        {
            Debug.Log("[ArmaManager] El arma ya está al nivel máximo: " + nombreArma);
            return false;
        }

        int costo = ObtenerCostoMejora(nombreArma);

        if (!EconomyManager.Instance.SpendMoney(costo))
        {
            Debug.Log("[ArmaManager] No hay suficiente dinero para mejorar " + nombreArma);
            return false;
        }

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

        Debug.Log($"[ArmaManager] {nombreArma} mejorada a nivel {arma.nivel}");

        GuardarArmasEnPlayFab();
        return true;
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
    public arma ObtenerArma(string nombre)
    {
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
