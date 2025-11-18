═══════════════════════════════════════════════════════════════════════
  СИСТЕМА ИНИЦИАЛИЗАЦИИ ИГРЫ - Инструкция по использованию
═══════════════════════════════════════════════════════════════════════

✅ ЧТО БЫЛО ИСПРАВЛЕНО:
─────────────────────────
• Устранена race condition при загрузке PlayerData
• Гарантирован правильный порядок инициализации
• Предотвращены null reference exceptions в UI


📊 ПОТОК ИНИЦИАЛИЗАЦИИ (в правильном порядке):
─────────────────────────────────────────────────

1. SaveSlotUI.Load()
   └─→ Устанавливает TempSaveCache.pendingSave
   └─→ Вызывает SceneLoader.LoadScene(sceneName)

2. LoadingScene загружается
   └─→ Показывается экран загрузки

3. Целевая сцена загружается (например, "Dungeon")
   └─→ Awake() → GameInitializer создаёт singleton
   └─→ Start() → GameInitializer запускает корутину InitializeGame()

4. GameInitializer.InitializeGame() (корутина):
   
   Фаза 1: Загрузка данных
   ├─ Если TempSaveCache.pendingSave != null
   │  └─ Загружает SaveData через GameManager.LoadGameData()
   └─ Иначе загружает из PlayerPrefs через GameData.LoadPlayer()
   
   Фаза 2: Задержка
   └─ yield return null (ждёт один кадр для инициализации других скриптов)
   
   Фаза 3: Уведомления
   ├─ UIEvents.OnGameInitialized?.Invoke()
   └─ UIEvents.OnPlayerStatsChanged?.Invoke()

5. GameUIController.Start()
   └─ Подписывается на OnPlayerStatsChanged
   └─ Если IsInitialized() == true, вызывает UpdateUI()

6. GameUIController получает событие OnPlayerStatsChanged
   └─ Обновляет UI барчики с корректными данными


🎯 КЛЮЧЕВЫЕ КОМПОНЕНТЫ:
────────────────────────

GameInitializer (DontDestroyOnLoad singleton):
  • Создаёт себя автоматически через [RuntimeInitializeOnLoadMethod]
  • IsInitialized() - проверить, готова ли игра
  • IsInitializing() - проверить, идёт ли процесс

GameManager:
  • LoadGameData(SaveData) - загружает и применяет данные
  • GetCurrentGameData() - получает текущее состояние для сохранения

GameData:
  • CurrentPlayer - глобальный игрок
  • LoadPlayer() - загружает из PlayerPrefs
  • SavePlayer(name, role) - сохраняет в PlayerPrefs

TempSaveCache:
  • pendingSave - временное хранилище для SaveData при загрузке сцены

UIEvents:
  • OnGameInitialized - срабатывает когда инициализация завершена
  • OnPlayerStatsChanged - срабатывает когда нужно обновить UI


📋 ИСПОЛЬЗОВАНИЕ В КОДЕ:
─────────────────────────

// Проверить, готова ли игра
if (GameInitializer.IsInitialized())
{
    Debug.Log("Игра готова!");
}

// Дождаться инициализации
UIEvents.OnGameInitialized += MyInitializationCode;

// Обновить UI при изменении статов
UIEvents.OnPlayerStatsChanged += UpdateMyUI;

// Сохранить текущую игру
SaveData data = GameManager.Instance.GetCurrentGameData();
SaveManager.Save(data, slotIndex);


⚠️ ВАЖНЫЕ ЗАМЕЧАНИЯ:
─────────────────────

1. GameInitializer создаётся автоматически - не нужно добавлять вручную!
   (благодаря [RuntimeInitializeOnLoadMethod])

2. Никогда не устанавливайте CurrentPlayer напрямую в UI скриптах!
   Используйте события вместо этого.

3. Если UI не обновляется - проверьте подписку на OnPlayerStatsChanged

4. SaveSlotUI.Load() ДОЛЖНА устанавливать TempSaveCache.pendingSave перед загрузкой сцены
   (она уже это делает в коде)


🔄 ВСЯ ЛОГИКА ГОНКИ ДАННЫХ УСТРАНЕНА:
───────────────────────────────────────
✓ Нет параллельной инициализации
✓ Гарантирован один кадр задержки перед уведомлениями
✓ UI получит событие ПОСЛЕ полной инициализации CurrentPlayer
✓ Нет null reference exceptions

═══════════════════════════════════════════════════════════════════════
