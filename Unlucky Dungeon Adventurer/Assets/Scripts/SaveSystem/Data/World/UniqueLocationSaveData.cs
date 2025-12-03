/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Data/World/UniqueLocationSaveData.cs     */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 16:55:16 by UDA                                      */
/*   Updated: 2025/12/03 16:55:16 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System;

/// <summary>
/// One handcrafted unique world location (1 of your 50).
/// </summary>
[Serializable]
public class UniqueLocationSaveData
{
    public string id;     // Stable identifier ("dragon_lair_01")
    public int x;
    public int y;

    public bool discovered;
    public bool completed;
}
