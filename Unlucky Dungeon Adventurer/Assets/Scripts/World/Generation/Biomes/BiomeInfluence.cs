using System;
using System.Collections.Generic;

public static class BiomeInfluence
{
    public static string GetDominantNeighbor(
        Func<int, int, string> biomeGetter,
        int x, int y,
        string centerBiome)
    {
        Dictionary<string,int> score = new();

        // 8 направлений
        int[,] dirs =
        {
            {0,1}, {0,-1}, {-1,0}, {1,0},
            {-1,1}, {1,1}, {-1,-1}, {1,-1}
        };

        for (int i = 0; i < dirs.GetLength(0); i++)
        {
            int nx = x + dirs[i,0];
            int ny = y + dirs[i,1];
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

        // return key with max score
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
