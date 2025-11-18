using UnityEngine;
using System.Collections;

/// <summary>
/// Управляет инициализацией игры и загрузкой игрока.
/// Предотвращает race conditions при загрузке SaveData.
/// 
/// ПОРЯДОК ИНИЦИАЛИЗАЦИИ:
/// 1. SaveSlotUI.Load() → устанавливает TempSaveCache.pendingSave
/// 2. Загружается новая сцена
/// 3. GameInitializer.Awake() → создаёт singleton
/// 4. GameInitializer.Start() → запускает корутину InitializeGame()
///    - Фаза 1: Проверяет TempSaveCache
///    - Фаза 2: Если нужно — загружает из PlayerPrefs
///    - Фаза 3: Ждёт один кадр для инициализации других скриптов
///    - Фаза 4: Отправляет события OnGameInitialized и OnPlayerStatsChanged
/// 5. GameUIController.Start() → проверяет IsInitialized() и подписывается на события
/// 6. GameUIController получает событие → обновляет UI
/// </summary>
public class GameInitializer : MonoBehaviour
{
    public static GameInitializer Instance { get; private set; }

    private bool isInitialized = false;
    private bool isInitializing = false;

    /// <summary>
    /// Гарантирует существование GameInitializer в сцене.
    /// Вызовите это, если нужно принудительно создать инициализатор.
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
        // Создаём singleton, если его нет
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameInitializer] Создан и отмечен как DontDestroyOnLoad");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Если не инициализировались — инициализируемся
        if (!isInitialized && !isInitializing)
        {
            StartCoroutine(InitializeGame());
        }
    }

    /// <summary>
    /// Корутина инициализации: гарантирует правильный порядок загрузки
    /// </summary>
    private IEnumerator InitializeGame()
    {
        isInitializing = true;

        Debug.Log("[GameInitializer] Начинаю инициализацию...");

        // Фаза 1: Проверяем, есть ли ожидающее сохранение
        if (TempSaveCache.pendingSave != null)
        {
            Debug.Log("[GameInitializer] Обнаружено сохранение в TempSaveCache");
            GameManager.Instance.LoadGameData(TempSaveCache.pendingSave);
            TempSaveCache.pendingSave = null;
        }
        // Фаза 2: Если игрока нет — пытаемся загрузить из PlayerPrefs
        else if (GameData.CurrentPlayer == null)
        {
            Debug.Log("[GameInitializer] Загружаю игрока из PlayerPrefs...");
            GameData.LoadPlayer();

            if (GameData.CurrentPlayer == null)
            {
                Debug.Log("[GameInitializer] PlayerPrefs пуст, создаю пустого игрока");
                GameData.CurrentPlayer = new PlayerData("", "Hermit", new ClassStats());
            }
        }
        else
        {
            Debug.Log("[GameInitializer] CurrentPlayer уже инициализирован");
        }

        // Фаза 3: Ждём один кадр, чтобы гарантировать инициализацию всех скриптов
        yield return null;

        // Фаза 4: Отправляем события об инициализации
        Debug.Log("[GameInitializer] Инициализация завершена, уведомляю UI...");
        
        // Сначала сообщаем что инициализация завершена
        UIEvents.OnGameInitialized?.Invoke();
        
        // Затем обновляем UI с текущими статами
        UIEvents.OnPlayerStatsChanged?.Invoke();

        isInitialized = true;
        isInitializing = false;
    }

    /// <summary>
    /// Проверка: инициализирована ли игра
    /// </summary>
    public static bool IsInitialized()
    {
        return Instance != null && Instance.isInitialized;
    }

    /// <summary>
    /// Проверка: идёт ли процесс инициализации
    /// </summary>
    public static bool IsInitializing()
    {
        return Instance != null && Instance.isInitializing;
    }
}
