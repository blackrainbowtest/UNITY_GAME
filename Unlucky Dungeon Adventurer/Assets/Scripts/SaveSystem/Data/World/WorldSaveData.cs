/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Data/World/WorldSaveData.cs              */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 16:53:29 by UDA                                      */
/*   Updated: 2025/12/03 16:53:29 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System;
using System.Collections.Generic;
using WorldLogic;

/// <summary>
/// World state snapshot: seed, time, and dynamic world modifications.
/// </summary>
[Serializable]
public class WorldSaveData
{
    public int worldSeed;
    public int currentDay;
    public float timeOfDay; // 0–24 hours

    // Tiles modified during gameplay
    public List<ModifiedTileSaveData> modifiedTiles = new List<ModifiedTileSaveData>();

    // Generated unique world locations (your 50 epic points)
    public List<UniqueLocationSaveData> uniqueLocations = new List<UniqueLocationSaveData>();

    // Full state of all 50 unique locations (runtime states with progress)
    public List<UniqueLocationState> uniqueLocationStates = new List<UniqueLocationState>();

    public void AddMinutes(int minutes)
    {
        float totalMinutes = timeOfDay * 60f + minutes;
        int minutesPerDay = 24 * 60;

        if (totalMinutes >= minutesPerDay)
        {
            int daysPassed = (int)(totalMinutes / minutesPerDay);
            currentDay += daysPassed;
            totalMinutes %= minutesPerDay;
        }

        timeOfDay = totalMinutes / 60f;
    }
}
