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
/// Metadata describing where, when and how the save was created.
/// This is used by UI and by compatibility systems.
/// </summary>
[Serializable]
public class MetaSaveData
{
    public int slotIndex;            // -1 = autosave, 0..N = manual slot
    public string sceneName;         // Scene where player was saved
    public string saveTime;          // Human-readable timestamp
    public string saveVersion = SaveDataVersion.Current; // Save format version
    public string currentBiome;      // Biome ID or world region
}
