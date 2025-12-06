using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using WorldLogic;

/// <summary>
/// Imports unique location defs from JSON into ScriptableObjects.
/// </summary>
public static class UniqueLocationImporter
{
    private const string jsonPath = "Assets/Data/unique_locations.json";
    private const string outputFolder = "Assets/Resources/WorldData/UniqueLocations";

    [System.Serializable]
    private class Entry
    {
        public string id;
        public string icon;
        public string biome;
        public int rarity;
        public bool mustBeNearMountains;
        public bool mustBeNearWater;
        public string lore;
    }

    [MenuItem("Tools/Import Unique Locations")]
    public static void Import()
    {
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("[Importer] JSON file not found: " + jsonPath);
            return;
        }

        string raw = File.ReadAllText(jsonPath);

        // JsonUtility needs an object root, so wrap the array
        string wrapped = "{ \"items\": " + raw + " }";

        var container = JsonUtility.FromJson<Wrapper>(wrapped);
        if (container == null || container.items == null)
        {
            Debug.LogError("[Importer] Failed to parse JSON!");
            return;
        }

        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        foreach (var entry in container.items)
        {
            string assetPath = $"{outputFolder}/{entry.id}.asset";

            UniqueLocationDef def = AssetDatabase.LoadAssetAtPath<UniqueLocationDef>(assetPath);
            if (def == null)
            {
                def = ScriptableObject.CreateInstance<UniqueLocationDef>();
                AssetDatabase.CreateAsset(def, assetPath);
            }

            // Fill SO fields from JSON
            def.id = entry.id;
            def.requiredBiome = entry.biome;
            def.rarity = entry.rarity;
            def.nearMountains = entry.mustBeNearMountains;
            def.nearWater = entry.mustBeNearWater;

            // Localization keys (store keys, not localized text)
            def.nameKey = entry.id + ".name";
            def.descriptionKey = entry.id + ".description";

            // Load icon if provided
            if (!string.IsNullOrEmpty(entry.icon))
            {
                def.icon = Resources.Load<Sprite>("Icons/" + entry.icon);
            }

            EditorUtility.SetDirty(def);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[Importer] Imported {container.items.Count} unique locations");
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<Entry> items;
    }
}
