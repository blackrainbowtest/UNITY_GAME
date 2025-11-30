using System;
using System.Collections.Generic;

public static class BiomeInfluence
{
    public static string GetDominantNeighbor(
        Func<int, int, string> biomeGetter,
        int x, int y,
        string centerBiome)
    {
        Dictionary<string, int> score = new();

        // 8 направлений: 4 кардинальных (полный вес) + 4 диагонали (меньший вес)
        // Формат: dx, dy, weight
        (int dx, int dy, int weight)[] dirs =
        {
            ( 0,  1, 10),   // N  (UP)
            ( 0, -1, 10),   // S  (DOWN)
            (-1,  0, 10),   // W  (LEFT)
            ( 1,  0, 10),   // E  (RIGHT)
            (-1,  1,  5),   // NW (UP_LEFT)
            ( 1,  1,  5),   // NE (UP_RIGHT)
            (-1, -1,  5),   // SW (DOWN_LEFT)
            ( 1, -1,  5)    // SE (DOWN_RIGHT)
        };

        foreach (var (dx, dy, weight) in dirs)
        {
            int nx = x + dx;
            int ny = y + dy;

            string nb = biomeGetter(nx, ny);

            if (nb == null || nb == centerBiome)
                continue;

            int power = BiomePower.GetPower(nb) * weight;

            if (!score.ContainsKey(nb))
                score[nb] = 0;

            score[nb] += power;
        }

        if (score.Count == 0)
            return null;

        string bestBiome = null;
        int bestValue = -1;

        foreach (var kv in score)
        {
            if (kv.Value > bestValue)
            {
                bestValue = kv.Value;
                bestBiome = kv.Key;
            }
        }

        return bestBiome;
    }
}
