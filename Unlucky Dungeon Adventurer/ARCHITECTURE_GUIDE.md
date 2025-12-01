# 📘 Unlucky Dungeon Adventurer — Architecture Guide

**Дата создания**: 2025-12-01  
**Версия**: 2.0  
**Автор**: UDA Team

---

## 📑 Содержание

1. [Обзор проекта](#обзор-проекта)
2. [Архитектура кода](#архитектура-кода)
3. [Системы игры](#системы-игры)
   - [Система инициализации](#1-система-инициализации-gameinitializer)
   - [Генерация мира](#2-генерация-мира-worldgenerator--tilegenerator)
   - [Система движения](#3-система-движения-playermovementcontroller)
   - [Система отдыха](#4-система-отдыха-restcontroller)
   - [Система инвентаря](#5-система-инвентаря-inventorycontroller)
   - [Система сохранения](#6-система-сохранения-savemanager)
   - [UI системы](#7-ui-системы)
4. [Модели данных](#модели-данных)
5. [Структура папок](#структура-папок)
6. [Важные паттерны](#важные-паттерны)
7. [Гайды и референсы](#гайды-и-референсы)

---

## Обзор проекта

**Unlucky Dungeon Adventurer** — процедурная RPG с открытым миром, где игрок путешествует по сгенерированной карте, взаимодействует с биомами, собирает предметы, выполняет квесты и участвует в боях.

### Основные особенности:
- **Процедурная генерация мира** с плавными переходами между биомами
- **Система перемещения** с расходом стамины и временем
- **Система отдыха** с событиями и окружением
- **Динамический инвентарь** с редкостью, сортировкой и экипировкой
- **Система сохранений** с автосохранением и слотами
- **Прогрессия персонажа** через опыт и классы

---

## Архитектура кода

Проект следует **MVC-подобной архитектуре** с разделением на:

### **MODEL** (Данные)
- `SaveData`, `PlayerSaveData`, `WorldSaveData` — структуры сохранений
- `TileData`, `BiomeConfig`, `ItemDefinition` — игровые данные
- `GameData`, `ItemDatabase`, `BiomeDB` — статические базы данных

### **VIEW** (Отображение)
- `TileRenderer` — отрисовка тайлов мира
- `MinimapRenderer` — отрисовка миникарты
- `InventoryUIController` — UI инвентаря
- `GameUIController` — главный UI игры

### **CONTROLLER** (Логика)
- `WorldMapController` — управление картой мира
- `PlayerMovementController` — управление движением игрока
- `InventoryController` — логика инвентаря
- `RestController` — логика отдыха
- `GameManager` — центральный менеджер игры

---

## Системы игры

### 1. Система инициализации (`GameInitializer`)

**Цель**: Гарантировать правильный порядок инициализации всех систем и избежать race conditions.

#### Поток инициализации:
```
SaveSlotUI.Load() → TempSaveCache.pendingSave
    ↓
Scene loads
    ↓
GameInitializer.Awake() → создаёт singleton
    ↓
GameInitializer.Start() → запускает InitializeGame() корутину
    ↓
Phase 1: Проверка TempSaveCache (загрузка сохранения)
Phase 2: Проверка PlayerPrefs (загрузка игрока)
Phase 3: Ожидание 1 кадра (для завершения Awake/Start других скриптов)
Phase 4: Вызов событий (OnGameInitialized, OnPlayerStatsChanged)
    ↓
GameUIController.Start() → подписывается на события
    ↓
UI обновляется
```

#### Ключевые файлы:
- `Scripts/Core/GameInitializer.cs` — координатор инициализации
- `Scripts/Managers/GameManager.cs` — singleton, хранит текущий SaveData
- `Scripts/Managers/TempSaveCache.cs` — временное хранилище сохранения

#### Методы:
```csharp
GameInitializer.IsInitialized() // true если инициализация завершена
GameInitializer.IsInitializing() // true если инициализация идёт
```

---

### 2. Генерация мира (`WorldGenerator` + `TileGenerator`)

**Цель**: Процедурно генерировать бесконечный мир с плавными переходами биомов.

#### Архитектура:
- **WorldGenerator** — MODEL, хранит кэш тайлов, отвечает за доступ к данным
- **TileGenerator** — генерирует `TileData` для координат (x, y)
- **TileRenderer** — VIEW, отрисовывает тайл на экране
- **WorldMapController** — CONTROLLER, управляет видимыми тайлами

#### Процесс генерации тайла:
```csharp
TileData tile = TileGenerator.GenerateTile(x, y, worldSeed);

// 1. Выбор базового биома (Perlin Noise)
string biomeId = ChooseBiomeId(x, y, worldSeed);

// 2. Выбор варианта спрайта биома (forest_01, forest_02, ...)
tile.biomeSpriteId = PickBiomeVariantSpriteId(biomeId, rng);

// 3. Определение перехода между биомами (edge blending)
string dominant = BiomeInfluence.GetDominantNeighbor(...);
byte mask = BiomeMaskUtils.GetMask(...);
tile.edgeBiome = dominant;
tile.edgeMask = mask;

// 4. Генерация структур (города, подземелья, etc.)
tile.structureId = null; // пока не реализовано

// 5. Установка gameplay-статов (moveCost, eventChance)
tile.moveCost = biome.moveCost * noiseFactor;
tile.eventChance = biome.eventChance * noiseFactor;
```

#### Система переходов между биомами:
См. **[SUBBIOME_MASK_GUIDE.md](Assets/Resources/WorldData/SUBBIOME_MASK_GUIDE.md)** для детального объяснения масок.

**Краткое описание:**
- Маска (0-255) кодирует направления чужих биомов вокруг тайла
- Каждый бит = одно из 8 направлений (N, NE, E, SE, S, SW, W, NW)
- Спрайт `sub_{biome}_{mask}` используется для плавного перехода

Примеры:
- `mask = 2` (TOP) → граница сверху
- `mask = 16` (LEFT) → граница слева
- `mask = 255` → полное окружение другим биомом

#### Ключевые файлы:
- `Scripts/World/Generation/WorldGenerator.cs`
- `Scripts/World/Generation/TileGenerator.cs`
- `Scripts/World/Generation/Biomes/BiomeInfluence.cs`
- `Scripts/World/Generation/Biomes/BiomeMaskUtils.cs`
- `Scripts/World/Rendering/TileRenderer.cs`

---

### 3. Система движения (`PlayerMovementController`)

**Цель**: Управление перемещением игрока по карте с расходом стамины и времени.

#### Поток движения:
```
1. Игрок кликает на тайл
    ↓
2. WorldMapController.HandleTileClick() → PreparePathTo(target)
    ↓
3. Pathfinding.FindPath(start, target) → A* алгоритм
    ↓
4. PathCostCalculator.GetStaminaCost(path) → расчёт стоимости
    ↓
5. PathRenderer.Show(path) → визуализация пути
    ↓
6. UIEvents.OnPathPreview → обновление UI (показать стоимость)
    ↓
7. Игрок нажимает "Walk"
    ↓
8. StartWalk() → WalkRoutine() корутина
    ↓
9. Для каждого тайла:
   - PlayerStatsController.ModifyStamina(-cost)
   - MovementTimeController.ApplyTime(minutes)
   - MovementEventResolver.ProcessTileEvent()
   - Анимация движения
    ↓
10. UIEvents.OnMovementEnded → обновление UI
```

#### Расчёт стоимости:
```csharp
// Стамина
int cost = 0;
foreach (var tile in path) {
    float moveCost = TileGenerator.GetTileMoveCost(tile.x, tile.y);
    cost += Mathf.CeilToInt(moveCost);
}

// Время
int totalMinutes = path.Count * minutesPerTile; // ~30 мин на тайл
```

#### События движения:
```csharp
MovementEventResolver.ProcessTileEvent(Vector2Int tile)
// → случайные встречи, находки, ловушки
```

#### Ключевые файлы:
- `Scripts/World/Player/PlayerMovementController.cs`
- `Scripts/World/Pathfinding.cs`
- `Scripts/World/Player/PathCostCalculator.cs`
- `Scripts/World/Player/MovementTimeController.cs`
- `Scripts/World/Player/MovementEventResolver.cs`
- `Scripts/World/Path/PathRenderer.cs`

---

### 4. Система отдыха (`RestController`)

**Цель**: Позволить игроку восстанавливать ресурсы (HP, MP, Stamina) с учётом окружения и событий.

#### Типы отдыха:
```csharp
public enum RestType
{
    ShortRest,    // Короткий отдых (30 мин)
    Meditation,   // Медитация (60 мин)
    LongSleep     // Долгий сон (8 часов)
}
```

#### Окружение:
```csharp
public enum RestEnvironment
{
    SafeCity,      // Город (безопасно)
    SafeCamp,      // Лагерь (относительно безопасно)
    Wilderness,    // Дикая местность (опасно)
    Dungeon        // Подземелье (очень опасно)
}
```

#### Поток отдыха:
```
1. Игрок нажимает "Rest"
    ↓
2. RestEnvironmentDetector определяет окружение
    ↓
3. RestUIController.Open(environment) → показывает окно выбора
    ↓
4. Игрок выбирает тип отдыха
    ↓
5. RestController.StartRest(type, environment)
    ↓
6. RestEventResolver.RollEvent(type, environment) → определяет событие
    ↓
7. Обработка события:
   - None: нормальный отдых
   - Noise: отдых с штрафом (меньше восстановления)
   - MinorAmbush: лёгкое нападение (UI выбор)
   - MajorAmbush: сильное нападение (переход в бой)
    ↓
8. RestCalculator.ApplyRest(player, type, environment)
    ↓
9. UIEvents.OnPlayerStatsChanged → обновление UI
```

#### Расчёт восстановления:
```csharp
// Короткий отдых
stamina = 30% от макс
HP = 10% от макс

// Медитация
MP = 50% от макс
stamina = 20% от макс

// Долгий сон
HP = 100%
MP = 100%
stamina = 100%
```

#### Ключевые файлы:
- `Scripts/RestSystem/RestController.cs`
- `Scripts/RestSystem/RestUIController.cs`
- `Scripts/RestSystem/RestCalculator.cs`
- `Scripts/RestSystem/RestEventResolver.cs`
- `Scripts/RestSystem/RestEnvironmentDetector.cs`

---

### 5. Система инвентаря (`InventoryController`)

**Цель**: Управление предметами игрока с поддержкой стаков, сортировки, экипировки.

#### Структура предмета:
```csharp
public class ItemDefinition
{
    public string id;           // "sword_iron_001"
    public string type;         // "weapon", "armor", "consumable", "bag"
    public string rarity;       // "common", "uncommon", "rare", "epic", "legendary"
    public int maxStack;        // 1 для оружия, 99 для зелий
    public int capacityBonus;   // для сумок (+10 слотов)
    
    // Боевые статы
    public int attackBonus;
    public int defenseBonus;
    public int hpBonus;
    public int mpBonus;
}

public class ItemInstance
{
    public string id;
    public int quantity;
    public ItemDefinition Def => ItemDatabase.Instance.Get(id);
}
```

#### Основные операции:
```csharp
// Добавление предмета
bool success = InventoryController.Instance.AddItem("potion_health", 5);

// Удаление предмета
InventoryController.Instance.RemoveItem(itemInstance, 1);

// Сортировка
InventoryController.Instance.SortInventory(SortMode.ByRarity);

// Получение вместимости
int capacity = InventoryController.Instance.GetCapacity();
// capacity = baseCapacity + sum(capacityBonus всех сумок)
```

#### Сортировка:
```csharp
public enum SortMode
{
    ByName,      // А-Я
    ByRarity,    // Legendary → Common
    ByType,      // Оружие, Броня, Расходники
    ByQuantity   // От большего к меньшему
}
```

#### Drag & Drop:
- `DragManager` — обработка перетаскивания предметов
- `InventorySlotUI` — визуальный слот инвентаря
- `ItemActionWindow` — контекстное меню (Use, Drop, Equip)

#### Ключевые файлы:
- `Scripts/Inventory/InventoryController.cs`
- `Scripts/Inventory/ItemDatabase.cs`
- `Scripts/Inventory/ItemDefinition.cs`
- `Scripts/Inventory/ItemInstance.cs`
- `Scripts/Inventory/InventorySort.cs`
- `Scripts/Inventory/UI/InventoryUIController.cs`
- `Scripts/Inventory/UI/DragManager.cs`

---

### 6. Система сохранения (`SaveManager`)

**Цель**: Сохранение/загрузка игрового прогресса с поддержкой слотов и автосохранения.

#### Структура сохранения:
```csharp
public class SaveData
{
    public PlayerSaveData player;       // Данные персонажа
    public WorldSaveData world;         // Данные мира (время, день)
    public InventorySaveData inventory; // Инвентарь (deprecated, теперь в player)
    public QuestSaveData quests;        // Квесты
    public MetaSaveData meta;           // Мета-информация (слот, время сохранения)
}

public class PlayerSaveData
{
    // Основное
    public string name;
    public string playerClass;
    public int level;
    public int gold;
    public int worldSeed;
    
    // Прогрессия
    public int experience;
    public int experienceToNext;
    
    // Базовые статы (от класса/уровня)
    public int baseMaxHP;
    public int baseMaxMP;
    public int baseMaxStamina;
    public int baseAttack;
    public int baseDefense;
    public int baseAgility;
    public int baseLust;
    
    // Текущие значения
    public int currentHP;
    public int currentMP;
    public int currentStamina;
    
    // Позиция на карте
    public float mapPosX;
    public float mapPosY;
    
    // Инвентарь
    public List<ItemInstance> inventoryItems;
}

public class WorldSaveData
{
    public int worldSeed;
    public int currentDay;
    public float timeOfDay; // 0.0-23.99
    
    public void AddMinutes(int minutes);
}
```

#### Операции сохранения:
```csharp
// Обычное сохранение (в слот)
SaveManager.Save(saveData, slotIndex);
// → save_0.json, save_1.json, ...

// Автосохранение
SaveManager.SaveAuto(saveData);
// → save_auto.json

// Загрузка
SaveData data = SaveManager.Load(slotPath);

// Удаление
SaveManager.Delete(slotPath);
```

#### Поток сохранения/загрузки:
```
SAVE:
1. GameMenuController.OnSaveClicked()
2. SaveLoadState.Mode = SaveLoadMode.Save
3. LoadScene("SaveLoadScene")
4. SaveLoadSceneManager показывает слоты (включая пустые)
5. SaveSlotUI.OnClick() → SaveManager.Save()
6. Возврат в игру

LOAD:
1. MainMenu.OnLoadClicked() или GameMenuController.OnLoadClicked()
2. SaveLoadState.Mode = SaveLoadMode.Load
3. LoadScene("SaveLoadScene")
4. SaveLoadSceneManager показывает слоты (включая автосохранение)
5. SaveSlotUI.OnClick() → TempSaveCache.pendingSave = loadedData
6. LoadScene("GameScene")
7. GameInitializer применяет TempSaveCache.pendingSave
```

#### Автосохранение:
Автоматически срабатывает при входе на карту мира:
```csharp
// WorldMapController.TryAutoSaveOnEnter()
if (player.worldSeed >= 10000) // Проверка валидности seed
{
    SaveManager.SaveAuto(currentSaveData);
}
```

#### Ключевые файлы:
- `Scripts/Managers/SaveManager.cs`
- `Scripts/Managers/SaveLoadSceneManager.cs`
- `Scripts/Managers/TempSaveCache.cs`
- `Scripts/Managers/SaveLoadState.cs`
- `Scripts/UI/SaveSlotUI.cs`
- `Scripts/Data/SaveData.cs`

---

### 7. UI системы

#### GameUIController
Главный контроллер UI игры. Отображает:
- Имя персонажа, класс, уровень
- HP, MP, Stamina (полоски)
- День и время
- Текущий биом
- Золото

```csharp
UIEvents.OnPlayerStatsChanged += Refresh;
UIEvents.OnGameInitialized += OnGameReady;
```

#### MovementUIController
UI для движения:
- Информация о пути (стамина, время)
- Кнопка "Walk"
- Кнопка "Rest"

```csharp
UIEvents.OnPathPreview += OnPathPreview;
UIEvents.OnMovementStarted += OnMovementStarted;
UIEvents.OnMovementEnded += OnMovementEnded;
UIEvents.OnRestAvailable += OnRestAvailable;
```

#### InventoryUIController
UI инвентаря:
- Слоты (16 базовых + бонусы от сумок)
- Сортировка
- Drag & Drop
- Контекстные меню

#### TooltipController
Система подсказок:
```csharp
TooltipController.Instance.Show(title, description, worldPosition);
TooltipController.Instance.Hide();
```

Используется для:
- Предметов (название, описание, статы)
- Тайлов (название биома, стоимость движения)
- Кнопок (объяснение действий)

#### MinimapController
Миникарта с:
- Рендерингом биомов (цветные пиксели)
- Позицией игрока (белая точка)
- Drag для скролла карты
- Клик для перемещения камеры

#### Ключевые файлы:
- `Scripts/UI/GameUIController.cs`
- `Scripts/UI/MovementUIController.cs`
- `Scripts/UI/GameMenuController.cs`
- `Scripts/Inventory/UI/InventoryUIController.cs`
- `Scripts/UI/Tooltip/TooltipController.cs`
- `Scripts/World/Minimap/MinimapController.cs`

---

## Модели данных

### TileData
```csharp
public class TileData
{
    public int x, y;                    // Координаты
    public string biomeId;              // "forest", "plains", ...
    public string biomeSpriteId;        // "forest_01", "forest_02"
    public string edgeBiome;            // Биом для перехода (если есть)
    public byte edgeMask;               // Маска направлений перехода
    public string structureId;          // ID структуры (город, подземелье)
    
    // Gameplay stats
    public float moveCost;              // Стоимость движения
    public float eventChance;           // Шанс события
    public float goodEventChance;
    public float badEventChance;
    
    public Color color;                 // Цвет на миникарте
}
```

### BiomeConfig
```csharp
public class BiomeConfig
{
    public string id;                   // "forest"
    public string displayName;          // "Лес"
    public string mapColor;             // "#228B22" (hex)
    
    public float moveCost;              // 1.0 = нормально, 2.0 = медленно
    public float eventChance;           // 0.0-1.0
    public float goodChance;            // 0.0-1.0
    public float badChance;             // 0.0-1.0
    
    public List<string> possibleEvents; // ["goblin_ambush", "treasure_chest"]
}
```

### ClassStats
```csharp
public class ClassStats
{
    public string className;
    public int maxHP;
    public int maxMP;
    public int maxStamina;
    public int attack;
    public int defense;
    public int agility;
    public int lust;
}
```

### ClassProgressionEntry
```csharp
public class ClassProgressionEntry
{
    public int level;
    public int experienceRequired;
    public ClassStats stats;
}
```

---

## Структура папок

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameInitializer.cs       // Координатор инициализации
│   │   └── Preloader.cs              // Прелоадер ресурсов
│   │
│   ├── Managers/
│   │   ├── GameManager.cs            // Главный менеджер игры
│   │   ├── SaveManager.cs            // Сохранения/загрузка
│   │   ├── SaveLoadSceneManager.cs   // UI сохранений
│   │   ├── TempSaveCache.cs          // Временный кэш сохранения
│   │   ├── SaveLoadState.cs          // Режим (Save/Load)
│   │   ├── ClassProgressionManager.cs // Прогрессия классов
│   │   └── LanguageManager.cs        // Локализация
│   │
│   ├── Data/
│   │   ├── SaveData.cs               // Структуры сохранений
│   │   ├── PlayerData.cs             // Данные персонажа
│   │   ├── GameData.cs               // Статические данные игры
│   │   ├── ClassStats.cs             // Статы классов
│   │   └── ClassProgression.cs       // Прогрессия
│   │
│   ├── World/
│   │   ├── Controllers/
│   │   │   └── WorldMapController.cs // Главный контроллер карты
│   │   │
│   │   ├── Generation/
│   │   │   ├── WorldGenerator.cs     // Генератор мира
│   │   │   ├── TileGenerator.cs      // Генератор тайлов
│   │   │   └── Biomes/
│   │   │       ├── BiomeInfluence.cs
│   │   │       ├── BiomeMaskUtils.cs
│   │   │       └── BiomePower.cs
│   │   │
│   │   ├── Rendering/
│   │   │   └── TileRenderer.cs       // Отрисовка тайлов
│   │   │
│   │   ├── Player/
│   │   │   ├── PlayerMovementController.cs
│   │   │   ├── PlayerStatsController.cs
│   │   │   ├── PlayerMarkerController.cs
│   │   │   ├── RestController.cs
│   │   │   ├── PathCostCalculator.cs
│   │   │   ├── MovementTimeController.cs
│   │   │   └── MovementEventResolver.cs
│   │   │
│   │   ├── Minimap/
│   │   │   ├── MinimapController.cs
│   │   │   ├── MinimapRenderer.cs
│   │   │   └── MinimapInputHandler.cs
│   │   │
│   │   ├── Data/
│   │   │   ├── BiomeDB.cs            // База данных биомов
│   │   │   ├── TileSpriteDB.cs       // Спрайты тайлов
│   │   │   └── StructureData.cs      // Данные структур
│   │   │
│   │   └── Models/
│   │       ├── TileData.cs
│   │       └── BiomeConfig.cs
│   │
│   ├── RestSystem/
│   │   ├── RestController.cs         // Логика отдыха
│   │   ├── RestUIController.cs       // UI отдыха
│   │   ├── RestCalculator.cs         // Расчёты восстановления
│   │   ├── RestEventResolver.cs      // События отдыха
│   │   ├── RestEnvironmentDetector.cs
│   │   ├── RestEnvironment.cs        // Enum окружений
│   │   ├── RestType.cs               // Enum типов отдыха
│   │   └── RestEvent.cs              // Структура события
│   │
│   ├── Inventory/
│   │   ├── InventoryController.cs
│   │   ├── EquipmentController.cs
│   │   ├── ItemDatabase.cs
│   │   ├── ItemDefinition.cs
│   │   ├── ItemInstance.cs
│   │   ├── ItemIconDatabase.cs
│   │   ├── InventorySort.cs
│   │   └── UI/
│   │       ├── InventoryUIController.cs
│   │       ├── InventorySlotUI.cs
│   │       ├── DragManager.cs
│   │       ├── ItemActionWindow.cs
│   │       └── RarityColors.cs
│   │
│   ├── UI/
│   │   ├── GameUIController.cs       // Главный UI
│   │   ├── MovementUIController.cs   // UI движения
│   │   ├── GameMenuController.cs     // Меню паузы
│   │   ├── SaveSlotUI.cs             // Слот сохранения
│   │   ├── SimpleBar.cs              // Полоски HP/MP/Stamina
│   │   └── Tooltip/
│   │       ├── TooltipController.cs
│   │       └── TooltipTrigger.cs
│   │
│   ├── Events/
│   │   └── UIEvents.cs               // Система событий UI
│   │
│   ├── CharacterCreator/
│   │   ├── CharacterCreatorUI.cs
│   │   ├── CharacterOutfitManager.cs
│   │   └── OutfitUtils.cs
│   │
│   └── Loading/
│       ├── LoadingScreen.cs
│       └── SceneLoader.cs
│
├── Resources/
│   ├── WorldData/
│   │   ├── biomes.json               // Конфиги биомов
│   │   └── SUBBIOME_MASK_GUIDE.md    // Гайд по маскам
│   │
│   ├── Items/
│   │   └── items.json                // База предметов
│   │
│   └── Classes/
│       └── class_progression.json    // Прогрессия классов
│
├── StreamingAssets/
│   └── class_progression.json        // Копия для билда
│
└── Scenes/
    ├── MainMenu                      // Главное меню
    ├── CharacterCreator              // Создание персонажа
    ├── GameScene                     // Игровая сцена (карта мира)
    └── SaveLoadScene                 // Экран сохранений
```

---

## Важные паттерны

### 1. Singleton Pattern
Используется для глобального доступа к системам:
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
```

**Применяется в:**
- `GameManager`
- `GameInitializer`
- `InventoryController`
- `TooltipController`
- `RestController`

### 2. Event System Pattern
Слабосвязанная коммуникация между системами:
```csharp
public static class UIEvents
{
    public static System.Action OnPlayerStatsChanged;
    public static System.Action OnGameInitialized;
    public static System.Action<int, int, bool> OnPathPreview;
    public static System.Action OnMovementStarted;
    public static System.Action OnMovementEnded;
    public static System.Action<bool> OnRestAvailable;
    
    public static void InvokePlayerStatsChanged() 
        => OnPlayerStatsChanged?.Invoke();
}
```

### 3. Static Database Pattern
Централизованный доступ к данным:
```csharp
public static class BiomeDB
{
    private static Dictionary<string, BiomeConfig> biomes;
    
    public static void EnsureLoaded() { /* load from JSON */ }
    public static BiomeConfig GetBiome(string id) => biomes[id];
}
```

**Применяется в:**
- `BiomeDB`
- `ItemDatabase`
- `ClassProgressionManager`
- `TileSpriteDB`

### 4. MVC Pattern
Разделение ответственности:
```
MODEL (данные)         → TileData, SaveData
VIEW (отображение)     → TileRenderer, InventoryUIController
CONTROLLER (логика)    → WorldMapController, InventoryController
```

### 5. Coroutine Pattern
Асинхронные операции:
```csharp
private IEnumerator WalkRoutine()
{
    _isMoving = true;
    
    foreach (var tile in path)
    {
        yield return MoveTo(tile);
        ProcessTileEvents();
    }
    
    _isMoving = false;
}
```

---

## Гайды и референсы

### Детальные гайды:
1. **[SUBBIOME_MASK_GUIDE.md](Assets/Resources/WorldData/SUBBIOME_MASK_GUIDE.md)**  
   Полное объяснение системы масок для плавных переходов биомов

### JSON конфиги:
1. **biomes.json** — конфигурация всех биомов
2. **items.json** — база данных предметов
3. **class_progression.json** — прогрессия классов по уровням

### Дополнительные документы:
- `CHANGES_SUMMARY.txt` — история изменений системы инициализации
- Встроенные комментарии в коде (XML-doc стиль)

---

## Частые задачи

### Добавить новый биом:
1. Добавить запись в `biomes.json`
2. Создать спрайты: `biome_id_01.png`, `biome_id_02.png`, ...
3. Создать спрайты переходов: `sub_biome_id_2.png`, `sub_biome_id_8.png`, ...
4. Обновить `TileGenerator.ChooseBiomeId()` с условием для нового биома

### Добавить новый предмет:
1. Добавить запись в `items.json`
2. Создать иконку в `Resources/Items/Icons/`
3. Зарегистрировать иконку в `ItemIconDatabase`

### Добавить новое событие отдыха:
1. Добавить тип события в `RestEventType` enum
2. Обновить `RestEventResolver.RollEvent()`
3. Обновить `RestController.StartRest()` с обработкой

### Добавить новый класс:
1. Добавить прогрессию в `class_progression.json`
2. Добавить `ClassOption` в `CharacterCreatorUI`
3. Обновить `GameData.classDatabase`

---

## Известные проблемы и TODO

### Текущие ограничения:
- ❌ Структуры на карте не реализованы (города, подземелья)
- ❌ Боевая система не реализована
- ❌ Квесты не реализованы
- ❌ Торговля не реализована
- ⚠️ События движения базовые (нужно больше разнообразия)
- ⚠️ События отдыха частично реализованы (засады не показывают UI)

### Планируемые улучшения:
- 🔜 Система структур (генерация городов, подземелий)
- 🔜 Пошаговая боевая система
- 🔜 Система квестов
- 🔜 NPC и диалоги
- 🔜 Торговля и магазины
- 🔜 Система крафта

---

## Контакты и поддержка

**Email**: unluckydungeonadventure@gmail.com  
**GitHub**: [blackrainbowtest/UNITY_GAME](https://github.com/blackrainbowtest/UNITY_GAME)

---

**Дата обновления**: 2025-12-01  
**Версия документа**: 2.0
