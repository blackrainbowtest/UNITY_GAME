using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DataRegistry is a global static container for GAME DATA:
/// - Structures (cities, villages, caves, dungeons)
/// - Activities (quests, events, resources)
/// 
/// NOTE:
/// Sub-biomes are NO LONGER LOADED from JSON.
/// They are fully handled by TileGenerator:
///   - biome influence
///   - mask-based transitions
///   - inner blend zones
/// 
/// Biome visual data is now loaded by BiomeDB (not DataRegistry).
/// 
/// DataRegistry should NEVER contain visual data.
/// It only contains pure DATA used by game logic.
/// 
/// Architecture Goal:
/// DataRegistry = GAME DATA
/// BiomeDB     = WORLD VISUAL / GENERATOR DATA
/// TileGenerator / TileRenderer handle visual-generation.
/// </summary>
public static class DataRegistry
{
    // GAME DATA ONLY
    public static Dictionary<string, StructureData> Structures = new();
    public static Dictionary<string, ActivityData> Activities = new();

    public static Dictionary<string, BiomeData> Biomes = new(); 
    // (Optional: keep BiomeData if used elsewhere in game logic, not visuals)

    // ============================================================
    // LOAD ALL JSON DATA FROM RESOURCES
    // ============================================================

    public static void LoadAll()
    {
        LoadBiomes();       // optional
        LoadStructures();
        LoadActivities();
    }

    // ============================================================
    // BIOMES (optional – logic only)
    // ============================================================

    private static void LoadBiomes()
    {
        var json = Resources.Load<TextAsset>("WorldData/biomes");
        if (json == null)
        {
            Debug.LogWarning("[DataRegistry] biomes.json not found.");
            return;
        }

        var col = JsonUtility.FromJson<BiomeDataCollection>(json.text);

        Biomes.Clear();
        foreach (var b in col.biomes)
            Biomes[b.id] = b;
    }

    // ============================================================
    // STRUCTURES (cities, villages, caves, ruins...)
    // ============================================================

    private static void LoadStructures()
    {
        var json = Resources.Load<TextAsset>("WorldData/structures");
        if (json == null)
        {
            Debug.LogWarning("[DataRegistry] structures.json not found.");
            return;
        }

        var col = JsonUtility.FromJson<StructureDataCollection>(json.text);

        Structures.Clear();
        foreach (var s in col.structures)
            Structures[s.id] = s;
    }

    // ============================================================
    // ACTIVITIES (events, quests, interactions)
    // ============================================================

    private static void LoadActivities()
    {
        var json = Resources.Load<TextAsset>("WorldData/activities");
        if (json == null)
        {
            Debug.LogWarning("[DataRegistry] activities.json not found.");
            return;
        }

        var col = JsonUtility.FromJson<ActivityDataCollection>(json.text);

        Activities.Clear();
        foreach (var a in col.activities)
            Activities[a.id] = a;
    }
}
