/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Data/Inventory/ItemSaveData.cs           */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 16:58:24 by UDA                                      */
/*   Updated: 2025/12/03 16:58:24 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System;

/// <summary>
/// Legacy stackable item entry.
/// </summary>
[Serializable]
public class ItemSaveData
{
    public string itemId;
    public int quantity;
}
