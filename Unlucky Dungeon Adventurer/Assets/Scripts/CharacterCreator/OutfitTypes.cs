using System;
using UnityEngine;

// Serializable types used by the outfit system.
[Serializable]
public struct SpriteEntry
{
    public string key;
    public Sprite sprite;
}

[Serializable]
public class OutfitDefinition
{
    public string id;
    public string legs;
    public string chest;
    public string hands;
    public string head;
}

[Serializable]
public class OutfitCollection
{
    public OutfitDefinition[] outfits;
}
