using System;
using UnityEngine;

[Serializable]
public class TileData
{
    public int x;
    public int y;

    // Main
    public string biomeId;
    public string subBiomeId;

    // TODO: Structures (e.g. village, bandit camp, cave)
    public string structureId;

    // Геймплейные свойства
    public float moveCost;          // How much time/energy does it take to take a step?
    public float eventChance;     // chance of an event on a tile
    public float goodEventChance; // chance of a good event
    public float badEventChance;  // chance of a bad event

    public Color color;           // tile color (for minimap)
	public string spriteId;       // PNG recourse key

    // Empty constructor
    public TileData() { }

    public TileData(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
