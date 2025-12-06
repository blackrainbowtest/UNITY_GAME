/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   MovementEventResolver.cs                             /\_/\               */
/*                                                       ( вЂў.вЂў )              */
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
        // Р’ Р±СѓРґСѓС‰РµРј: С‚Р°Р±Р»РёС†Р° СЃРѕР±С‹С‚РёР№, РІСЃС‚СЂРµС‡, Р»СѓС‚Р°, Р»РѕРІСѓС€РµРє
        UDADebug.Log($"[Event] РџСЂРѕРІРµСЂРєР° СЃРѕР±С‹С‚РёСЏ РЅР° С‚Р°Р№Р»Рµ {tile.x}, {tile.y}");
    }
}


