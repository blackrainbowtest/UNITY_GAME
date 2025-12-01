/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   ItemDefinition.cs                                    /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:30:46 by UDA                                      */
/*   Updated: 2025/12/01 13:30:46 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System;

[Serializable]
public class ItemDefinition
{
    public string id;              // Уникальный ID предмета
    public string type;            // consumable, bag, camp, weapon, armor, etc.
    public int maxStack = 1;

    public int capacityBonus;      // Только для сумок
    public ItemEffects effects;    // Для consumable
}

[Serializable]
public class ItemEffects
{
    public int hp;
    public int stamina;
    public int mana;
}
