using UnityEngine;

public class ActivadorDeArma : MonoBehaviour
{
    // Armas que se activan
    public GameObject pistola;
    public GameObject escopeta;
    public GameObject fusil;
    public GameObject francotirador;

    // SpriteRenderer del personaje
    public SpriteRenderer personajeRenderer;

    // Sprites del personaje según el arma
    public Sprite pistolaSprite;
    public Sprite escopetaSprite;
    public Sprite fusilSprite;
    public Sprite francotiradorSprite;

    void Start()
    {
        string arma = SeleccionarArmaUI.armaSeleccionada;
        Debug.Log($"[Armas] Arma seleccionada: {arma}");

        // Desactivar todas las armas
        DesactivarTodasLasArmas();

        switch (arma)
        {
            case "Pistola":
                pistola.SetActive(true);
                personajeRenderer.sprite = pistolaSprite;
                break;
            case "Escopeta":
                escopeta.SetActive(true);
                personajeRenderer.sprite = escopetaSprite;
                break;
            case "Fusil":
                fusil.SetActive(true);
                personajeRenderer.sprite = fusilSprite;
                break;
            case "Francotirador":
                francotirador.SetActive(true);
                personajeRenderer.sprite = francotiradorSprite;
                break;
            default:
                Debug.LogWarning("[Armas] No se reconoció el arma seleccionada.");
                break;
        }
    }

    void DesactivarTodasLasArmas()
    {
        pistola.SetActive(false);
        escopeta.SetActive(false);
        fusil.SetActive(false);
        francotirador.SetActive(false);
    }
}
