using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BiomeConfig
{
    public string id;
    public string[] subbiomes;

    public float moveCost;
    public float eventChance;
    public float goodChance;
    public float badChance;

    public string mapColor;   // "#RRGGBB"
}

[Serializable]
public class BiomeConfigList
{
    public BiomeConfig[] biomes;
}

[Serializable]
public class SubBiomeConfig
{
    public string id;
    public string parent;     // id base biom
    public string spriteId;
    public string mapColor;   // "#RRGGBB"
}

[Serializable]
public class SubBiomeConfigList
{
    public SubBiomeConfig[] subbiomes;
}

/// <summary>
/// Single access point to biome/subbiome data from JSON.
/// Waiting for files:
/// Resources/biomes.json
/// Resources/subbiomes.json
/// </summary>
public static class BiomeDB
{
    private static bool _loaded;

    private static Dictionary<string, BiomeConfig> _biomesById;
    private static Dictionary<string, SubBiomeConfig> _subById;

    public static void EnsureLoaded()
    {
        if (_loaded) return;

        _biomesById = new Dictionary<string, BiomeConfig>();
        _subById = new Dictionary<string, SubBiomeConfig>();

        // --- Biomes ---
        TextAsset biomesJson = Resources.Load<TextAsset>("biomes");
        if (biomesJson == null)
        {
            Debug.LogError("[BiomeDB] biomes.json not found in Resources/");
        }
        else
        {
            try
            {
                var list = JsonUtility.FromJson<BiomeConfigList>(biomesJson.text);
                if (list?.biomes != null)
                {
                    foreach (var b in list.biomes)
                    {
                        if (!string.IsNullOrEmpty(b.id))
                            _biomesById[b.id] = b;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[BiomeDB] Failed to parse biomes.json: " + e.Message);
            }
        }

        // --- SubBiomes ---
        TextAsset subJson = Resources.Load<TextAsset>("subbiomes");
        if (subJson == null)
        {
            Debug.LogWarning("[BiomeDB] subbiomes.json not found in Resources/ (optional)");
        }
        else
        {
            try
            {
                var list = JsonUtility.FromJson<SubBiomeConfigList>(subJson.text);
                if (list?.subbiomes != null)
                {
                    foreach (var s in list.subbiomes)
                    {
                        if (!string.IsNullOrEmpty(s.id))
                            _subById[s.id] = s;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[BiomeDB] Failed to parse subbiomes.json: " + e.Message);
            }
        }

        _loaded = true;
    }

    public static BiomeConfig GetBiome(string id)
    {
        EnsureLoaded();
        if (id != null && _biomesById.TryGetValue(id, out var b))
            return b;
        return null;
    }

    public static SubBiomeConfig GetSubBiome(string id)
    {
        EnsureLoaded();
        if (id != null && _subById.TryGetValue(id, out var sb))
            return sb;
        return null;
    }

    public static IEnumerable<BiomeConfig> AllBiomes
    {
        get
        {
            EnsureLoaded();
            return _biomesById.Values;
        }
    }
}
