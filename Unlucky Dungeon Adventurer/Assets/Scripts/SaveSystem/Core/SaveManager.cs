/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Core/SaveManager.cs                      */
/*                                                        /\_/\               */
/*                                                       ( вЂў.вЂў )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Updated to AAA architecture: SaveManager is now a pure I/O layer.        */
/*                                                                            */
/* ************************************************************************** */

using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Low-level save/load handler.
/// Does not modify SaveData, does not write metadata,
/// and contains no game logic. Pure file I/O only.
/// </summary>
public static class SaveManager
{
	/// <summary>
	/// Writes save data to disk. Assumes metadata is already prepared by SaveService.
	/// </summary>
	public static void Save(SaveData data, int slotIndex)
	{
		if (data == null)
		{
			Debug.LogError("[SaveManager] Attempted to save NULL SaveData.");
			return;
		}

		string fileName = slotIndex < 0
			? "save_auto.json"
			: $"save_{slotIndex}.json";

		string path = Path.Combine(Application.persistentDataPath, fileName);

		string json = JsonUtility.ToJson(data, true);
		File.WriteAllText(path, json);

		UDADebug.Log($"[SaveManager] Saved -> {path}");
	}

	/// <summary>
	/// Loads save data from disk and deserializes it into SaveData.
	/// Returns NULL if the file does not exist or is corrupted.
	/// </summary>
	public static SaveData Load(int slotIndex)
	{
		string fileName = slotIndex < 0
			? "save_auto.json"
			: $"save_{slotIndex}.json";

		string path = Path.Combine(Application.persistentDataPath, fileName);

		if (!File.Exists(path))
		{
			Debug.LogWarning($"[SaveManager] File not found: {path}");
			return null;
		}

		string json = File.ReadAllText(path);
		SaveData data = JsonUtility.FromJson<SaveData>(json);

		UDADebug.Log($"[SaveManager] Loaded <- {path}");
		return data;
	}

	/// <summary>
	/// Helper for autosaves. Behaves just like Save(), but always targets auto slot.
	/// </summary>
	public static void SaveAuto(SaveData data)
	{
		try
		{
			string path = Path.Combine(Application.persistentDataPath, "save_auto.json");

			string json = JsonUtility.ToJson(data, true);
			File.WriteAllText(path, json);

			UDADebug.Log("[SaveManager] Autosave OK -> " + path);
		}
		catch (Exception ex)
		{
			Debug.LogError("[SaveManager] Autosave error: " + ex.Message);
		}
	}
}

