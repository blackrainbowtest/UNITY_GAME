using UnityEngine;

[System.Serializable]
public class BiomeConfig
{
    public string id;

    public float moveCost;
    public float eventChance;
    public float goodChance;
    public float badChance;

    public string mapColor;
}

[System.Serializable]
public class BiomeConfigCollection
{
    public BiomeConfig[] biomes;
}
