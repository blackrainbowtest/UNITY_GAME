using System;
using System.Collections.Generic;
using UnityEngine;

public static class TileGenerator
{
    public static TileData GenerateTile(int x, int y, int worldSeed)
    {
        BiomeDB.EnsureLoaded();

        // Deterministic RNG for tile
        int hash = HashCoords(x, y, worldSeed);
        System.Random rng = new System.Random(hash);

        // 1. Determine biome
        string biomeId = ChooseBiomeId(x, y, worldSeed);
        BiomeConfig biome = BiomeDB.GetBiome(biomeId);

        if (biome == null)
        {
            biomeId = "plains";
            biome = BiomeDB.GetBiome(biomeId);
        }

        // 2. Create tile
        TileData tile = new TileData(x, y);
        tile.biomeId = biomeId;

        // Select random biome sprite variant (biome_01..05)
        int variant = rng.Next(1, 6);
        tile.biomeSpriteId = $"{biomeId}_{variant:00}";

        // Prepare sub-biomes list
        tile.subBiomeIds = new List<string>();

        // 3. Dominant neighboring biome
        // Wrap ChooseBiomeId (нужен worldSeed) в лямбду без третьего аргумента
        Func<int,int,string> biomeGetter = (ix, iy) => ChooseBiomeId(ix, iy, worldSeed);
        string dominant = BiomeInfluence.GetDominantNeighbor(
            biomeGetter,
            x, y,
            biomeId
        );

        if (dominant != null)
        {
            // Mask
            byte mask = BiomeMaskUtils.GetMask(
                biomeGetter,
                x, y,
                biomeId
            );

            tile.biomeMask = mask;

            // Main transition tile: sub_forest_26
            string subId = $"sub_{dominant}_{mask}";
            tile.subBiomeIds.Add(subId);

            // Transition zone (2–3 tiles)
            AddBlendZone(tile, x, y, biomeId, dominant, worldSeed);
        }

        // 4. Structures (later)
        tile.structureId = null;

        // 5. Gameplay stats
        float noiseFactor = 0.8f + (float)rng.NextDouble() * 0.4f;

        tile.moveCost = (biome?.moveCost ?? 1f) * noiseFactor;
        tile.eventChance = (biome?.eventChance ?? 0.1f) * noiseFactor;
        tile.goodEventChance = biome?.goodChance ?? 0.5f;
        tile.badEventChance = biome?.badChance ?? 0.5f;

        // 6. Minimap color
        tile.color = HexToColor(biome.mapColor);

        return tile;
    }

    private static void AddBlendZone(
        TileData tile,
        int x, int y,
        string centerBiome,
        string dominant,
        int seed)
    {
        System.Random rng = new System.Random(seed * (x + 2137) * (y + 9157));

        int radius = rng.Next(2, 4); // zone 2–3 tiles

        for (int i = 1; i <= radius; i++)
        {
            float density = 1f - (i / (radius + 1f));

            if (rng.NextDouble() < density)
            {
                string layered = $"sub_{dominant}_inner_{i}";
                tile.subBiomeIds.Add(layered);
            }
        }
    }

    private static string ChooseBiomeId(int x, int y, int worldSeed)
    {
        float scale = 0.025f;
        float nx = (x + worldSeed * 0.13f) * scale;
        float ny = (y - worldSeed * 0.07f) * scale;

        float n = Mathf.PerlinNoise(nx, ny);

        if (n < 0.18f) return "desert";
        if (n < 0.35f) return "plains";
        if (n < 0.65f) return "forest";
        if (n < 0.82f) return "mountains";
        if (n < 0.95f) return "tundra";
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
}
