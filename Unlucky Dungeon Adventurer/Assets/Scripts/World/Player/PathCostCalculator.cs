/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   PathCostCalculator.cs                                /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:02:37 by UDA                                      */
/*   Updated: 2025/12/01 13:02:37 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using UnityEngine;

public static class PathCostCalculator
{
    public static int GetStaminaCost(List<Vector2Int> path)
    {
        float cost = 0f;

        for (int i = 1; i < path.Count; i++)
        {
            var p = path[i];
            cost += TileGenerator.GetTileMoveCost(p.x, p.y);
        }

        return Mathf.CeilToInt(cost);
    }

    public static int GetTimeCost(List<Vector2Int> path, int minutesPerTile)
    {
        return minutesPerTile * (path.Count - 1);
    }
}
