using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Debug.Log("[GameManager] Awake — Singleton готов");
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

    public SaveData GetCurrentGameData()
    {
        SaveData data = new SaveData();
        var p = GameData.CurrentPlayer;

        if (p != null)
        {
            data.player.name = p.playerName;
            data.player.playerClass = p.playerClass;
            data.player.worldSeed = p.worldSeed;
            Debug.Log($"[GameManager] Preparing save: player.worldSeed={p.worldSeed}");

            data.player.level = p.level;
            data.player.gold = p.gold;

            // базовые статы
            data.player.baseMaxHP = p.baseMaxHP;
            data.player.baseMaxMP = p.baseMaxMP;
            data.player.baseMaxStamina = p.baseMaxStamina;

            data.player.baseAttack = p.baseAttack;
            data.player.baseDefense = p.baseDefense;
            data.player.baseAgility = p.baseAgility;
            data.player.baseLust = p.baseLust;

            // текущие значения
            data.player.currentHP = p.currentHP;
            data.player.currentMP = p.currentMP;
            data.player.currentStamina = p.currentStamina;

            data.player.isPregnant = p.isPregnant;

            data.player.mapPosX = p.mapPosX;
            data.player.mapPosY = p.mapPosY;
        }

        // --- World ---
        data.world.worldSeed = 0;
        data.world.currentDay = 1;
        data.world.timeOfDay = 12f;

        // --- Inventory ---
        // оставим заглушку

        // --- Quests ---
        // тоже пусто пока

        // --- Meta ---
        data.meta.sceneName = SceneManager.GetActiveScene().name;
        data.meta.saveTime = System.DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        data.meta.saveVersion = "0.1.0021";

        // Save biome at player's current position (only for WorldMap scene)
        if (data.meta.sceneName == "WorldMap" && p != null)
        {
            int px = Mathf.FloorToInt(p.mapPosX);
            int py = Mathf.FloorToInt(p.mapPosY);
            int seed = p.worldSeed;
            string biomeId = null;
            if (seed >= 10000)
            {
                biomeId = TileGenerator.GenerateTile(px, py, seed)?.biomeId;
            }
            data.meta.currentBiome = biomeId ?? "unknown";
        }

        return data;
    }


    public void LoadGameData(SaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Нет данных для загрузки");
            return;
        }

        // получаем базовый шаблон класса
        ClassStats stats;
        if (!GameData.classDatabase.TryGetValue(data.player.playerClass, out stats))
        {
            Debug.LogWarning($"Неизвестный класс: {data.player.playerClass}. Загружаю базовый класс.");
            stats = new ClassStats();
        }

        // создаём PlayerData с базовыми статами из класса
        PlayerData p = new PlayerData(
            data.player.name,
            data.player.playerClass,
            stats
        );
        p.worldSeed = data.player.worldSeed;
        Debug.Log($"[GameManager] Loaded player.worldSeed from save: {data.player.worldSeed} -> assigned to p.worldSeed={p.worldSeed}");

        // перезаписываем тем, что есть в сейве
        p.level = data.player.level;
        p.gold = data.player.gold;

        // базовые статы из сейва (если они уже менялись по уровню и т.п.)
        p.baseMaxHP = data.player.baseMaxHP;
        p.baseMaxMP = data.player.baseMaxMP;
        p.baseMaxStamina = data.player.baseMaxStamina;

        p.baseAttack = data.player.baseAttack;
        p.baseDefense = data.player.baseDefense;
        p.baseAgility = data.player.baseAgility;
        p.baseLust = data.player.baseLust;

        // текущее состояние
        p.currentHP = data.player.currentHP;
        p.currentMP = data.player.currentMP;
        p.currentStamina = data.player.currentStamina;

        p.isPregnant = data.player.isPregnant;

        p.mapPosX = data.player.mapPosX;
        p.mapPosY = data.player.mapPosY;

        // пересчёт финальных статов (finalMaxHP и т.п.)
        p.RecalculateFinalStats();

        GameData.CurrentPlayer = p;

        Debug.Log($"Загружен персонаж: {p.playerName} [{p.playerClass}] | Уровень {p.level}");
    }


    private void Start()
    {
        // GameInitializer будет управлять инициализацией
        // GameManager только предоставляет методы для загрузки/сохранения
        Debug.Log("[GameManager] Start — готов к работе");
    }
}
