using System;
using UnityEngine;

[Serializable]
public class TileData
{
    public int x;
    public int y;

    // BIOME (BASE LAYER)
    public string biomeId;          // лес, пустыня, тундра
    public string biomeSpriteId;    // forest_01, desert_02 (вариантность)

    // TILESET EDGE (TRANSITION)
    public string edgeBiome;        // какой биом "вторгается" (dominant)
    public byte edgeMask;           // mask 0–255 для tileset lookup

    // STRUCTURE & ACTIVITY (пока не трогаем)
    public string structureId;
    public string activityId;

    // GAMEPLAY
    public float moveCost;
    public float eventChance;
    public float goodEventChance;
    public float badEventChance;

    // fallback minimap color
    public Color color;

    public TileData(int x, int y)
    {
        this.x = x;
        this.y = y;

        edgeMask = 0;
        edgeBiome = null;
    }
}
