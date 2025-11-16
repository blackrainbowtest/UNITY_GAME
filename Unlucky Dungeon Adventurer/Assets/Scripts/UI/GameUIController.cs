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
        // Подписываемся — UI обновится, когда игрок появится или его статы изменятся
        UIEvents.OnPlayerStatsChanged += UpdateUI;

        // Если игрок уже есть, обновляем сразу
        if (GameData.CurrentPlayer != null)
        {
            UpdateUI();
        }
        // Если игрока нет — ждём события OnPlayerStatsChanged (которое должно прийти после загрузки игрока)
    }

    private void UpdateUI()
    {
        var p = GameData.CurrentPlayer;
        if (p == null)
            return;

        // HP
        hpFill.fillAmount = (float)p.currentHP / p.finalMaxHP;
        hpText.text = $"{p.currentHP}/{p.finalMaxHP}";

        // MP
        mpFill.fillAmount = (float)p.currentMP / p.finalMaxMP;
        mpText.text = $"{p.currentMP}/{p.finalMaxMP}";

        // Stamina
        staFill.fillAmount = (float)p.currentStamina / p.finalMaxStamina;
        staText.text = $"{p.currentStamina}/{p.finalMaxStamina}";
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
