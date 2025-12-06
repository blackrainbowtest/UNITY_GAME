using System.Collections.Generic;
using UnityEngine;

public static class OutfitUtils
{
    // Populate dictionary from array of SpriteEntry
    public static Dictionary<string, Sprite> BuildSpriteMap(SpriteEntry[] entries)
    {
        var map = new Dictionary<string, Sprite>();
        if (entries == null) return map;
        foreach (var e in entries)
        {
            if (!string.IsNullOrEmpty(e.key) && e.sprite != null)
            {
                map[e.key] = e.sprite;
            }
        }
        return map;
    }

    // Try add fallback sprite into map if not present
    public static void TryAddFallback(Dictionary<string, Sprite> map, string key, Sprite sprite)
    {
        if (sprite == null) return;
        if (!map.ContainsKey(key))
        {
            map[key] = sprite;
            UDADebug.Log($"[OutfitUtils] Added fallback sprite map entry '{key}' -> {sprite.name}");
        }
    }

    // Load outfits collection from a TextAsset (safe parse)
    public static OutfitCollection LoadOutfits(TextAsset jsonAsset)
    {
        if (jsonAsset == null) return null;
        try
        {
            return JsonUtility.FromJson<OutfitCollection>(jsonAsset.text);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[OutfitUtils] Failed to parse outfits JSON: {ex.Message}");
            return null;
        }
    }
}

