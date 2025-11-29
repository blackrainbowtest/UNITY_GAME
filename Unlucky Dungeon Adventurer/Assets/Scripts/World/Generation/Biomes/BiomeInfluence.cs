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

        // Только 4 стороны — идеально для tileset переходов
        int[,] dirs =
        {
            { 0,  1 },   // N
            { 0, -1 },   // S
            { -1, 0 },   // W
            { 1,  0 }    // E
        };

        for (int i = 0; i < dirs.GetLength(0); i++)
        {
            int nx = x + dirs[i, 0];
            int ny = y + dirs[i, 1];

            string nb = biomeGetter(nx, ny);

            if (nb == null || nb == centerBiome)
                continue;

            int power = BiomePower.GetPower(nb);

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
