using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ControldeCadaArma : MonoBehaviour
{

    //Este script esta en cada recuadro de cada arma
    [Header("Configuración del arma")]
    public string nombreArma; 
    public Button botonMejorar;

    private void Start()
    {
        botonMejorar.onClick.AddListener(Mejorar);
    }

    void Mejorar()
    {
        bool mejorado = ArmaManager.Instance.MejorarArma(nombreArma);// llama a la clase ArmaManager para mejorar el arma
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
