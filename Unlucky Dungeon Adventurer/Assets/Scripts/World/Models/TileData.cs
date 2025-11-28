using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileData
{
    public int x;
    public int y;

    // biome & sub-biomes
    public string biomeId;            // основной биом
    public string biomeSpriteId;      // случайный вариант спрайта биома

    public List<string> subBiomeIds;  // подбиомы по уровням силы
    public byte biomeMask;            // битовая маска соседей (0-255)

    // Structures
    public string structureId;

    // Gameplay
    public float moveCost;
    public float eventChance;
    public float goodEventChance;
    public float badEventChance;

    // Visual fallback
    public Color color;

    public TileData(int x, int y)
    {
        this.x = x;
        this.y = y;

        subBiomeIds = new List<string>();
    }
}