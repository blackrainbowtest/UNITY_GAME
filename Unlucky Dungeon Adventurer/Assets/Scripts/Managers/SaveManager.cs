using System;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    public static void Save(SaveData data, int slotIndex)
    {
        if (data == null)
        {
            Debug.LogError("Пустые данные сохранения");
            return;
        }

        data.meta.slotIndex = slotIndex;

        string fileName = slotIndex < 0
            ? "save_auto.json"
            : $"save_{slotIndex}.json";

        string path = Path.Combine(Application.persistentDataPath, fileName);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log($"[SaveManager] Сохранено в {path}");
    }

    public static SaveData Load(int slotIndex)
    {
        string fileName = slotIndex < 0
            ? "save_auto.json"
            : $"save_{slotIndex}.json";

        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveManager] Файл не найден: {path}");
            return null;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        Debug.Log($"[SaveManager] Загрузка из {path}");
        return data;
    }

    public static void SaveAuto(SaveData data)
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, "save_auto.json");

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            Debug.Log("[AUTO SAVE] Автосейв выполнен: " + path);
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка автосохранения: " + ex.Message);
        }
    }
}
