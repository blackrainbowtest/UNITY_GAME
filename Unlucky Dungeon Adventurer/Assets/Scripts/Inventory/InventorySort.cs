/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   InventorySort.cs                                     /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 14:46:41 by UDA                                      */
/*   Updated: 2025/12/01 14:46:41 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using System.Linq;

public static class InventorySort
{
    public enum SortMode
    {
        Name,
        Type,
        Rarity,
        Price,
        Id
    }

    public static List<ItemInstance> Sort(List<ItemInstance> items, SortMode mode)
    {
        switch (mode)
        {
            case SortMode.Name:
                return items.OrderBy(i => i.Loc.name).ToList();

            case SortMode.Type:
                return items.OrderBy(i => i.Def.type).ThenBy(i => i.Loc.name).ToList();

            case SortMode.Rarity:
                return items.OrderBy(i => i.Def.rarity).ThenBy(i => i.Loc.name).ToList();

            case SortMode.Price:
                return items.OrderByDescending(i => i.Def.price).ToList();

            case SortMode.Id:
            default:
                return items.OrderBy(i => i.id).ToList();
        }
    }
}
