/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   RarityColors.cs                                      /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 16:29:51 by UDA                                      */
/*   Updated: 2025/12/01 16:29:51 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public static class RarityColors
{
    public static readonly Color Common      = new Color(0.78f, 0.78f, 0.78f);  // серый
    public static readonly Color Uncommon    = new Color(0.3f, 0.85f, 0.3f);    // зелёный
    public static readonly Color Rare        = new Color(0.2f, 0.45f, 1f);      // синий
    public static readonly Color Epic        = new Color(0.7f, 0.2f, 0.85f);    // фиолетовый
    public static readonly Color Legendary   = new Color(1f, 0.55f, 0.1f);      // оранжевый
    public static readonly Color Mythic      = new Color(1f, 0.2f, 0.25f);      // красный

    public static Color Get(int rarity)
    {
        return rarity switch
        {
            1 => Uncommon,
            2 => Rare,
            3 => Epic,
            4 => Legendary,
            5 => Mythic,
            _ => Common,
        };
    }
}
