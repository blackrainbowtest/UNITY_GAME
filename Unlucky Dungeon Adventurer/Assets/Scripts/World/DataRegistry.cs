using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DataRegistry — единый источник данных для:
/// - SubBiomeData
/// - StructureData
/// - ActivityData
///
/// Биомы НЕ загружаются здесь, а загружаются через BiomeDB.
/// 
/// JSON-файлы (опциональные):
/// Resources/WorldData/subbiomes.json
/// Resources/WorldData/structures.json
/// Resources/WorldData/activities.json
/// </summary>
public static class DataRegistry
{
    private static bool loaded = false;

    public static readonly Dictionary<string, SubBiomeData> SubBiomes = new();
    public static readonly Dictionary<string, StructureData> Structures = new();
    public static readonly Dictionary<string, ActivityData> Activities = new();

    // ============================================================
    // Public API
    // ============================================================

    public static SubBiomeData GetSubBiome(string id)
        => (id != null && SubBiomes.TryGetValue(id, out var sb)) ? sb : null;

    public static StructureData GetStructure(string id)
        => (id != null && Structures.TryGetValue(id, out var st)) ? st : null;

    public static ActivityData GetActivity(string id)
        => (id != null && Activities.TryGetValue(id, out var ac)) ? ac : null;

    public static void EnsureLoaded()
    {
        if (loaded) return;

        LoadSubBiomes();
        LoadStructures();
        LoadActivities();

        loaded = true;
    }

    // ============================================================
    // Loaders
    // ============================================================

    private static void LoadSubBiomes()
    {
        TextAsset json = Resources.Load<TextAsset>("WorldData/subbiomes");
        if (json == null)
        {
            // Debug.LogWarning("[DataRegistry] subbiomes.json not found — optional.");
            return;
        }

        try
        {
            var list = JsonUtility.FromJson<SubBiomeDataList>(json.text);
            if (list?.subbiomes != null)
            {
                foreach (var s in list.subbiomes)
                {
                    if (!string.IsNullOrEmpty(s.id))
                        SubBiomes[s.id] = s;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[DataRegistry] subbiomes.json parse FAILED: " + e.Message);
        }
    }

    private static void LoadStructures()
    {
        TextAsset json = Resources.Load<TextAsset>("WorldData/structures");
        if (json == null)
        {
            // Debug.LogWarning("[DataRegistry] structures.json not found — optional.");
            return;
        }

        try
        {
            var list = JsonUtility.FromJson<StructureDataList>(json.text);
            if (list?.structures != null)
            {
                foreach (var s in list.structures)
                {
                    if (!string.IsNullOrEmpty(s.id))
                        Structures[s.id] = s;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[DataRegistry] structures.json parse FAILED: " + e.Message);
        }
    }

    private static void LoadActivities()
    {
        TextAsset json = Resources.Load<TextAsset>("WorldData/activities");
        if (json == null)
        {
            // Debug.LogWarning("[DataRegistry] activities.json not found — optional.");
            return;
        }

        try
        {
            var list = JsonUtility.FromJson<ActivityDataCollection>(json.text);
            if (list?.activities != null)
            {
                foreach (var a in list.activities)
                {
                    if (!string.IsNullOrEmpty(a.id))
                        Activities[a.id] = a;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[DataRegistry] activities.json parse FAILED: " + e.Message);
        }
    }
}
