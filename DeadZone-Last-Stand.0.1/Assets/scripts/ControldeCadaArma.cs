using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ControldeCadaArma : MonoBehaviour
{
    [Header("Configuración del arma")]
    public string nombreArma; // "Pistola", "Escopeta", "Fusil", "Francotirador"
    public Button botonMejorar;

    private void Start()
    {
        botonMejorar.onClick.AddListener(Mejorar);
    }

    void Mejorar()
    {
        bool mejorado = ArmaManager.Instance.MejorarArma(nombreArma);
        if (mejorado)
        {
            Debug.Log($"{nombreArma} mejorada.");
        }
        else
        {
            Debug.LogWarning($"No se pudo mejorar {nombreArma}.");
        }
    }

    
}
