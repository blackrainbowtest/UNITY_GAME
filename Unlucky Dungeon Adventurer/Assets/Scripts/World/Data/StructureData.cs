using System;

/*
spawnChance
rarity
minDistanceBetween
maxPerBiome
loot table
enemy list
*/

[Serializable]
public class StructureData
{
    public string id;
    public string category;
}

[Serializable]
public class StructureDataCollection
{
    public StructureData[] structures;
}
