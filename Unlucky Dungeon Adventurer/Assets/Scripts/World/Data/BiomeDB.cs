using System.Collections.Generic;
using UnityEngine;

public static class BiomeDB
{
    private static Dictionary<string, BiomeConfig> biomes;
    private static bool loaded = false;

    public static void EnsureLoaded()
    {
        if (loaded) return;

        TextAsset json = Resources.Load<TextAsset>("WorldData/biomes");
        if (json == null)
        {
            Debug.LogError("[BiomeDB] biomes.json not found in Resources/WorldData/");
            biomes = new Dictionary<string, BiomeConfig>();
            loaded = true;
            return;
        }

        var col = JsonUtility.FromJson<BiomeConfigCollection>(json.text);

        biomes = new Dictionary<string, BiomeConfig>();
        foreach (var b in col.biomes)
        {
            if (!string.IsNullOrEmpty(b.id))
                biomes[b.id] = b;
        }

        loaded = true;
    }

    public static BiomeConfig GetBiome(string id)
    {
        EnsureLoaded();

        if (string.IsNullOrEmpty(id))
            return null;

        biomes.TryGetValue(id, out var cfg);
        return cfg;
    }

    public static bool HasBiome(string id)
    {
        EnsureLoaded();
        return biomes.ContainsKey(id);
    }
}
