using UnityEngine;
using System.Collections.Generic;

public class TileRenderer : MonoBehaviour
{
    [Header("Renderer layers")]
    [SerializeField] private SpriteRenderer biomeRenderer; 
    [SerializeField] private SpriteRenderer edgeRenderer;  

    [Header("Tileset Mapping")]
    public Dictionary<string, TileSet> biomeTilesets;  // биом → tileset

    private TileSet activeTileSet;

    private void Awake()
    {
        // Если мешает отсутствующий рендерер — создаём автоматически
        if (biomeRenderer == null)
        {
            GameObject obj = new GameObject("BiomeRenderer");
            obj.transform.SetParent(transform);
            biomeRenderer = obj.AddComponent<SpriteRenderer>();
            biomeRenderer.sortingOrder = 0;
        }

        if (edgeRenderer == null)
        {
            GameObject obj = new GameObject("EdgeRenderer");
            obj.transform.SetParent(transform);
            edgeRenderer = obj.AddComponent<SpriteRenderer>();
            edgeRenderer.sortingOrder = 1;
        }
    }

    public void RenderTile(TileData tile)
    {
        // 1️⃣ --- РЕНДЕР ОСНОВНОГО БИОМА ---
        Sprite biomeSprite = TileSpriteDB.Get(tile.biomeSpriteId);
        if (biomeSprite == null)
        {
            Debug.LogWarning($"[TileRenderer] Missing biome sprite: {tile.biomeSpriteId}");
        }
        biomeRenderer.sprite = biomeSprite;

        // 2️⃣ --- РЕНДЕР ПЕРЕХОДА (EDGEMASK) ---
        if (tile.edgeMask != 0 && tile.edgeBiome != null)
        {
            if (!biomeTilesets.TryGetValue(tile.edgeBiome, out activeTileSet))
            {
                Debug.LogWarning($"[TileRenderer] No tileset for biome {tile.edgeBiome}");
                edgeRenderer.sprite = null;
                return;
            }

            int index = activeTileSet.GetIndexForMask(tile.edgeMask);
            if (index < 0 || index >= activeTileSet.tiles.Length)
            {
                Debug.LogWarning($"[TileRenderer] Tileset index out of range: {index}");
                edgeRenderer.sprite = null;
                return;
            }

            edgeRenderer.sprite = activeTileSet.tiles[index];
        }
        else
        {
            // Нет перехода
            edgeRenderer.sprite = null;
        }
    }
}
