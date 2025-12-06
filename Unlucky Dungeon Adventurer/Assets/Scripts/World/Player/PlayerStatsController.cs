using UnityEngine;

public class PlayerStatsController : MonoBehaviour
{
    public static PlayerStatsController Instance;

    // Work directly with GameData.CurrentPlayer instead of temporary SaveData
    private PlayerData Player => GameData.CurrentPlayer;

    /// <summary>
    /// Ensure a PlayerStatsController exists in the scene.
    /// Called automatically before scene loads.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void EnsureExists()
    {
        if (Instance == null)
        {
            var obj = new GameObject("PlayerStatsController");
            obj.AddComponent<PlayerStatsController>();
            UDADebug.Log("[PlayerStatsController] Created automatically");
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            UDADebug.Log("[PlayerStatsController] Singleton initialized");
        }
        else
        {
            Destroy(gameObject);
        }
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

        UIEvents.InvokePlayerStatsChanged();

        UDADebug.Log($"[HP] HP changed by {amount}, now {Player.currentHP}/{Player.baseMaxHP}");
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

        UIEvents.InvokePlayerStatsChanged();

        UDADebug.Log($"[MP] MP changed by {amount}, now {Player.currentMP}/{Player.baseMaxMP}");
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

        UIEvents.InvokePlayerStatsChanged();

        UDADebug.Log($"[Stamina] Stamina changed by {amount}, now {Player.currentStamina}/{Player.baseMaxStamina}");
    }

    // ============================
    //        MAP POSITION
    // ============================
    public void UpdateMapPosition(int x, int y)
    {
        Player.mapPosX = x;
        Player.mapPosY = y;

        UDADebug.Log($"[Position] Player moved to ({x}, {y})");
    }
}

