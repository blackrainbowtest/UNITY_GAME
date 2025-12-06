/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   ItemIconDatabase.cs                                  /\_/\               */
/*                                                       ( вЂў.вЂў )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 14:17:02 by UDA                                      */
/*   Updated: 2025/12/01 14:17:02 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using UnityEngine;

public class ItemIconDatabase : MonoBehaviour
{
    public static ItemIconDatabase Instance;

    private Dictionary<string, Sprite> _icons = new Dictionary<string, Sprite>();
    public Sprite defaultIcon;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadIcons();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadIcons()
    {
        Sprite[] loaded = Resources.LoadAll<Sprite>("ItemIcons");

        foreach (var sprite in loaded)
        {
            string id = sprite.name.ToLower();
            _icons[id] = sprite;
        }

        UDADebug.Log("[ItemIconDB] Р—Р°РіСЂСѓР¶РµРЅРѕ РёРєРѕРЅРѕРє: " + _icons.Count);
    }

    public static Sprite Get(string itemId)
    {
        string id = itemId.ToLower();

        if (Instance._icons.TryGetValue(id, out var sprite))
            return sprite;

        Debug.LogWarning($"[ItemIconDB] РќРµС‚ РёРєРѕРЅРєРё РґР»СЏ {itemId}, РІРѕР·РІСЂР°С‰Р°СЋ default.");
        return Instance.defaultIcon;
    }
}

