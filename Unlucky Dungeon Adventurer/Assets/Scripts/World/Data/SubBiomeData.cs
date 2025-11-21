using System;

[Serializable]
public class SubBiomeData
{
    public string id;
    public string parent;
}

[Serializable]
public class SubBiomeDataCollection
{
    public SubBiomeData[] subbiomes;
}
