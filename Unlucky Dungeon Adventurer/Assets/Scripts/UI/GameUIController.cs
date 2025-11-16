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
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (p == null)
        {
            Debug.LogWarning("GameUIController: CurrentPlayer == NULL");
            return;
        }

        // HP
        hpFill.fillAmount = (float)p.currentHP / p.maxHP;
        hpText.text = $"{p.currentHP}/{p.maxHP}";

        // MP
        mpFill.fillAmount = (float)p.currentMP / p.maxMP;
        mpText.text = $"{p.currentMP}/{p.maxMP}";

        // Stamina
        staFill.fillAmount = (float)p.currentStamina / p.maxStamina;
        staText.text = $"{p.currentStamina}/{p.maxStamina}";
    }
}
