using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator
{
    private int worldSeed;

    // Store only modified tiles:
    private Dictionary<Vector2Int, TileData> modifiedTiles;

    public WorldGenerator(int worldSeed)
    {
        this.worldSeed = worldSeed;
        modifiedTiles = new Dictionary<Vector2Int, TileData>();
    }

    // Tile request
    public TileData GetTile(int x, int y)
    {
        var key = new Vector2Int(x, y);

        // If the tile is in the modified ones, return it
        if (modifiedTiles.TryGetValue(key, out TileData saved))
            return saved;

        // Otherwise generate by sid
        return TileGenerator.GenerateTile(x, y, worldSeed);
    }

    // When a player modifies a tile (e.g. builds a house or destroys a structure)
    public void SaveTile(TileData tile)
    {
        var key = new Vector2Int(tile.x, tile.y);
        modifiedTiles[key] = tile;
    }

    // To save
    public Dictionary<Vector2Int, TileData> ExportModifiedTiles()
    {
        return modifiedTiles;
    }

    // To download
    public void ImportModifiedTiles(Dictionary<Vector2Int, TileData> data)
    {
        modifiedTiles = data ?? new Dictionary<Vector2Int, TileData>();
    }
}
