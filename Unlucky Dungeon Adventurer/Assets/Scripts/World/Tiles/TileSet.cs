using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TileSet", menuName = "World/Tileset", order = 1)]
public class TileSet : ScriptableObject
{
    [Header("Tileset atlas (sliced sprites)")]
    public Sprite[] tiles;

    [Header("Mask → Tile Index")]
    public List<MaskMapping> mappings = new();

    private Dictionary<byte, int> _lookup;

    [Serializable]
    public struct MaskMapping
    {
        public byte mask;
        public int index;
    }

    private void OnEnable()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        _lookup = new Dictionary<byte, int>();

        foreach (var m in mappings)
        {
            if (!_lookup.ContainsKey(m.mask))
                _lookup.Add(m.mask, m.index);
        }
    }

    public int GetIndexForMask(byte mask)
    {
        if (_lookup == null)
            BuildLookup();

        if (_lookup.TryGetValue(mask, out int index))
            return index;

        return 0; // fallback
    }
}
