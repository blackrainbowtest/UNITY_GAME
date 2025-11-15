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

        // --- Player ---
        if (GameData.CurrentPlayer != null)
        {
            data.player.name = GameData.CurrentPlayer.playerName;
            data.player.playerClass = GameData.CurrentPlayer.playerClass;
            data.player.level = GameData.CurrentPlayer.level;
            data.player.gold = GameData.CurrentPlayer.gold;

            data.player.hp = GameData.CurrentPlayer.hp;
            data.player.mp = GameData.CurrentPlayer.mp;
            data.player.attack = GameData.CurrentPlayer.attack;
            data.player.defense = GameData.CurrentPlayer.defense;
            data.player.agility = GameData.CurrentPlayer.agility;
            data.player.lust = GameData.CurrentPlayer.lust;
            data.player.isPregnant = GameData.CurrentPlayer.isPregnant;

            // TODO: когда будет реальная позиция на WorldMap — сюда
            data.player.mapPosX = 0f;
            data.player.mapPosY = 0f;
        }

        // --- World ---
        // Позже будем забирать отсюда генерацию мира, время суток и т.п.
        data.world.worldSeed = 0;
        data.world.currentDay = 1;
        data.world.timeOfDay = 12.0f;

        // --- Inventory ---
        // Пока оставляем пустым, позже подключим InventorySystem.Export()

        // --- Quests ---
        // Тоже пока заглушка. Потом: QuestSystem.Export()

        // --- Meta ---
        data.meta.sceneName = SceneManager.GetActiveScene().name;
        data.meta.saveTime = System.DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        data.meta.saveVersion = "0.1"; // версию можно будет обновлять
        // slotIndex мы будем проставлять в SaveManager.Save()

        return data;
    }

    public void LoadGameData(SaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Нет данных для загрузки");
            return;
        }

        // --- Player ---
        // тут мы одну из двух вещей можем делать:
        // 1) создать нового CurrentPlayer из SaveData
        // 2) обновить уже существующего
        // Пока сделаем новый:

        ClassStats stats;
        if (GameData.classDatabase.TryGetValue(data.player.playerClass, out stats))
        {
            GameData.CurrentPlayer = new PlayerData(
                data.player.name,
                data.player.playerClass,
                stats
            );

            // Перезаписываем боевые статы теми, что в сейве
            GameData.CurrentPlayer.level = data.player.level;
            GameData.CurrentPlayer.gold = data.player.gold;
            GameData.CurrentPlayer.hp = data.player.hp;
            GameData.CurrentPlayer.mp = data.player.mp;
            GameData.CurrentPlayer.attack = data.player.attack;
            GameData.CurrentPlayer.defense = data.player.defense;
            GameData.CurrentPlayer.agility = data.player.agility;
            GameData.CurrentPlayer.lust = data.player.lust;
            GameData.CurrentPlayer.isPregnant = data.player.isPregnant;
        }
        else
        {
            Debug.LogWarning($"Неизвестный класс в сейве: {data.player.playerClass}");
        }

        // --- World ---
        // Здесь потом будем возвращать позицию на карте, день, время и т.п.

        Debug.Log($"Загружен {data.player.name} ({data.player.playerClass}), уровень {data.player.level}, золото {data.player.gold}");
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
