/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Core/SaveDataFactory.cs                  */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03                                                      */
/*                                                                            */
/* ************************************************************************** */

using System;
using System.Collections.Generic;
using UnityEngine;

public static class SaveDataFactory
{
	public static SaveData CreateNew(string playerName, string classId, int worldSeed)
	{
		Debug.Log($"[SaveDataFactory] Creating new save: player={playerName}, class={classId}, seed={worldSeed}");
		
		var data = new SaveData();

		// -------------------------
		// META DATA
		// -------------------------
		data.meta = new MetaSaveData
		{
			slotIndex      = 0,
			sceneName      = "WorldMap",
			playerName     = playerName,
			saveVersion    = SaveDataVersion.Current,
			createdTime    = DateTime.Now.ToString("o"),
			lastPlayedTime = DateTime.Now.ToString("o"),
			currentBiome   = "unknown"
		};

		// -------------------------
		// PLAYER DATA
		// -------------------------
		data.player = new PlayerSaveData
		{
			name        = playerName,
			playerClass = classId,
			worldSeed   = worldSeed,

			level       = 1,
			experience  = 0,
			experienceToNext = 100,

			gold        = 500,

			baseMaxHP      = 100,
			baseMaxMP      = 20,
			baseMaxStamina = 100,

			baseAttack  = 5,
			baseDefense = 3,
			baseAgility = 5,
			baseLust    = 0,

			currentHP      = 100,
			currentMP      = 20,
			currentStamina = 100,

			isPregnant = 0,

			mapPosX = 0,
			mapPosY = 0,

			inventoryItems = new List<ItemInstance>()
		};

		// -------------------------
		// WORLD DATA
		// -------------------------
		data.world = new WorldSaveData
		{
			worldSeed     = worldSeed,
			currentDay    = 0,
			timeOfDay     = 12f, // start at noon
			modifiedTiles = new List<ModifiedTileSaveData>(),
			uniqueLocations = new List<UniqueLocationSaveData>()
		};

		return data;
	}
}
