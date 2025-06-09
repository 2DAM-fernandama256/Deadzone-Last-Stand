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
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "kills", bestKills.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("Récord de kills guardado: " + bestKills),
            error => Debug.LogError("Error al guardar kills en PlayFab: " + error.GenerateErrorReport()));
    }

    public int GetCurrentKills() => currentKills;
    public int GetBestKills() => bestKills;
}
