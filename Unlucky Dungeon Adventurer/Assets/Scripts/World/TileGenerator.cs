using System;
using UnityEngine;

public static class TileGenerator
{
	/// <summary>
	/// Main method: Given (x,y) and the world seed, returns a tile.
	/// </summary>
    public static TileData GenerateTile(int x, int y, int worldSeed)
    {
        BiomeDB.EnsureLoaded();

        // Deterministic randomness for a specific tile:
        int hash = HashCoords(x, y, worldSeed);
        System.Random rng = new System.Random(hash);

        // 1. Selecting a biome based on Perlin noise
        string biomeId = ChooseBiomeId(x, y, worldSeed);
        BiomeConfig biome = BiomeDB.GetBiome(biomeId);

        // If suddenly the biome is not found, there is a backup
        if (biome == null)
        {
            biomeId = "plains"; // default
            biome = BiomeDB.GetBiome(biomeId);
        }

        // 2. Select a sub-biome (if any)
        string subBiomeId = null;
        SubBiomeConfig subBiome = null;

        if (biome != null && biome.subbiomes != null && biome.subbiomes.Length > 0)
        {
            int idx = rng.Next(0, biome.subbiomes.Length);
            subBiomeId = biome.subbiomes[idx];
            subBiome = BiomeDB.GetSubBiome(subBiomeId);
        }

        // 3. Fill TileData
        TileData tile = new TileData();
        tile.x = x;
        tile.y = y;
        tile.biomeId = biomeId;
        tile.subBiomeId = subBiomeId;
        tile.structureId = null;              // later: cities, caves, etc.

        // Basic parameters from the biome + some noise
        float noiseFactor = 0.8f + (float)rng.NextDouble() * 0.4f; // 0.8..1.2

        tile.moveCost = (biome?.moveCost ?? 1f) * noiseFactor;
        tile.eventChance = (biome?.eventChance ?? 0.1f) * noiseFactor;
        tile.goodEventChance = biome?.goodChance ?? 0.5f;
        tile.badEventChance = biome?.badChance ?? 0.5f;

        // Color for minimap: priority subbiome, otherwise biome
        string colorHex = subBiome?.mapColor ?? biome?.mapColor ?? "#777777";
        tile.color = HexToColor(colorHex);

        // Sprite: subbiome priority, otherwise biome id
        tile.spriteId = subBiome?.spriteId ?? biomeId;

        return tile;
    }

	/// <summary>
    /// Выбор биома по Perlin noise (очень простое правило, его легко поменять потом).
    /// </summary>
    private static string ChooseBiomeId(int x, int y, int worldSeed)
    {
        float scale = 0.025f; // чем меньше, тем “более плавные” области
        float nx = (x + worldSeed * 0.13f) * scale;
        float ny = (y - worldSeed * 0.07f) * scale;

        float n = Mathf.PerlinNoise(nx, ny); // 0..1

        // Примитивная схема распределения
        if (n < 0.18f) return "desert";
        if (n < 0.35f) return "plains";
        if (n < 0.65f) return "forest";
        if (n < 0.82f) return "mountains";
        if (n < 0.95f) return "tundra";

        // Крайние участки — шанс пещер / чего-то особенного
        return "cave";
    }

    private static int HashCoords(int x, int y, int seed)
    {
        unchecked
        {
            int h = seed;
            h = h * 73856093 ^ x;
            h = h * 19349663 ^ y;
            return h;
        }
    }

    private static Color HexToColor(string hex)
    {
        if (string.IsNullOrEmpty(hex))
            return Color.magenta;

        if (hex[0] == '#')
            hex = hex.Substring(1);

        if (hex.Length != 6)
            return Color.magenta;

        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color32(r, g, b, 255);
    }







    // ========================================================
    // Auxiliary methods
    // ========================================================

    // Hash to make the seed unique for each cell
    private static int Hash(int x, int y, int seed)
    {
        unchecked
        {
            int h = seed;
            h = h * 31 + x;
            h = h * 31 + y;
            return h;
        }
    }

    private static string PickBiome(int seed)
    {
        var list = DataRegistry.Biomes;
        int index = Mathf.Abs(seed) % list.Count;
        int i = 0;
        foreach (var kv in list)
        {
            if (i == index) return kv.Key;
            i++;
        }
        return "Forest";
    }

    private static string PickSubBiome(string biomeId, int seed)
    {
        // Search for all subbiomes with parent == biomeId
        var list = DataRegistry.SubBiomes;
        var matches = new System.Collections.Generic.List<string>();

        foreach (var sb in list.Values)
        {
            if (sb.parent == biomeId)
                matches.Add(sb.id);
        }

        if (matches.Count == 0)
            return null;

        return matches[Mathf.Abs(seed) % matches.Count];
    }

    private static string PickStructure(int seed)
    {
        // 90% - nothing
        if ((seed % 1000) < 900)
            return null;

        var list = DataRegistry.Structures;
        int index = Mathf.Abs(seed) % list.Count;

        int i = 0;
        foreach (var kv in list)
        {
            if (i == index) return kv.Key;
            i++;
        }
        return null;
    }
}
