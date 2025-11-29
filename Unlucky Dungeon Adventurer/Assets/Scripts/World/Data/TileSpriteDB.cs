using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSpriteDB", menuName = "World/TileSpriteDB")]
public class TileSpriteDB : ScriptableObject
{
    [Header("List of all tile sprites (biomes, subbiomes, structures, icons...)")]
    public List<Entry> entries = new List<Entry>();

    [System.Serializable]
    public struct Entry
    {
        public string id;       // Example: forest_01, sub_forest_26, sub_forest_inner_2, structure_village, activity_event
        public Sprite sprite;   // The PNG assigned to this ID
    }

    private Dictionary<string, Sprite> dict;


    // ================================================================
    // Initialize dictionary (lazy loading)
    // ================================================================
    private void EnsureMap()
    {
        if (dict != null) return;

        dict = new Dictionary<string, Sprite>();

        foreach (var e in entries)
        {
            if (!string.IsNullOrEmpty(e.id) && e.sprite != null)
            {
                dict[e.id] = e.sprite;
            }
        }
    }


    // ================================================================
    // Main sprite access method
    // ================================================================
    public Sprite Get(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        EnsureMap();

        if (dict.TryGetValue(id, out Sprite found))
            return found;

        // If exact match does not exist, try fallback options:
        // Example: sub_forest_26 → fallback sub_forest
        // Example: forest_03 → fallback forest

        Sprite fallback = GetFallback(id);
        return fallback;
    }


    // ================================================================
    // Fallback logic for missing sprite IDs (AAA feature)
    // Helps avoid empty tiles even if some IDs not yet drawn.
    // ================================================================
    private Sprite GetFallback(string id)
    {
        // 1) Try reduce detail:
        // sub_forest_26 → sub_forest
        int underscore = id.LastIndexOf('_');
        if (underscore > 0)
        {
            string shortened = id.Substring(0, underscore);
            if (dict.TryGetValue(shortened, out Sprite fb1))
                return fb1;
        }

        // 2) Try biome root:
        // forest_03 → forest
        underscore = id.IndexOf('_');
        if (underscore > 0)
        {
            string biomeRoot = id.Substring(0, underscore);
            if (dict.TryGetValue(biomeRoot, out Sprite fb2))
                return fb2;
        }

        // 3) No fallback found — return null
        return null;
    }


    // ================================================================
    // Utility: Check if ID exists
    // ================================================================
    public bool Has(string id)
    {
        EnsureMap();
        return dict.ContainsKey(id);
    }

    public List<string> GetAllVariants(string prefix)
    {
        List<string> result = new List<string>();

        foreach (var e in entries)
        {
            if (e.id.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
                result.Add(e.id);
        }

        return result;
    }

    public List<string> GetVariants(string baseId)
    {
        List<string> list = new List<string>();

        foreach (var e in entries)
        {
            if (e.id.StartsWith(baseId, StringComparison.OrdinalIgnoreCase))
                list.Add(e.id);
        }

        return list;
    }


    // ================================================================
    // Debug helper: return all IDs
    // ================================================================
    public IEnumerable<string> GetAllIds()
    {
        EnsureMap();
        return dict.Keys;
    }
}
