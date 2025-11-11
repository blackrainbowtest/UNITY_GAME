using UnityEngine;

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

        if (GameData.CurrentPlayer != null)
        {
            data.playerName = GameData.CurrentPlayer.playerName;   // ✅ исправлено
            data.playerClass = GameData.CurrentPlayer.playerClass;   // ✅
            data.level = GameData.CurrentPlayer.level;         // ✅
            data.gold = GameData.CurrentPlayer.gold;          // ✅
            data.mapPosition = Vector2.zero;                         // временно
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

        Debug.Log($"Загружен {data.playerName} ({data.playerClass}), уровень {data.level}");
    }
}
