using System.Collections.Generic;

public static class BiomePower
{
    private static readonly Dictionary<string, int> power = new()
    {
        {"forest", 5},
        {"desert", 4},
        {"tundra", 3},
        {"mountains", 2},
        {"cave", 1},
        {"plains", 0},
    };

    public static int GetPower(string biomeId)
    {
        if (!power.ContainsKey(biomeId)) return 0;
        return power[biomeId];
    }
}
