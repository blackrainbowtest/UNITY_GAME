using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class LanguageManager
{
    private static Dictionary<string, string> texts;
    public static string CurrentLanguage = "ru";

    public static void LoadLanguage(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "lng", CurrentLanguage, fileName + ".json");
        // UDADebug.Log("[LanguageManager] Load: " + path);

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
            // UDADebug.Log($"[LanguageManager] Loaded {texts.Count} keys");
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

    public static string GetFormat(string key, params object[] args)
    {
        string format = Get(key);
        return string.Format(format, args);
    }

    private static Dictionary<string, string> ParseSimpleJson(string json)
    {
        var dict = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(json)) return dict;

        var matches = Regex.Matches(json, @"""(?<k>[^""]+)""\s*:\s*""(?<v>[^""]*)""");
        foreach (Match m in matches)
        {
            string key = m.Groups["k"].Value;
            dict[key] = m.Groups["v"].Value;
        }

        return dict;
    }
}

