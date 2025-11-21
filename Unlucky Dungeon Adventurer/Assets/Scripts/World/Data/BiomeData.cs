using System;
using UnityEngine;

[Serializable]
public class BiomeData
{
    public string id;
    public string group;
    public string color;
}

[Serializable]
public class BiomeDataCollection
{
    public BiomeData[] biomes;
}
