/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   ItemInstance.cs                                      /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:32:05 by UDA                                      */
/*   Updated: 2025/12/01 13:32:05 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public string id;
    public int quantity = 1;

    public ItemDefinition Def => ItemDatabase.Instance.Get(id);
    public LocalizedItem Loc => ItemLocalization.Get(id);

    public bool IsStackable => Def.maxStack > 1;
}
