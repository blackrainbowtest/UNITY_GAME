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
        TileData tile = WorldGenerator.GetTile(tilePos.x, tilePos.y);

        if (tile == null)
            return RestEnvironment.Field;

        // 🔥 Когда появится генерация деревень/городов — добавим:
        if (tile.hasCity)
            return RestEnvironment.City;

        if (tile.hasVillage)
            return RestEnvironment.Village;

        // 🔥 Палатка игрока (в будущем: размещение палатки)
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
