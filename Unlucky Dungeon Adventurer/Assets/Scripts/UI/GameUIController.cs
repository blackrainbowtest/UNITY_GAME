using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIController : MonoBehaviour
{
    [Header("HP Bar")]
    public Image hpFill;
    public TextMeshProUGUI hpText;

    [Header("MP Bar")]
    public Image mpFill;
    public TextMeshProUGUI mpText;

    [Header("Stamina Bar")]
    public Image staFill;
    public TextMeshProUGUI staText;

    [Header("Portrait")]
    public RawImage portrait; // если есть

    private PlayerData p => GameData.CurrentPlayer;

    private void Start()
    {
        // Пытаемся обновить UI, если инициализация уже завершена
        if (GameInitializer.IsInitialized() && GameData.CurrentPlayer != null)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        var p = GameData.CurrentPlayer;
        if (p == null)
            return;

        // Check if UI elements are still valid before updating
        if (hpFill == null || mpFill == null || staFill == null || this == null)
        {
            // Object is being destroyed, OnDisable will handle unsubscribe
            return;
        }

        // HP
        if (hpFill != null && hpText != null)
        {
            hpFill.fillAmount = (float)p.currentHP / p.finalMaxHP;
            hpText.text = $"{p.currentHP}/{p.finalMaxHP}";
        }

        // MP
        if (mpFill != null && mpText != null)
        {
            mpFill.fillAmount = (float)p.currentMP / p.finalMaxMP;
            mpText.text = $"{p.currentMP}/{p.finalMaxMP}";
        }

        // Stamina
        if (staFill != null && staText != null)
        {
            staFill.fillAmount = (float)p.currentStamina / p.finalMaxStamina;
            staText.text = $"{p.currentStamina}/{p.finalMaxStamina}";
        }
    }

    private void OnEnable()
    {
        UIEvents.OnPlayerStatsChanged += UpdateUI;
    }

    private void OnDisable()
    {
        UIEvents.OnPlayerStatsChanged -= UpdateUI;
    }
}
