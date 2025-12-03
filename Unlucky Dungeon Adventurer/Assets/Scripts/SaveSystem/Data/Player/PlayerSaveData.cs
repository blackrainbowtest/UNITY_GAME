/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Data/Player/PlayerSaveData.cs            */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 16:53:08 by UDA                                      */
/*   Updated: 2025/12/03 16:53:08 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System;
using System.Collections.Generic;

/// <summary>
/// Complete persistent player state: stats, inventory, progression.
/// </summary>
[Serializable]
public class PlayerSaveData
{
    public string name;
    public string playerClass;

    public int level;
    public int gold;
    public int worldSeed;

    public int experience;
    public int experienceToNext;

    // Base stats (from class progression)
    public int baseMaxHP;
    public int baseMaxMP;
    public int baseMaxStamina;

    public int baseAttack;
    public int baseDefense;
    public int baseAgility;
    public int baseLust;

    // Current values (UI bars)
    public int currentHP;
    public int currentMP;
    public int currentStamina;

    public int isPregnant;

    // Map position
    public float mapPosX;
    public float mapPosY;

    // Player-bound inventory
    public List<ItemInstance> inventoryItems = new List<ItemInstance>();
}
