using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Static utility for converting between screen space, minimap texture space, and world tile coordinates.
/// Handles coordinate transformations for minimap interactions.
/// </summary>
public static class MinimapCoordinateConverter
{
    /// <summary>
    /// Converts screen pointer position to world tile coordinates.
    /// Accounts for minimap's RectTransform positioning, texture mapping, and UV Rect offset.
    /// </summary>
    /// <param name="eventData">Pointer event containing screen position</param>
    /// <param name="minimapImage">RawImage component displaying the minimap</param>
    /// <param name="tilesPerSide">Size of visible minimap area in tiles</param>
    /// <param name="originX">World X coordinate of minimap's bottom-left corner</param>
    /// <param name="originY">World Y coordinate of minimap's bottom-left corner</param>
    /// <param name="worldTile">Output: world tile coordinates</param>
    /// <returns>True if conversion succeeded (pointer within bounds)</returns>
    public static bool TryGetWorldTile(
        PointerEventData eventData,
        RawImage minimapImage,
        int tilesPerSide,
        int originX,
        int originY,
        out Vector2Int worldTile)
    {
        worldTile = Vector2Int.zero;

        if (minimapImage == null)
            return false;

        RectTransform rectTransform = minimapImage.rectTransform;

        // Convert cursor position to RawImage local coordinates
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
        {
            return false;
        }

        Rect rect = rectTransform.rect;

        // Normalize to 0..1 range в UI-координатах
        float normalizedX = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
        float normalizedY = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

        // Учитываем UV Rect: текстура может показывать только часть (при зуме)
        Rect uvRect = minimapImage.uvRect;
        
        // Переводим нормализованные UI-координаты в UV-координаты текстуры
        float uvX = uvRect.x + normalizedX * uvRect.width;
        float uvY = uvRect.y + normalizedY * uvRect.height;

        // UV → текстурные пиксели (полный размер текстуры)
        int textureSize = minimapImage.texture.width; // текстура всегда квадратная
        int pixelX = Mathf.Clamp(Mathf.FloorToInt(uvX * textureSize), 0, textureSize - 1);
        int pixelY = Mathf.Clamp(Mathf.FloorToInt(uvY * textureSize), 0, textureSize - 1);

        // Convert to world coordinates (origin соответствует углу полной текстуры)
        worldTile = new Vector2Int(originX + pixelX, originY + pixelY);
        return true;
    }

    /// <summary>
    /// Converts world tile coordinates to local minimap texture coordinates.
    /// Useful for determining if a tile is visible on the current minimap view.
    /// </summary>
    /// <param name="worldTile">World tile coordinates</param>
    /// <param name="originX">World X coordinate of minimap's bottom-left corner</param>
    /// <param name="originY">World Y coordinate of minimap's bottom-left corner</param>
    /// <param name="tilesPerSide">Size of minimap texture in tiles</param>
    /// <param name="localX">Output: local texture X coordinate (0 to tilesPerSide-1)</param>
    /// <param name="localY">Output: local texture Y coordinate (0 to tilesPerSide-1)</param>
    /// <returns>True if tile is within minimap bounds</returns>
    public static bool TryGetLocalCoordinates(
        Vector2Int worldTile,
        int originX,
        int originY,
        int tilesPerSide,
        out int localX,
        out int localY)
    {
        localX = worldTile.x - originX;
        localY = worldTile.y - originY;

        if (localX < 0 || localX >= tilesPerSide ||
            localY < 0 || localY >= tilesPerSide)
        {
            return false;
        }

        return true;
    }
}
