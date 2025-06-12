using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance; // instancia unica del manager

    private int playerMoney = 50; // Valor por defecto  

    private List<MoneyTextUpdater> moneyTexts = new List<MoneyTextUpdater>();//listado de los objetos que actualizan el dinero en la UI
    private bool iscargado = false; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);// esta instancia no se destruye
        }
        else
        {
            Destroy(gameObject); // Evita duplicados
        }
    }

    void Update()
    {
        // carga el dinero del jugador desde PlayFab una sola vez
        if (!iscargado && PlayFabClientAPI.IsClientLoggedIn() && !Inicio.desdeInicio)
        {
            Debug.Log("Cargando dinero del jugador desde PlayFab...");
            iscargado = true;
            LoadPlayerMoney();
        }
    }


    // Cargar dinero del jugador desde PlayFab
    public void LoadPlayerMoney()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result => {
                if (result.Data != null && result.Data.ContainsKey("PlayerMoney"))
                {
                    playerMoney = int.Parse(result.Data["PlayerMoney"].Value);
                    Debug.Log("Dinero cargado: " + playerMoney);
                }
                else
                {
                    // Si no existe guardamos el valor inicial 
                    playerMoney = 50;
                    SavePlayerMoney();
                }
                UpdateMoneyUI(); // Actualizar UI
            },
            error => {
                Debug.LogError("Error al cargar dinero: " + error.GenerateErrorReport());
                // Usar valor por defecto si hay error
                playerMoney = 50;
                UpdateMoneyUI();
            });
    }

    // Guardar/actualizar dinero en PlayFab 
    private void SavePlayerMoney()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"PlayerMoney", playerMoney.ToString()}
            }
        };
        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("Dinero guardado exitosamente"),
            error => Debug.LogError("Error al guardar dinero: " + error.GenerateErrorReport()));
    }

    // Método público para agregar dinero 
    public bool AddMoney(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("No se puede agregar una cantidad negativa o cero.");
            return false;
        }

        playerMoney += amount;
        SavePlayerMoney();
        UpdateMoneyUI();

        Debug.Log($"Se agregaron {amount} monedas. Total: {playerMoney}");
        return true; // Operación exitosa
    }

    // Método público para gastar dinero
    public bool SpendMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            SavePlayerMoney();
            UpdateMoneyUI();
            return true; // Transacción exitosa
        }
        return false; // No hay suficiente dinero
    }

    // Obtener el dinero actual
    public int GetCurrentMoney()
    {
        return playerMoney;
    }

    // Actualizar la UI 
    private void UpdateMoneyUI()
    {
        foreach (var updater in moneyTexts)
        {
            if (updater != null)
            {
                updater.UpdateText();
            }
        }

        Debug.Log("Dinero actual: " + playerMoney);
    }
    // Registrar y desregistrar objetos que actualizan el texto del dinero
    // esto hace que este constantenente actualizado en la UI
    public void RegisterMoneyText(MoneyTextUpdater updater)
    {
        if (!moneyTexts.Contains(updater))
        {
            moneyTexts.Add(updater);
            updater.UpdateText(); // Inicializa texto con valor actual
        }
    }

    public void UnregisterMoneyText(MoneyTextUpdater updater)
    {
        if (moneyTexts.Contains(updater))
        {
            moneyTexts.Remove(updater);
        }
    }

}