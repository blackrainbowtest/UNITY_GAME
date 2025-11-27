using System;

[Serializable]
public class SubBiomeData
{
    public string id;
    public string parent;
    public float spawnChance; // 0..1
}

[Serializable]
public class SubBiomeDataCollection
{
    public SubBiomeData[] subbiomes;
}
