using TMPro; // o UnityEngine.UI si usas UI Text
using UnityEngine;

public class MoneyTextUpdater : MonoBehaviour
{
    private TMP_Text moneyText;

    void Start()
    {
        moneyText = GetComponent<TMP_Text>();
        UpdateText();

        // Registrar este updater en EconomyManager
        EconomyManager.Instance.RegisterMoneyText(this);
    }

    public void UpdateText()
    {
        if (moneyText != null)
        {
            moneyText.text = EconomyManager.Instance.GetCurrentMoney().ToString();
        }
    }

    void OnDestroy()
    {
        // Quitar este updater al destruir el objeto
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.UnregisterMoneyText(this);
        }
    }
}
