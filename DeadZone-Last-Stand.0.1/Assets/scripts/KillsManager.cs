using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro; 
using System.Collections.Generic;

public class KillsManager : MonoBehaviour
{
    public static KillsManager Instance;
    public TextMeshProUGUI textoKillsActuales;  
    private int bestKills = 0;     // Guardado en PlayFab
    private int currentKills = 0;  // Durante la partida

    private void Awake()
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

    private void Start()
    {
        LoadBestKills();
    }

    void Update()
    {
        if (textoKillsActuales != null)
        {
            textoKillsActuales.text = currentKills.ToString();
        }
    }

    public void AddKill()
    {
        currentKills++;
        Debug.Log("Kills actuales: " + currentKills);
    }

    public void CheckAndSaveBestKills()
    {
        if (currentKills > bestKills)
        {
            bestKills = currentKills;
            SaveBestKills();
        }
        else
        {
            Debug.Log("No se superó el récord. Récord actual: " + bestKills);
        }
    }

    private void LoadBestKills()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result => {
                if (result.Data != null && result.Data.ContainsKey("kills"))
                {
                    bestKills = int.Parse(result.Data["kills"].Value);
                    Debug.Log("Kills cargadas desde PlayFab: " + bestKills);
                }
                else
                {
                    bestKills = 0;
                    SaveBestKills(); // Primera vez: crear el dato
                    Debug.Log("No había récord, se creó uno nuevo con 0 kills.");
                }
            },
            error => {
                Debug.LogError("Error al cargar kills desde PlayFab: " + error.GenerateErrorReport());
            });
    }

    private void SaveBestKills()
    {
        // 1. Guardar en PlayerData (como ya haces)
        var userDataRequest = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { "kills", bestKills.ToString() }
        }
        };

        PlayFabClientAPI.UpdateUserData(userDataRequest,
            result => Debug.Log("Récord de kills guardado en PlayerData: " + bestKills),
            error => Debug.LogError("Error al guardar kills en PlayerData: " + error.GenerateErrorReport()));

        // 2. Guardar en PlayerStatistics (para ranking)
        var statsRequest = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate
            {
                StatisticName = "kills", 
                Value = bestKills
            }
        }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(statsRequest,
            result => Debug.Log("Récord de kills enviado a estadísticas (ranking): " + bestKills),
            error => Debug.LogError("Error al guardar kills en estadísticas: " + error.GenerateErrorReport()));
    }


    public int GetCurrentKills() => currentKills;
    public int GetBestKills() => bestKills;
}
