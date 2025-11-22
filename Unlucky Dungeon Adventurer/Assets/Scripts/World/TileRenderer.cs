using UnityEngine;

public class TileRenderer : MonoBehaviour
{
    public SpriteRenderer sr;
    public TileSpriteDB spriteDB;

    private static Sprite _fallbackSprite;

    private static Sprite GetFallbackSprite()
    {
        if (_fallbackSprite != null)
            return _fallbackSprite;

        // Unity already has a built-in white 1x1 texture: Texture2D.whiteTexture
        var tex = Texture2D.whiteTexture;

        // Create sprite safely (pivot center, PPU = 32)
        _fallbackSprite = Sprite.Create(
            tex,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            32,
            0,
            SpriteMeshType.FullRect
        );

        _fallbackSprite.name = "__FallbackTileSprite";
        return _fallbackSprite;
    }

    private void Awake()
    {
        // Auto-load spriteDB once
        if (spriteDB == null)
        {
            spriteDB = Resources.Load<TileSpriteDB>("WorldData/TileSpriteDB");

            if (spriteDB == null)
            {
                Debug.LogWarning("[TileRenderer] TileSpriteDB not found in Resources. Using fallback rendering.");
            }
        }
    }

    public void RenderTile(TileData data)
    {
        if (sr == null)
        {
            Debug.LogError("[TileRenderer] SpriteRenderer not assigned.");
            return;
        }

        if (spriteDB == null)
        {
            // Try to auto-load from Resources (common path: Resources/WorldData/ or Resources/)
            spriteDB = Resources.Load<TileSpriteDB>("WorldData/TileSpriteDB") ?? Resources.Load<TileSpriteDB>("TileSpriteDB");
            if (spriteDB == null)
            {
                Debug.LogError("[TileRenderer] spriteDB is not assigned on the TileRenderer and no TileSpriteDB found in Resources. Using existing sprite on SpriteRenderer or color fallback.");
                if (sr.sprite == null)
                {
                    // keep tile visible by tinting an existing sprite if any; otherwise leave sprite null so color fallback can apply elsewhere
                    // We do not overwrite sr.sprite here to avoid making tiles invisible; ensure prefab SpriteRenderer has a default sprite if possible.
                }
                sr.color = data.color;
                return;
            }
            else
            {
                Debug.Log("[TileRenderer] Auto-loaded TileSpriteDB from Resources.");
            }
        }

        Sprite sprite = spriteDB.Get(data.spriteId);

        if (sprite != null)
        {
            sr.sprite = sprite;
            sr.color = Color.white;
        }
        else
        {
            // fallback: if the SpriteRenderer already has a sprite (e.g., from the prefab), keep it and tint it;
            // otherwise keep sprite null and use color tint as fallback (ensure prefab has a default sprite to be visible).
            if (sr.sprite != null)
            {
                sr.color = data.color;
            }
            else
            {
                sr.sprite = null; // leave null; WorldMapController may create/assign placeholder sprites
                sr.color = data.color;
            }
        }
            // Debug.LogWarning($"[TileRenderer] Sprite for id '{data.spriteId}' not found in spriteDB. Using color fallback.");
    }
}
