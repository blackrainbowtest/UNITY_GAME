/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   ItemLocalization.cs                                  /\_/\               */
/*                                                       ( вЂў.вЂў )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:26:16 by UDA                                      */
/*   Updated: 2025/12/01 13:26:16 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ItemLocalization
{
    private static Dictionary<string, LocalizedItem> local;

    public static void Load(string lang)
    {
        string path = Path.Combine(
            Application.streamingAssetsPath,
            "lng", lang, "items.json"
        );

        if (!File.Exists(path))
        {
            Debug.LogError("[ItemLocalization] РќРµС‚ С„Р°Р№Р»Р°: " + path);
            local = new Dictionary<string, LocalizedItem>();
            return;
        }

        string json = File.ReadAllText(path);
        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);

        local = wrapper.items;
        UDADebug.Log($"[ItemLocalization] Р—Р°РіСЂСѓР¶РµРЅРѕ: {local.Count} РїСЂРµРґРјРµС‚РѕРІ РґР»СЏ СЏР·С‹РєР° {lang}");
    }

    public static LocalizedItem Get(string id)
    {
        return (local != null && local.ContainsKey(id)) ? local[id] : null;
    }

    [System.Serializable]
    private class Wrapper
    {
        public Dictionary<string, LocalizedItem> items;
    }
}

[System.Serializable]
public class LocalizedItem
{
    public string name;
    public string description;
}

