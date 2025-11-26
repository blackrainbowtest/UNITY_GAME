using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Утилита для конвертации координат миникарты
/// </summary>
public static class MinimapCoordinateConverter
{
    /// <summary>
    /// Конвертирует позицию указателя в мировые координаты тайла
    /// </summary>
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

        // Конвертим позицию курсора в локальные координаты RawImage
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
        {
            return false;
        }

        Rect rect = rectTransform.rect;

        // Нормализуем в 0..1
        float normalizedX = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
        float normalizedY = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

        // Переводим в координаты пикселей текстуры
        int pixelX = Mathf.Clamp(
            Mathf.FloorToInt(normalizedX * tilesPerSide),
            0, tilesPerSide - 1
        );
        int pixelY = Mathf.Clamp(
            Mathf.FloorToInt(normalizedY * tilesPerSide),
            0, tilesPerSide - 1
        );

        // Переводим в координаты мира
        worldTile = new Vector2Int(originX + pixelX, originY + pixelY);
        return true;
    }

    /// <summary>
    /// Конвертирует мировые координаты в локальные координаты текстуры миникарты
    /// </summary>
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

        // Проверяем, что координаты в пределах текстуры
        if (localX < 0 || localX >= tilesPerSide ||
            localY < 0 || localY >= tilesPerSide)
        {
            return false;
        }

        return true;
    }
}
