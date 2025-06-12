using UnityEngine;
using UnityEngine.UI;

public class SeleccionarArmaUI : MonoBehaviour
{
    public static string armaSeleccionada = null;  

    
    public MenuInicial menuInicial;

 
    public void SeleccionarArma()
    {
        string nombreArma = gameObject.name; // El nombre del boton = nombre del arma  

        int costo = ObtenerCostoArma(nombreArma);

        Debug.Log($"[UI] Intentando seleccionar arma {nombreArma} por {costo} monedas.");

        if (EconomyManager.Instance.SpendMoney(costo))
        {
            Debug.Log($"[UI] Arma {nombreArma} seleccionada correctamente.");
            armaSeleccionada = nombreArma;

            // Usar la referencia de instancia para llamar al metodo jugar  
            if (menuInicial != null)
            {
                menuInicial.jugar();
            }
            else
            {
                Debug.LogError("[UI] La referencia a MenuInicial no está asignada.");
            }
        }
        else
        {
            Debug.LogWarning($"[UI] No tienes suficiente dinero para comprar {nombreArma}.");
        }
    }

    // Costo de cada arma segun su nombre  
    private int ObtenerCostoArma(string nombreArma)
    {
        switch (nombreArma)
        {
            case "Pistola": return 0;   // Gratis  
            case "Escopeta": return 200;
            case "Fusil": return 500;
            case "Francotirador": return 300;
            default:
                Debug.LogWarning($"[UI] Costo no definido para el arma {nombreArma}");
                return 9999;
        }
    }
}
