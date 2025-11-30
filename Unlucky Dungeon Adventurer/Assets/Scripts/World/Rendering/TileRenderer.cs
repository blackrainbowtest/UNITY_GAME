using UnityEngine;
using System.Collections.Generic;

public class TileRenderer : MonoBehaviour
{
    [Header("Base biome sprite")]
    public SpriteRenderer biomeRenderer;

    [Header("Base Biome Sprites (TileSpriteDB)")]
    [SerializeField] private TileSpriteDB spriteDB;

    [Header("Edge transition renderer")]
    public SpriteRenderer edgeRenderer;

    [Header("Biomes → Tilesets")]
    public List<BiomeTilesetEntry47> tilesets47;

    private Dictionary<string, TileSet47> dict;

    private void Awake()
    {
        dict = new();

        foreach (var e in tilesets47)
            if (!string.IsNullOrEmpty(e.biomeId) && e.tileset != null)
                dict[e.biomeId] = e.tileset;
    }

    public void RenderTile(TileData tile)
    {
        biomeRenderer.sprite = spriteDB != null 
            ? spriteDB.Get(tile.biomeSpriteId)
            : null;

        if (tile.edgeMask == 0 || string.IsNullOrEmpty(tile.edgeBiome))
        {
            edgeRenderer.sprite = null;
            return;
        }

        if (!dict.TryGetValue(tile.edgeBiome, out var set))
        {
            edgeRenderer.sprite = null;
            return;
        }

        int index = set.GetTileIndex(tile.edgeMask);

        if (index < 0 || index >= set.tiles.Length)
        {
            edgeRenderer.sprite = null;
            return;
        }

        edgeRenderer.sprite = set.tiles[index];
    }
}

[System.Serializable]
public class BiomeTilesetEntry47
{
    public string biomeId;
    public TileSet47 tileset;
}
