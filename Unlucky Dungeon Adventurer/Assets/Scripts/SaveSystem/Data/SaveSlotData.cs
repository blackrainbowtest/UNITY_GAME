/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Data/SaveSlotData.cs                     */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 10:36:11 by UDA                                      */
/*   Updated: 2025/12/03 10:36:11 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

/**
 * Small data model for UI
 */
using UnityEngine;

public struct SaveSlotData
{
	public string path;
	public int index;
	public bool exists;
	public bool isAuto;

	public SaveSlotData(string path, int index, bool exists, bool isAuto)
	{
		this.path = path;
		this.index = index;
		this.exists = exists;
		this.isAuto = isAuto;
	}
}
