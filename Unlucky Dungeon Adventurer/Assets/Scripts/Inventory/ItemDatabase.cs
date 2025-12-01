/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   ItemDatabase.cs                                      /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:31:04 by UDA                                      */
/*   Updated: 2025/12/01 13:31:04 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public Dictionary<string, ItemDefinition> items = new Dictionary<string, ItemDefinition>();

    [System.Serializable]
    private class ItemWrapper
    {
        public List<ItemDefinition> items;
    }

    private void Awake()
    {
        Instance = this;

        TextAsset json = Resources.Load<TextAsset>("items");
        if (json == null)
        {
            Debug.LogError("[ItemDB] Не найден Resources/items.json");
            return;
        }

        var wrapper = JsonUtility.FromJson<ItemWrapper>(json.text);

        foreach (var def in wrapper.items)
        {
            items[def.id] = def;
        }

        Debug.Log("[ItemDB] Загружено defs: " + items.Count);
    }

    public ItemDefinition Get(string id)
    {
        items.TryGetValue(id, out var def);
        return def;
    }
}
