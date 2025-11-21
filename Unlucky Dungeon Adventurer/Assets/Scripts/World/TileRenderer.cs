using UnityEngine;

public class TileRenderer : MonoBehaviour
{
    public SpriteRenderer sr;
    public TileSpriteDB spriteDB;

    public void RenderTile(TileData data)
    {
        Sprite s = spriteDB.Get(data.spriteId);

        if (s != null)
        {
            sr.sprite = s;
            sr.color = Color.white;
        }
        else
        {
            // fallback
            sr.sprite = null;
            sr.color = data.color;
        }
    }
}