using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WorldGenerator is the CORE logic layer of the whole world.
///
/// Responsibilities:
/// ✔ Generate TileData using TileGenerator (once per tile)
/// ✔ Cache generated tiles for deterministic world
/// ✔ Store modified tiles (structures, events, player actions)
/// ✔ Provide stable data for WorldMapController
///
/// Guarantees:
/// - Each tile is generated ONCE per world seed.
/// - Sub-biomes, masks, transitions, random biome variants remain stable.
/// - No visual flickering.
/// - Modified tiles override cached generated tiles.
///
/// Architecture:
/// TileGenerator = procedural visual generator
/// WorldGenerator = stable world data provider
/// </summary>
public class WorldGenerator
{
    private readonly int worldSeed;

    // Tiles generated procedurally (cached permanently)
    private Dictionary<Vector2Int, TileData> tileCache;

    // Tiles modified by player or game logic
    private Dictionary<Vector2Int, TileData> modifiedTiles;


    // ============================================================
    // Constructor
    // ============================================================

    public WorldGenerator(int worldSeed)
    {
        this.worldSeed = worldSeed;
        tileCache = new Dictionary<Vector2Int, TileData>();
        modifiedTiles = new Dictionary<Vector2Int, TileData>();
    }


    // ============================================================
    // Main API: GetTile
    // ============================================================

    /// <summary>
    /// Returns a stable TileData for coordinates (x,y).
    /// 1) Modified tiles have priority.
    /// 2) Cached tiles are returned without regeneration.
    /// 3) If no cached tile exists, it is generated ONCE and cached.
    /// </summary>
    public TileData GetTile(int x, int y)
    {
        var key = new Vector2Int(x, y);

        // 1. Modified tile overrides everything
        if (modifiedTiles.TryGetValue(key, out TileData modified))
            return modified;

        // 2. Cached tile (already generated earlier)
        if (tileCache.TryGetValue(key, out TileData cached))
            return cached;

        // 3. Generate new tile (FIRST TIME EVER)
        TileData generated = TileGenerator.GenerateTile(x, y, worldSeed);

        // Cache it permanently
        tileCache[key] = generated;

        return generated;
    }


    // ============================================================
    // Save / modify tiles
    // ============================================================

    /// <summary>
    /// Save tile modified by the player (buildings, events, caves)
    /// </summary>
    public void SaveTile(TileData tile)
    {
        var key = new Vector2Int(tile.x, tile.y);
        modifiedTiles[key] = tile;
    }

    /// <summary>
    /// Returns dictionary of all modified tiles for saving to disk.
    /// </summary>
    public Dictionary<Vector2Int, TileData> ExportModifiedTiles()
    {
        return modifiedTiles;
    }

    /// <summary>
    /// Load modified tiles from savefile.
    /// </summary>
    public void ImportModifiedTiles(Dictionary<Vector2Int, TileData> saved)
    {
        modifiedTiles = saved ?? new Dictionary<Vector2Int, TileData>();
    }


    // ============================================================
    // OPTIONAL: exporting generated tiles for debugging
    // ============================================================

    public Dictionary<Vector2Int, TileData> ExportGeneratedTilesDebug()
    {
        return tileCache;
    }
}
