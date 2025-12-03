/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Data/World/ModifiedTileSaveData.cs       */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 16:53:55 by UDA                                      */
/*   Updated: 2025/12/03 16:53:55 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System;

/// <summary>
/// A single tile that diverges from procedural generation.
/// Used for burned forests, built roads, terraforming, etc.
/// </summary>
[Serializable]
public class ModifiedTileSaveData
{
    public int x;
    public int y;

    public string biomeId;        // Optional override
    public string overrideTileId; // Sprite/type override
}
