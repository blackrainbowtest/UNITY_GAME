using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WorldLogic;

namespace WorldLogic.Cities
{
    public static class CityNameDatabase
    {
        private static Dictionary<string, List<string>> namesByBiome = new();
        private static Dictionary<string, List<string>> descByBiome = new();
        private static bool loaded;

        public static void Load()
        {
            if (loaded) return;

            string lang = LanguageManager.CurrentLanguage ?? "ru";
            string pathNames = Path.Combine(Application.streamingAssetsPath, "lng", lang, "city_names.json");
            string pathDescs = Path.Combine(Application.streamingAssetsPath, "lng", lang, "city_descriptions.json");

            if (!File.Exists(pathNames))
            {
                Debug.LogError($"[CityNameDatabase] File not found: {pathNames}");
                namesByBiome = new();
                loaded = true;
                // continue to try descriptions
            }
            try
            {
                if (File.Exists(pathNames))
                {
                    string jsonNames = File.ReadAllText(pathNames);
                    namesByBiome = JsonUtilityWrapper.FromCityNamesJson(jsonNames);
                    Debug.Log($"[CityNameDatabase] Loaded city names for {namesByBiome.Count} biomes.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CityNameDatabase] Names parse error: {ex.Message}");
                namesByBiome = new();
            }

            try
            {
                if (File.Exists(pathDescs))
                {
                    string jsonDescs = File.ReadAllText(pathDescs);
                    descByBiome = JsonUtilityWrapper.FromCityNamesJson(jsonDescs);
                    Debug.Log($"[CityNameDatabase] Loaded city descriptions for {descByBiome.Count} biomes.");
                }
                else
                {
                    descByBiome = new();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CityNameDatabase] Descriptions parse error: {ex.Message}");
                descByBiome = new();
            }

            loaded = true;
        }

        public static string GetName(string biomeId, int seed, int index)
        {
            Load();

            if (string.IsNullOrEmpty(biomeId))
                biomeId = "unknown";

            if (!namesByBiome.TryGetValue(biomeId, out var list) || list == null || list.Count == 0)
                return "Unknown City";

            int hash = DeterministicHash.Hash(seed, index);
            int nameIndex = Mathf.Abs(hash) % list.Count;
            return list[nameIndex];
        }

        public static string GetDescription(string biomeId, int seed, int index)
        {
            Load();

            if (string.IsNullOrEmpty(biomeId))
                biomeId = "unknown";

            if (!descByBiome.TryGetValue(biomeId, out var list) || list == null || list.Count == 0)
                return "";

            int hash = DeterministicHash.Hash(seed, index * 97 + 13);
            int i = Mathf.Abs(hash) % list.Count;
            return list[i];
        }
    }
}
