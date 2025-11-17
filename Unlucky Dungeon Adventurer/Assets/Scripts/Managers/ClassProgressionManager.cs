using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ClassProgressionManager
{
    public static Dictionary<string, ClassProgressionEntry> Data;

    static ClassProgressionManager()
    {
        Load();
    }

    public static void Load()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "class_progression.json");

        if (!File.Exists(path))
        {
            Debug.LogError("Файл class_progression.json не найден!");
            return;
        }

        string json = File.ReadAllText(path);
        var wrapper = JsonUtility.FromJson<ClassProgressionDataWrapper>(json);

        Data = wrapper.ToDictionary();
    }

    [System.Serializable]
    private class ClassProgressionDataWrapper
    {
        public ClassProgressionEntry Paladin;
        public ClassProgressionEntry Hermit;
        public ClassProgressionEntry Rogue;
        public ClassProgressionEntry Slave;

        public Dictionary<string, ClassProgressionEntry> ToDictionary()
        {
            return new Dictionary<string, ClassProgressionEntry>()
            {
                { "Paladin", Paladin },
                { "Hermit", Hermit },
                { "Rogue", Rogue },
                { "Slave", Slave }
            };
        }
    }
}
