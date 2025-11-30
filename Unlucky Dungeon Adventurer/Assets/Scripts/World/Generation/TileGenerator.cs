using System;
using System.Collections.Generic;
using UnityEngine;

public static class TileGenerator
{
    private static TileSpriteDB _spriteDB;
    private static readonly Dictionary<string, List<string>> _variantCache = new();

    public static TileData GenerateTile(int x, int y, int worldSeed)
    {
        BiomeDB.EnsureLoaded();

        // Deterministic RNG for variants
        int hash = HashCoords(x, y, worldSeed);
        System.Random rng = new(hash);

        // 1) Base biome
        string biomeId = ChooseBiomeId(x, y, worldSeed);
        BiomeConfig biome = BiomeDB.GetBiome(biomeId);

        if (biome == null)
        {
            biomeId = "plains";
            biome = BiomeDB.GetBiome(biomeId);
        }

        // Create tile
        TileData tile = new TileData(x, y)
        {
            biomeId = biomeId
        };

        // 2) Biome sprite variant (forest_01, _02 ...)
        tile.biomeSpriteId = PickBiomeVariantSpriteId(biomeId, rng);

        // 3) Edge detection (tileset transitions)
        Func<int, int, string> biomeGetter = (ix, iy) => ChooseBiomeId(ix, iy, worldSeed);

        string dominant = BiomeInfluence.GetDominantNeighbor(biomeGetter, x, y, biomeId);

        if (dominant != null && dominant != biomeId)
        {
            // Маска показывает где ДОМИНАНТНЫЙ биом присутствует вокруг этого тайла
            // (бит=1 означает "там dominant биом", бит=0 означает "там другой биом")
            byte mask = BiomeMaskUtils.GetMask(biomeGetter, x, y, dominant);

            if (mask != 0)
            {
                tile.edgeBiome = dominant; // чей tileset использовать
                tile.edgeMask = mask;       // какой кусок tileset нужен
            }
        }

        // 4) Structures
        tile.structureId = null;

        // 5) Gameplay stats
        float noiseFactor = 0.8f + (float)rng.NextDouble() * 0.4f;

        tile.moveCost        = (biome?.moveCost ?? 1f)      * noiseFactor;
        tile.eventChance     = (biome?.eventChance ?? 0.1f) * noiseFactor;
        tile.goodEventChance = biome?.goodChance ?? 0.5f;
        tile.badEventChance  = biome?.badChance ?? 0.5f;

        // Minimap color
        tile.color = HexToColor(biome.mapColor);

        return tile;
    }

    // ------------------------- Helpers -----------------------------

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

    private static string PickBiomeVariantSpriteId(string biomeId, System.Random rng)
    {
        EnsureSpriteDB();
        if (_spriteDB == null)
            return biomeId;

        if (!_variantCache.TryGetValue(biomeId, out var list))
        {
            list = _spriteDB.GetVariants(biomeId + "_");
            _variantCache[biomeId] = list ?? new List<string>();
        }

        if (list != null && list.Count > 0)
        {
            int idx = rng.Next(0, list.Count);
            return list[idx];
        }

        return biomeId;
    }

    private static void EnsureSpriteDB()
    {
        if (_spriteDB == null)
            _spriteDB = Resources.Load<TileSpriteDB>("WorldData/TileSpriteDB");
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
