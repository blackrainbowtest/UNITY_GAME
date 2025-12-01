using UnityEngine;

public class PlayerStatsController : MonoBehaviour
{
    public static PlayerStatsController Instance;

    private SaveData Save => GameManager.Instance.GetCurrentGameData();
    private PlayerSaveData Player => Save.player;

    private void Awake()
    {
        Instance = this;
    }

    // ============================
    //        HP
    // ============================
    public void ModifyHP(int amount)
    {
        Player.currentHP += amount;

        if (Player.currentHP < 0)
            Player.currentHP = 0;

        if (Player.currentHP > Player.baseMaxHP)
            Player.currentHP = Player.baseMaxHP;

        UIEvents.OnPlayerStatsChanged?.Invoke();

        Debug.Log($"[HP] HP changed by {amount}, now {Player.currentHP}/{Player.baseMaxHP}");
    }

    // ============================
    //        MP
    // ============================
    public void ModifyMP(int amount)
    {
        Player.currentMP += amount;

        if (Player.currentMP < 0)
            Player.currentMP = 0;

        if (Player.currentMP > Player.baseMaxMP)
            Player.currentMP = Player.baseMaxMP;

        UIEvents.OnPlayerStatsChanged?.Invoke();

        Debug.Log($"[MP] MP changed by {amount}, now {Player.currentMP}/{Player.baseMaxMP}");
    }

    // ============================
    //        STAMINA
    // ============================
    public void ModifyStamina(int amount)
    {
        Player.currentStamina += amount;

        if (Player.currentStamina < 0)
            Player.currentStamina = 0;

        if (Player.currentStamina > Player.baseMaxStamina)
            Player.currentStamina = Player.baseMaxStamina;

        UIEvents.OnPlayerStatsChanged?.Invoke();

        Debug.Log($"[Stamina] Stamina changed by {amount}, now {Player.currentStamina}/{Player.baseMaxStamina}");
    }
}
