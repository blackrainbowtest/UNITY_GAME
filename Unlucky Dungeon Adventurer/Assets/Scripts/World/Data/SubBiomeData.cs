using System;

[Serializable]
public class SubBiomeData
{
    public string id;
    public string parent;
    public float spawnChance;
}

[Serializable]
public class SubBiomeDataList
{
    public SubBiomeData[] subbiomes;
}
