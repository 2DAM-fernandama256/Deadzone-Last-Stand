using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class Ranking : MonoBehaviour
{
    [Header("UI para Top 5")]
    public TMP_Text[] top5Texts; 

    [Header("UI para la posición del jugador")]
    public TMP_Text playerRankText;

    [Header("Nombre de la estadística en PlayFab")]
    public string statisticName = "kills";

    // Llamar esto para actualizar el ranking
    public void ActualizarRanking()
    {
        ObtenerTop5();
        ObtenerPosicionDelJugador();
    }

    private void ObtenerTop5()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = 0,
            MaxResultsCount = 5
        };

        PlayFabClientAPI.GetLeaderboard(request, result =>
        {
            for (int i = 0; i < top5Texts.Length; i++)
            {
                if (i < result.Leaderboard.Count)
                {
                    var item = result.Leaderboard[i];
                    string nombre = item.DisplayName ;
                    top5Texts[i].text = $"{item.Position + 1}. {nombre} - {item.StatValue} kills";
                }
                else
                {
                    top5Texts[i].text = $"{i + 1}. ---";
                }
            }
        },
        error => Debug.LogError("Error al obtener Top 5: " + error.GenerateErrorReport()));
    }

    private void ObtenerPosicionDelJugador()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = statisticName,
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, result =>
        {
            var item = result.Leaderboard.FirstOrDefault();
            if (item != null)
            {
                string nombre = item.DisplayName ?? item.PlayFabId;
                playerRankText.text = $"Tu mejor partida: #{item.Position + 1} - {item.StatValue} kills";
            }
            else
            {
                playerRankText.text = "Aún no tienes ranking.";
            }
        },
        error => Debug.LogError("Error al obtener tu posición: " + error.GenerateErrorReport()));
    }
}
