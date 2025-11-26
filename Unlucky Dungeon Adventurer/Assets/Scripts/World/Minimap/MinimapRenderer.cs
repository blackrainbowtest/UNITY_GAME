using UnityEngine;

/// <summary>
/// Static utility providing low-level rendering operations for minimap textures.
/// All methods directly manipulate texture pixels for performance.
/// Remember to call texture.Apply() after batch operations.
/// </summary>
public static class MinimapRenderer
{
    /// <summary>
    /// Clears entire minimap texture to specified color.
    /// Typically used before redrawing to prevent visual artifacts.
    /// </summary>
    /// <param name="texture">Target minimap texture</param>
    /// <param name="clearColor">Color to fill texture with (usually transparent)</param>
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
    /// Draws camera viewport frame on the minimap.
    /// Renders a rectangular outline showing the current visible area.
    /// </summary>
    /// <param name="texture">Target minimap texture</param>
    /// <param name="camMinTile">World coordinates of camera's bottom-left tile</param>
    /// <param name="camMaxTile">World coordinates of camera's top-right tile</param>
    /// <param name="originX">World X of minimap's bottom-left corner</param>
    /// <param name="originY">World Y of minimap's bottom-left corner</param>
    /// <param name="tilesPerSide">Size of minimap texture</param>
    /// <param name="frameColor">Color for the frame outline (typically yellow)</param>
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

        // Clamp to texture bounds
        minX = Mathf.Clamp(minX, 0, tilesPerSide - 1);
        maxX = Mathf.Clamp(maxX, 0, tilesPerSide - 1);
        minY = Mathf.Clamp(minY, 0, tilesPerSide - 1);
        maxY = Mathf.Clamp(maxY, 0, tilesPerSide - 1);

        // Horizontal lines
        for (int x = minX; x <= maxX; x++)
        {
            texture.SetPixel(x, minY, frameColor);
            texture.SetPixel(x, maxY, frameColor);
        }

        // Vertical lines
        for (int y = minY; y <= maxY; y++)
        {
            texture.SetPixel(minX, y, frameColor);
            texture.SetPixel(maxX, y, frameColor);
        }
    }

    /// <summary>
    /// Draws player position marker on the minimap.
    /// Renders as a single colored pixel at the player's location.
    /// </summary>
    /// <param name="texture">Target minimap texture</param>
    /// <param name="playerTile">Player's world tile position</param>
    /// <param name="originX">World X of minimap's bottom-left corner</param>
    /// <param name="originY">World Y of minimap's bottom-left corner</param>
    /// <param name="tilesPerSide">Size of minimap texture</param>
    /// <param name="markerColor">Color for player marker (typically red)</param>
    /// <returns>True if marker was drawn (player within minimap bounds)</returns>
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
    /// Updates the color of a specific tile on the minimap.
    /// Typically called when terrain is revealed or modified.
    /// </summary>
    /// <param name="texture">Target minimap texture</param>
    /// <param name="worldTile">World coordinates of tile to update</param>
    /// <param name="originX">World X of minimap's bottom-left corner</param>
    /// <param name="originY">World Y of minimap's bottom-left corner</param>
    /// <param name="tilesPerSide">Size of minimap texture</param>
    /// <param name="color">New color for the tile</param>
    /// <returns>True if tile was updated (within minimap bounds)</returns>
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

        if (localX < 0 || localX >= tilesPerSide ||
            localY < 0 || localY >= tilesPerSide)
        {
            return false;
        }

        texture.SetPixel(localX, localY, color);
        return true;
    }
}
