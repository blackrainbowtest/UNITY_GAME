using UnityEngine;
using System.Collections.Generic;

public static class DataRegistry
{
    public static Dictionary<string, BiomeData> Biomes = new();
    public static Dictionary<string, SubBiomeData> SubBiomes = new();
    public static Dictionary<string, StructureData> Structures = new();

    public static void LoadAll()
    {
        LoadBiomes();
        LoadSubBiomes();
        LoadStructures();
    }

    private static void LoadBiomes()
    {
        var json = Resources.Load<TextAsset>("WorldData/biomes");
        var col = JsonUtility.FromJson<BiomeDataCollection>(json.text);

        Biomes.Clear();
        foreach (var b in col.biomes)
            Biomes[b.id] = b;
    }

    private static void LoadSubBiomes()
    {
        var json = Resources.Load<TextAsset>("WorldData/subbiomes");
        var col = JsonUtility.FromJson<SubBiomeDataCollection>(json.text);

        SubBiomes.Clear();
        foreach (var sb in col.subbiomes)
            SubBiomes[sb.id] = sb;
    }

    private static void LoadStructures()
    {
        var json = Resources.Load<TextAsset>("WorldData/structures");
        var col = JsonUtility.FromJson<StructureDataCollection>(json.text);

        Structures.Clear();
        foreach (var s in col.structures)
            Structures[s.id] = s;
    }
}
