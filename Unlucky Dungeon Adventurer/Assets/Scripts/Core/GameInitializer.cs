using UnityEngine;
using System.Collections;

/// <summary>
/// Coordinates game initialization and player loading.
/// Ensures correct ordering to avoid race conditions when applying save data.
///
/// Initialization flow (high level):
/// 1) SaveSlotUI.Load() may set TempSaveCache.pendingSave
/// 2) Scene loads
/// 3) GameInitializer.Awake() creates the singleton
/// 4) GameInitializer.Start() starts InitializeGame() coroutine which:
///    - Phase 1: checks TempSaveCache
///    - Phase 2: attempts PlayerPrefs load if needed
///    - Phase 3: yields one frame to let other scripts initialize
///    - Phase 4: invokes OnGameInitialized and OnPlayerStatsChanged events
/// 5) GameUIController.Start() subscribes if IsInitialized()
/// 6) UI updates when events are received
/// </summary>
public class GameInitializer : MonoBehaviour
{
    public static GameInitializer Instance { get; private set; }

    private bool isInitialized = false;
    private bool isInitializing = false;

    /// <summary>
    /// Ensure a GameInitializer exists in the scene.
    /// Call this to programmatically create the initializer if absent.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void EnsureExists()
    {
        if (Instance == null)
        {
            var obj = new GameObject("GameInitializer");
            obj.AddComponent<GameInitializer>();
        }
    }

    private void Awake()
    {
    // Create singleton instance and make persistent
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Start initialization coroutine if not already initialized
        if (!isInitialized && !isInitializing)
        {
            StartCoroutine(InitializeGame());
        }
    }

    /// <summary>
    /// Initialization coroutine вЂ” performs ordered loading and event dispatch.
    /// </summary>
    private IEnumerator InitializeGame()
    {
        isInitializing = true;

        // Phase 1: apply pending save if present
        if (TempSaveCache.pendingSave != null)
        {
            GameManager.Instance.LoadGameData(TempSaveCache.pendingSave);
            TempSaveCache.pendingSave = null;
        }
        // Phase 2: attempt to load player from PlayerPrefs if none present
        else if (GameData.CurrentPlayer == null)
        {
            GameData.LoadPlayer();

            if (GameData.CurrentPlayer == null)
            {
                GameData.CurrentPlayer = new PlayerData("", "Hermit", new ClassStats());
            }
        }

        // Phase 3: wait one frame to allow other scripts to finish their Awake/Start
        yield return null;

        // Phase 4: fire initialization events so UI and other systems can update
        UIEvents.InvokeGameInitialized();
        UIEvents.InvokePlayerStatsChanged();

        isInitialized = true;
        isInitializing = false;
    }

    /// <summary>
    /// РџСЂРѕРІРµСЂРєР°: РёРЅРёС†РёР°Р»РёР·РёСЂРѕРІР°РЅР° Р»Рё РёРіСЂР°
    /// </summary>
    public static bool IsInitialized()
    {
        return Instance != null && Instance.isInitialized;
    }

    /// <summary>
    /// РџСЂРѕРІРµСЂРєР°: РёРґС‘С‚ Р»Рё РїСЂРѕС†РµСЃСЃ РёРЅРёС†РёР°Р»РёР·Р°С†РёРё
    /// </summary>
    public static bool IsInitializing()
    {
        return Instance != null && Instance.isInitializing;
    }
}

