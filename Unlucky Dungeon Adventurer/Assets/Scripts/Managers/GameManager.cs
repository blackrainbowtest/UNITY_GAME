using UnityEngine;
using UnityEngine.SceneManagement;

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

    public SaveData GetCurrentGameData()
    {
        SaveData data = new SaveData();
        var p = GameData.CurrentPlayer;

        if (p != null)
        {
            // --- Player ---
            data.player.name = p.playerName;
            data.player.playerClass = p.playerClass;

            data.player.level = p.level;
            data.player.gold = p.gold;

            // 🔥 сохраняем HP/MP/STA
            data.player.currentHP = p.currentHP;
            data.player.maxHP = p.maxHP;

            data.player.currentMP = p.currentMP;
            data.player.maxMP = p.maxMP;

            data.player.currentStamina = p.currentStamina;
            data.player.maxStamina = p.maxStamina;

            // остальное
            data.player.attack = p.attack;
            data.player.defense = p.defense;
            data.player.agility = p.agility;
            data.player.lust = p.lust;
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
        data.meta.saveVersion = "0.1";

        return data;
    }


    public void LoadGameData(SaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Нет данных для загрузки");
            return;
        }

        // --- Создаём PlayerData на основе класса ---
        ClassStats stats;

        if (!GameData.classDatabase.TryGetValue(data.player.playerClass, out stats))
        {
            Debug.LogWarning($"Неизвестный класс: {data.player.playerClass}. Загружаю базовый класс.");
            stats = new ClassStats();
        }

        // Создаём объект PlayerData на основе базовых статов
        PlayerData p = new PlayerData(
            data.player.name,
            data.player.playerClass,
            stats
        );

        // --- Перезаписываем сохранённые характеристики (🔥 важная часть) ---

        p.level = data.player.level;
        p.gold = data.player.gold;

        // max/current HP/MP/Stamina
        p.maxHP = data.player.maxHP;
        p.currentHP = data.player.currentHP;

        p.maxMP = data.player.maxMP;
        p.currentMP = data.player.currentMP;

        p.maxStamina = data.player.maxStamina;
        p.currentStamina = data.player.currentStamina;

        // боевые характеристики
        p.attack = data.player.attack;
        p.defense = data.player.defense;
        p.agility = data.player.agility;
        p.lust = data.player.lust;
        p.isPregnant = data.player.isPregnant;

        // координаты карты
        p.mapPosX = data.player.mapPosX;
        p.mapPosY = data.player.mapPosY;

        // сохраняем в GameData
        GameData.CurrentPlayer = p;

        // --- World (когда подключим систему мира) ---
        // TODO: восстановить день/время/погоду/seed

        Debug.Log($"Загружен персонаж: {p.playerName} [{p.playerClass}] | Уровень {p.level}");
    }


    private void Start()
    {
        if (TempSaveCache.pendingSave != null)
        {
            LoadGameData(TempSaveCache.pendingSave);
            TempSaveCache.pendingSave = null;
        }
    }
}
