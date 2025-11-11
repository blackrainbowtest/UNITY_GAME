using System.IO;
using UnityEngine;

public static class SaveManager
{
    // путь до файла сохранения
    private static string GetPath(int slot)
    {
        string fileName = slot == -1 ? "save_auto.json" : $"save_{slot}.json";
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    // 💾 Сохранение
    public static void Save(SaveData data, int slot)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(slot), json);
        Debug.Log($"Game saved to slot {slot}");
    }

    // 📂 Загрузка
    public static SaveData Load(int slot)
    {
        string path = GetPath(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save slot {slot} not found!");
            return null;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"Game loaded from slot {slot}");
        return data;
    }
}
