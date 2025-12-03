/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Core/SaveService.cs                      */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 10:34:18 by UDA                                      */
/*   Updated: 2025/12/03 10:34:18 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

/**
 * This class is responsible for the single logic center
 */
 using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveService
{
	/// Get a list of all UI slots
	public static List<SaveSlotData> GetAllSlots(bool includeAutoSave)
	{
		var list = new List<SaveSlotData>();

		if (includeAutoSave)
		{
			string autoPath = SaveRepository.GetAutoSavePath();
			list.Add(new SaveSlotData(autoPath, -1, File.Exists(autoPath), true));
		}

		string[] files = SaveRepository.GetAllSlotFiles();

		foreach (string file in files)
		{
			int index = SaveRepository.ExtractIndexFromPath(file);
			list.Add(new SaveSlotData(file, index, true, false));
		}

		return list;
	}

	public static void RequestSave(int index, SaveData data)
	{
		SaveManager.Save(data, index);
	}

	public static SaveData RequestLoad(int index)
	{
		return SaveManager.Load(index);
	}

	public static void RequestDelete(int index)
	{
		SaveRepository.DeleteSlot(index);
	}
}
