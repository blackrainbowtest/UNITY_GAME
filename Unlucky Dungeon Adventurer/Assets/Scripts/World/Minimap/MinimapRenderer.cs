using UnityEngine;

/// <summary>
/// Утилита для рендеринга миникарты
/// </summary>
public static class MinimapRenderer
{
    /// <summary>
    /// Очищает всю текстуру миникарты
    /// </summary>
    public static void ClearTexture(Texture2D texture, Color clearColor)
    {
        if (texture == null) return;

        int width = texture.width;
        int height = texture.height;
        var colors = new Color[width * height];
        
        for (int i = 0; i < colors.Length; i++)
            colors[i] = clearColor;

        texture.SetPixels(colors);
        texture.Apply();
    }

    /// <summary>
    /// Рисует рамку камеры на миникарте
    /// </summary>
    public static void DrawCameraFrame(
        Texture2D texture,
        Vector2Int camMinTile,
        Vector2Int camMaxTile,
        int originX,
        int originY,
        int tilesPerSide,
        Color frameColor)
    {
        if (texture == null) return;

        int minX = camMinTile.x - originX;
        int maxX = camMaxTile.x - originX;
        int minY = camMinTile.y - originY;
        int maxY = camMaxTile.y - originY;

        // Клампим на всякий случай
        minX = Mathf.Clamp(minX, 0, tilesPerSide - 1);
        maxX = Mathf.Clamp(maxX, 0, tilesPerSide - 1);
        minY = Mathf.Clamp(minY, 0, tilesPerSide - 1);
        maxY = Mathf.Clamp(maxY, 0, tilesPerSide - 1);

        // Горизонтальные линии
        for (int x = minX; x <= maxX; x++)
        {
            texture.SetPixel(x, minY, frameColor);
            texture.SetPixel(x, maxY, frameColor);
        }

        // Вертикальные линии
        for (int y = minY; y <= maxY; y++)
        {
            texture.SetPixel(minX, y, frameColor);
            texture.SetPixel(maxX, y, frameColor);
        }
    }

    /// <summary>
    /// Рисует маркер игрока на миникарте
    /// </summary>
    public static bool DrawPlayerMarker(
        Texture2D texture,
        Vector2Int playerTile,
        int originX,
        int originY,
        int tilesPerSide,
        Color markerColor)
    {
        if (texture == null) return false;

        int px = playerTile.x - originX;
        int py = playerTile.y - originY;

        if (px < 0 || px >= tilesPerSide ||
            py < 0 || py >= tilesPerSide)
            return false;

        texture.SetPixel(px, py, markerColor);
        return true;
    }

    /// <summary>
    /// Обновляет цвет конкретного тайла на миникарте
    /// </summary>
    public static bool UpdateTile(
        Texture2D texture,
        Vector2Int worldTile,
        int originX,
        int originY,
        int tilesPerSide,
        Color color)
    {
        if (texture == null) return false;

        int localX = worldTile.x - originX;
        int localY = worldTile.y - originY;

        // Если тайл вне текущего окна миникарты — игнорируем
        if (localX < 0 || localX >= tilesPerSide ||
            localY < 0 || localY >= tilesPerSide)
        {
            return false;
        }

        texture.SetPixel(localX, localY, color);
        return true;
    }
}
