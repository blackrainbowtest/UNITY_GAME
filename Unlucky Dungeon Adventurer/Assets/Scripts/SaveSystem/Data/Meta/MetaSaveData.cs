/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Data/Meta/MetaSaveData.cs                */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 16:52:50 by UDA                                      */
/*   Updated: 2025/12/03 16:52:50 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System;

/// <summary>
/// Metadata used by the save slot UI and compatibility checks.
/// </summary>
[Serializable]
public class MetaSaveData
{
    public int slotIndex;                 // -1 = autosave
    public string sceneName;              // Scene where save was made
    public string playerName;             // Name of character
    public string saveVersion;            // Version of save format

    public string createdTime;            // ISO timestamp of creation
    public string lastPlayedTime;         // ISO timestamp of last load/save

    public string currentBiome;           // optional: biome ID for UI
}
