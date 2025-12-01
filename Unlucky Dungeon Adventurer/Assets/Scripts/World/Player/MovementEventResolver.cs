/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   MovementEventResolver.cs                             /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:03:59 by UDA                                      */
/*   Updated: 2025/12/01 13:03:59 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public static class MovementEventResolver
{
    public static void ProcessTileEvent(Vector2Int tile)
    {
        // В будущем: таблица событий, встреч, лута, ловушек
        Debug.Log($"[Event] Проверка события на тайле {tile.x}, {tile.y}");
    }
}

