/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Core/SaveRepository.cs                   */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 10:31:36 by UDA                                      */
/*   Updated: 2025/12/03 10:31:36 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

/**
 * This class is responsible only for working with files:
 * — slot search
 * — existence check
 * — deletion
 * — ​​path generation
 */
 using System.IO;
using System.Linq;
using UnityEngine;

public static class SaveRepository
{
	private const string AutoSaveName = "save_auto.json";
	private const string SlotPrefix = "save_";
	private const string Extension = ".json";

	public static string GetAutoSavePath()
	{
		return Path.Combine(Application.persistentDataPath, AutoSaveName);
	}

	public static string GetSlotPath(int index)
	{
		return Path.Combine(Application.persistentDataPath, $"{SlotPrefix}{index}{Extension}");
	}

	public static bool SlotExists(int index)
	{
		return File.Exists(GetSlotPath(index));
	}

	public static bool AutoSaveExists()
	{
		return File.Exists(GetAutoSavePath());
	}

	public static void DeleteSlot(int index)
	{
		string path = GetSlotPath(index);
		if (File.Exists(path))
			File.Delete(path);
	}

	public static string[] GetAllSlotFiles()
	{
		return Directory
			.GetFiles(Application.persistentDataPath, $"{SlotPrefix}*{Extension}")
			.Where(f => !f.Contains("auto"))
			.ToArray();
	}

	public static int ExtractIndexFromPath(string path)
	{
		string name = Path.GetFileNameWithoutExtension(path);
		if (!name.StartsWith(SlotPrefix))
			return -1;

		string num = name.Substring(SlotPrefix.Length);
		return int.TryParse(num, out int result) ? result : -1;
	}
}
