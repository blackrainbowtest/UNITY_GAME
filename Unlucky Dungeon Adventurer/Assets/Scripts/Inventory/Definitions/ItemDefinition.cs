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
    public string id;               // ID предмета
    public string type;             // weapon, armor, consumable, bag, camp, misc
    public int maxStack = 1;

    // Новые поля:
    public int rarity = 0;          // 0=common, 1=uncommon, 2=rare, 3=epic, 4=legendary, 5=mythic
    public int price = 0;           // цена предмета в золоте

    // Боевая логика:
    public int weaponDamage = 0;    // если оружие
    public int armorValue = 0;      // если броня

    // Доп. свойства:
    public string[] tags;           // "sharp", "magic", "heavy" и т.п.

    // Потребляемые предметы:
    public ItemEffects effects;

    // Для сумок
    public int capacityBonus;
}

[Serializable]
public class ItemEffects
{
    public int hp;
    public int stamina;
    public int mana;
}
