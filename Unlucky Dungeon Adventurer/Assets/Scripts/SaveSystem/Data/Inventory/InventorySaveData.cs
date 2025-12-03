/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Data/Inventory/InventorySaveData.cs      */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 16:55:38 by UDA                                      */
/*   Updated: 2025/12/03 16:55:38 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System;
using System.Collections.Generic;

/// <summary>
/// Legacy/global inventory container.
/// Consider migrating fully into PlayerSaveData.
/// </summary>
[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items = new List<ItemSaveData>();
}
