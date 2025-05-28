using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance; // Singleton para acceso facil

    private int _playerMoney = 50;

    private List<MoneyTextUpdater> moneyTexts = new List<MoneyTextUpdater>();// Valor por defecto

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadPlayerMoney();
    }

    // Cargar dinero del jugador desde PlayFab
    public void LoadPlayerMoney()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result => {
                if (result.Data != null && result.Data.ContainsKey("PlayerMoney"))
                {
                    _playerMoney = int.Parse(result.Data["PlayerMoney"].Value);
                    Debug.Log("Dinero cargado: " + _playerMoney);
                }
                else
                {
                    // Si no existe, guardamos el valor inicial 
                    _playerMoney = 50;
                    SavePlayerMoney();
                }
                UpdateMoneyUI(); // Actualizar UI
            },
            error => {
                Debug.LogError("Error al cargar dinero: " + error.GenerateErrorReport());
                // Usar valor por defecto si hay error
                _playerMoney = 50;
                UpdateMoneyUI();
            });
    }

    // Guardar dinero en PlayFab
    private void SavePlayerMoney()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"PlayerMoney", _playerMoney.ToString()}
            }
        };
        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("Dinero guardado exitosamente"),
            error => Debug.LogError("Error al guardar dinero: " + error.GenerateErrorReport()));
    }

    // Método público para agregar dinero 
    public void AddMoney(int amount)
    {
        _playerMoney += amount;
        SavePlayerMoney();
        UpdateMoneyUI();

        Debug.Log($"Se agregaron {amount} monedas. Total: {_playerMoney}");
    }

    // Método público para gastar dinero
    public bool SpendMoney(int amount)
    {
        if (_playerMoney >= amount)
        {
            _playerMoney -= amount;
            SavePlayerMoney();
            UpdateMoneyUI();
            return true; // Transacción exitosa
        }
        return false; // No hay suficiente dinero
    }

    // Obtener el dinero actual
    public int GetCurrentMoney()
    {
        return _playerMoney;
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

        Debug.Log("Dinero actual: " + _playerMoney);
    }
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