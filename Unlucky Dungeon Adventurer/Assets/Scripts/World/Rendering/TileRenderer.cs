using System.Collections.Generic;
using UnityEngine;

public class TileRenderer : MonoBehaviour
{
    [Header("Assigned Automatically")]
    public TileSpriteDB spriteDB;

    [Header("Sprite Renderers (auto-created)")]
    public SpriteRenderer biomeRenderer;         // Order 0
    public SpriteRenderer sub1Renderer;          // Order 1
    public SpriteRenderer sub2Renderer;          // Order 2
    public SpriteRenderer sub3Renderer;          // Order 3
    public SpriteRenderer structureRenderer;     // Order 4
    public SpriteRenderer activityRenderer;      // Order 5

    private void Awake()
    {
        if (spriteDB == null)
        {
            spriteDB = Resources.Load<TileSpriteDB>("WorldData/TileSpriteDB");
        }

        CreateLayersIfMissing();
    }

    private void CreateLayersIfMissing()
    {
        biomeRenderer      = EnsureLayer("BiomeLayer",      0, ref biomeRenderer);
        sub1Renderer       = EnsureLayer("SubBiomeLayer1",  1, ref sub1Renderer);
        sub2Renderer       = EnsureLayer("SubBiomeLayer2",  2, ref sub2Renderer);
        sub3Renderer       = EnsureLayer("SubBiomeLayer3",  3, ref sub3Renderer);
        structureRenderer  = EnsureLayer("StructureLayer",  4, ref structureRenderer);
        activityRenderer   = EnsureLayer("ActivityLayer",   5, ref activityRenderer);
    }

    private SpriteRenderer EnsureLayer(string name, int order, ref SpriteRenderer field)
    {
        if (field != null) return field;

        Transform child = transform.Find(name);
        if (child != null)
        {
            field = child.GetComponent<SpriteRenderer>();
            field.sortingOrder = order;
            return field;
        }

        // Create new
        GameObject obj = new GameObject(name);
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = order;

        field = sr;
        return sr;
    }

    // ------------------------------------------
    //               MAIN RENDER
    // ------------------------------------------
    public void RenderTile(TileData data)
    {
        if (spriteDB == null)
        {
            Debug.LogError("[TileRenderer] Missing spriteDB");
            return;
        }

        // BASE BIOME LAYER ----------------------
        biomeRenderer.sprite = spriteDB.Get(data.biomeSpriteId);
        biomeRenderer.color = Color.white;

        // SUB-BIOME LAYERS ----------------------
        SpriteRenderer[] subRenderers = {
            sub1Renderer, sub2Renderer, sub3Renderer
        };

        for (int i = 0; i < subRenderers.Length; i++)
        {
            if (data.subBiomeIds != null && i < data.subBiomeIds.Count)
            {
                string id = data.subBiomeIds[i];
                Sprite sp = spriteDB.Get(id);
                subRenderers[i].sprite = sp;
                subRenderers[i].color = Color.white;
            }
            else
            {
                subRenderers[i].sprite = null;
            }
        }

        // STRUCTURE LAYER -----------------------
        if (!string.IsNullOrEmpty(data.structureId))
        {
            structureRenderer.sprite = spriteDB.Get(data.structureId);
        }
        else
        {
            structureRenderer.sprite = null;
        }

        // ACTIVITY LAYER ------------------------
        if (!string.IsNullOrEmpty(data.activityId))
        {
            activityRenderer.sprite = spriteDB.Get(data.activityId);
        }
        else
        {
            activityRenderer.sprite = null;
        }
    }
}
