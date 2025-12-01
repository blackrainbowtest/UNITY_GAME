/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   RestEnvironmentDetector.cs                           /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:09:04 by UDA                                      */
/*   Updated: 2025/12/01 13:09:04 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public static class RestEnvironmentDetector
{
    /// <summary>
    /// Определяет тип отдыха в зависимости от тайла.
    /// </summary>
    public static RestEnvironment GetEnvironment(Vector2Int tilePos)
    {
        // Получаем генератор мира
        var gen = WorldMapController.Instance?.GetWorldGenerator();
        TileData tile = gen != null ? gen.GetTile(tilePos.x, tilePos.y) : null;
        if (tile == null)
            return RestEnvironment.Field;

        // Временная логика: используем structureId, если начнём помечать структуры
        // Например: structureId == "city" → City, "village" → Village
        if (!string.IsNullOrEmpty(tile.structureId))
        {
            switch (tile.structureId)
            {
                case "city": return RestEnvironment.City;
                case "village": return RestEnvironment.Village;
                case "camp": return RestEnvironment.Tent; // лагерь / палатка
            }
        }

        // Позже: проверка размещённых объектов игрока (палатка и т.п.)
        if (PlayerHasTentOnTile(tilePos))
            return RestEnvironment.Tent;

        return RestEnvironment.Field;
    }

    /// <summary>
    /// Проверка палатки игрока.
    /// Пока возвращаем false — система будет добавлена позже.
    /// </summary>
    private static bool PlayerHasTentOnTile(Vector2Int tile)
    {
        // TODO: позже добавим хранение палатки в SaveData
        return false;
    }
}
