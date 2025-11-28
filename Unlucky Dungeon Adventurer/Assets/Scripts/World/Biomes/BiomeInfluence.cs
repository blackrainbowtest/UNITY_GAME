public static class BiomeInfluence
{
    public static string GetDominantNeighbor(
        Func<int, int, string> biomeGetter,
        int x, int y,
        string centerBiome)
    {
        string bestBiome = null;
        int bestPower = -1;

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

            if (nb != null && nb != centerBiome)
            {
                int p = BiomePower.GetPower(nb);
                if (p > bestPower)
                {
                    bestPower = p;
                    bestBiome = nb;
                }
            }
        }

        return bestBiome;
    }
}
