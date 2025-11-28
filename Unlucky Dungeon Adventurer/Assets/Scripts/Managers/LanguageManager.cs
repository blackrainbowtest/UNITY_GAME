using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class LanguageManager
{
    private static Dictionary<string, string> texts;
    public static string CurrentLanguage = "ru";

    public static void LoadLanguage(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "lng", CurrentLanguage, fileName + ".json");
        // Debug.Log("[LanguageManager] Load: " + path);

        if (!File.Exists(path))
        {
            Debug.LogError("Language file not found!");
            texts = new Dictionary<string, string>();
            return;
        }

        string json = File.ReadAllText(path);

        try
        {
            texts = ParseSimpleJson(json);
            // Debug.Log($"[LanguageManager] Loaded {texts.Count} keys");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Language parse error: " + e.Message);
            texts = new Dictionary<string, string>();
        }
    }

    public static string Get(string key)
    {
        if (texts != null && texts.TryGetValue(key, out string value))
            return value;
        return key;
    }

    // ����� ������� ������, �������� � JSON ������� { "����": "�����" }
    private static Dictionary<string, string> ParseSimpleJson(string json)
    {
        var dict = new Dictionary<string, string>();

        json = json.Trim().TrimStart('{').TrimEnd('}');
        string[] pairs = json.Split(',');

        foreach (var p in pairs)
        {
            string[] kv = p.Split(':');
            if (kv.Length == 2)
            {
                string key = kv[0].Trim().Trim('"');
                string value = kv[1].Trim().Trim('"');
                dict[key] = value;
            }
        }

        return dict;
    }
}
