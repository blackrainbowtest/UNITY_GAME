/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Core/SaveService.cs                      */
/*                                                        /\_/\               */
/*                                                       ( вЂў.вЂў )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 10:34:18 by UDA                                      */
/*   Updated: 2025/12/03 10:34:18 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

/**
 * This class is responsible for the single logic center
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central save system service.
/// UI and controllers call this class to perform save/load/delete.
/// All metadata preparation is handled here.
/// </summary>
public static class SaveService
{
	/// <summary>
	/// Returns a list of all save slots for UI.
	/// </summary>
	public static List<SaveSlotData> GetAllSlots(bool includeAutoSave)
	{
		var list = new List<SaveSlotData>();

		// Autosave slot
		if (includeAutoSave)
		{
			string autoPath = SaveRepository.GetAutoSavePath();
			list.Add(new SaveSlotData(autoPath, -1, System.IO.File.Exists(autoPath), true));
		}

		// Manual save slots
		string[] files = SaveRepository.GetAllSlotFiles();
		foreach (string file in files)
		{
			int index = SaveRepository.ExtractIndexFromPath(file);
			list.Add(new SaveSlotData(file, index, true, false));
		}

		return list;
	}

	/// <summary>
	/// Saves the supplied SaveData into the specified slot.
	/// Fills meta information before writing to disk.
	/// </summary>
	public static void RequestSave(int slotIndex, SaveData data)
	{
		if (data == null)
		{
			Debug.LogError("[SaveService] Attempted to save NULL data.");
			return;
		}

		PrepareMetadata(slotIndex, data);

		SaveManager.Save(data, slotIndex);

		UDADebug.Log($"[SaveService] Saved slot {slotIndex} | Scene={data.meta.sceneName} | Time={data.meta.lastPlayedTime}");
	}

	/// <summary>
	/// Load save data by slot index.
	/// </summary>
	public static SaveData RequestLoad(int slotIndex)
	{
		SaveData data = SaveManager.Load(slotIndex);

		if (data == null)
		{
			Debug.LogError($"[SaveService] Failed to load slot {slotIndex}");
			return null;
		}

		return data;
	}

	/// <summary>
	/// Delete a save slot from disk.
	/// </summary>
	public static void RequestDelete(int slotIndex)
	{
		SaveRepository.DeleteSlot(slotIndex);
		UDADebug.Log($"[SaveService] Deleted slot {slotIndex}");
	}

	// ---------------------------------------------------------
	// META INFORMATION BUILDER
	// ---------------------------------------------------------

	/// <summary>
	/// Prepares meta information before saving.
	/// </summary>
	private static void PrepareMetadata(int slotIndex, SaveData data)
	{
		data.meta.slotIndex   = slotIndex;
		data.meta.sceneName   = SceneManager.GetActiveScene().name;
		data.meta.lastPlayedTime    = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		data.meta.saveVersion = SaveDataVersion.Current;

		// Biome detection (if your world manager exists)
		data.meta.currentBiome = TryGetBiomeIdSafe();
	}

	/// <summary>
	/// Safely retrieves the current biome without crashing
	/// even if biome system is not loaded.
	/// TODO: Re-enable when WorldManager is available.
	/// </summary>
	private static string TryGetBiomeIdSafe()
	{
		// TODO: Implement biome detection when WorldManager exists
		// try
		// {
		// 	if (WorldManager.Instance != null)
		// 		return WorldManager.Instance.CurrentBiomeID;
		// }
		// catch { }

		return "unknown";
	}

	/// <summary>
	/// Creates a new game with initial save data.
	/// </summary>
	public static void CreateNewGame(string name, string role, int seed)
	{
		var data = SaveDataFactory.CreateNew(name, role, seed);

		// РђРІС‚РѕСЃРµР№РІ РЅРѕРІРѕРіРѕ РїРµСЂСЃРѕРЅР°Р¶Р°
		SaveManager.SaveAuto(data);

		// РўР°РєР¶Рµ РјРѕР¶РЅРѕ СЃРѕР·РґР°С‚СЊ persistent СЃР»РѕС‚ 0
		SaveManager.Save(data, 0);

		// Р’РђР–РќРћ: Р—Р°РіСЂСѓР¶Р°РµРј СЃРѕР·РґР°РЅРЅС‹Рµ РґР°РЅРЅС‹Рµ РІ TempSaveCache,
		// С‡С‚РѕР±С‹ РїСЂРё Р·Р°РіСЂСѓР·РєРµ WorldMap СЃС†РµРЅС‹ РѕРЅРё РїСЂРёРјРµРЅРёР»РёСЃСЊ
		TempSaveCache.pendingSave = data;

		UDADebug.Log($"[SaveService] New game created: {name} ({role}) | seed={seed}");
	}
}


