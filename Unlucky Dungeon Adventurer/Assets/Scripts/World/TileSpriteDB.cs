using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSpriteDB", menuName = "World/TileSpriteDB")]
public class TileSpriteDB : ScriptableObject
{
    public List<Entry> entries;

    [System.Serializable]
    public struct Entry
    {
        public string id;
        public Sprite sprite;
    }

    private Dictionary<string, Sprite> map;

    public Sprite Get(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        // lazy init
        if (map == null)
        {
            map = new Dictionary<string, Sprite>();
            foreach (var e in entries)
                map[e.id] = e.sprite;
        }

        map.TryGetValue(id, out Sprite s);
        return s;
    }
}
