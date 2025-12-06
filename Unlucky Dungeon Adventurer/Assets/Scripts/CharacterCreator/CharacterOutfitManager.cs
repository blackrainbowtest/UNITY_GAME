using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Simple JSON-driven outfit support: outfit definitions (id -> parts keys)
// and a sprite key -> Sprite mapping you fill in the Inspector.

public class CharacterOutfitManager : MonoBehaviour
{
    [Header("Character Layers")]
    public Image legs;

    [Header("Leg Sprites")]
    public Sprite legsPaladin;
    public Sprite legsRogue;
    public Sprite legsSlave;
    public Sprite legsHermit;
    [Header("Optional: Outfits JSON (Resources/outfits.json or assign TextAsset)")]
    // If left empty, the script will try to load `Resources/outfits.json`.
    public TextAsset outfitsJson;

    [Header("Sprite entries (key -> sprite)")]
    // Fill this with keys referenced from JSON (e.g. "Paladin", "Rogue"...)
    public SpriteEntry[] spriteEntries;

    // Runtime maps
    private Dictionary<string, Sprite> spriteMap;
    private OutfitCollection outfitsCollection;

    private void Awake()
    {
        InitializeOutfits();
    }

    private void OnValidate()
    {
        // Auto-populate spriteEntries from hardcoded sprite fields (Editor only)
        if (spriteEntries == null || spriteEntries.Length == 0)
        {
            spriteEntries = new SpriteEntry[4];
            spriteEntries[0] = new SpriteEntry { key = "Paladin", sprite = legsPaladin };
            spriteEntries[1] = new SpriteEntry { key = "Rogue", sprite = legsRogue };
            spriteEntries[2] = new SpriteEntry { key = "Slave", sprite = legsSlave };
            spriteEntries[3] = new SpriteEntry { key = "Hermit", sprite = legsHermit };
        }
    }

    private void InitializeOutfits()
    {
        // Load JSON if not assigned
        if (outfitsJson == null)
        {
            outfitsJson = Resources.Load<TextAsset>("WorldData/outfits");
        }

        outfitsCollection = OutfitUtils.LoadOutfits(outfitsJson);

        // Build sprite map
        spriteMap = OutfitUtils.BuildSpriteMap(spriteEntries);

        // Diagnostic: report hardcoded legs sprite fields so we can see if they're assigned at runtime
        UDADebug.Log($"[OutfitManager] Hardcoded legs sprites - Paladin: {(legsPaladin!=null?legsPaladin.name:"null")}, Rogue: {(legsRogue!=null?legsRogue.name:"null")}, Slave: {(legsSlave!=null?legsSlave.name:"null")}, Hermit: {(legsHermit!=null?legsHermit.name:"null")} ");

        // Ensure common keys exist in the sprite map by falling back to the hardcoded fields.
        OutfitUtils.TryAddFallback(spriteMap, "Paladin", legsPaladin);
        OutfitUtils.TryAddFallback(spriteMap, "Rogue", legsRogue);
        OutfitUtils.TryAddFallback(spriteMap, "Slave", legsSlave);
        OutfitUtils.TryAddFallback(spriteMap, "Hermit", legsHermit);
    }

    // Apply by index (dropdown index).
    public void ApplySet(int index)
    {
        UDADebug.Log($"[OutfitManager] ApplySet(int) called with index: {index}");

        // Try JSON-defined outfit first (log-only will still check and report)
        if (outfitsCollection != null && outfitsCollection.outfits != null && index >= 0 && index < outfitsCollection.outfits.Length)
        {
            var def = outfitsCollection.outfits[index];
            if (def != null && !string.IsNullOrEmpty(def.legs))
            {
                UDADebug.Log($"[OutfitManager] Found outfit: id={def.id}, legs key={def.legs}");
                if (spriteMap != null && spriteMap.TryGetValue(def.legs, out var sp))
                {
                    legs.sprite = sp;
                    UDADebug.Log($"[OutfitManager] Applied sprite from JSON: {sp.name}");
                    return;
                }
                else
                {
                    // Try to load the sprite by key from Resources as a runtime fallback
                    var resSp = Resources.Load<Sprite>(def.legs);
                    if (resSp != null)
                    {
                        spriteMap[def.legs] = resSp;
                        legs.sprite = resSp;
                        UDADebug.Log($"[OutfitManager] Loaded and applied sprite from Resources: {resSp.name}");
                        return;
                    }

                    Debug.LogWarning($"[OutfitManager] Sprite key '{def.legs}' not found in sprite map!");
                }
            }
            else
            {
                Debug.LogWarning($"[OutfitManager] Outfit definition at index {index} is null or has no legs key!");
            }
        }
        else
        {
            Debug.LogWarning($"[OutfitManager] JSON collection is null or index {index} out of bounds!");
        }

        // Determine fallback sprite
        Sprite fallback = null;
        switch (index)
        {
            case 0: fallback = legsPaladin; break;
            case 1: fallback = legsRogue; break;
            case 2: fallback = legsSlave; break;
            case 3: fallback = legsHermit; break;
        }

        if (fallback != null)
        {
            legs.sprite = fallback;
            UDADebug.Log($"[OutfitManager] Applied legacy fallback sprite: {fallback.name}");
        }
        else
        {
            // Try runtime load from Resources using conventional keys
            string key = null;
            switch (index)
            {
                case 0: key = "Paladin"; break;
                case 1: key = "Rogue"; break;
                case 2: key = "Slave"; break;
                case 3: key = "Hermit"; break;
            }
            if (!string.IsNullOrEmpty(key))
            {
                var res = Resources.Load<Sprite>(key);
                if (res != null)
                {
                    spriteMap[key] = res;
                    legs.sprite = res;
                    UDADebug.Log($"[OutfitManager] Loaded and applied fallback sprite from Resources: {res.name}");
                    return;
                }
            }

            Debug.LogWarning($"[OutfitManager] No fallback sprite defined for index {index}");
        }
    }

    // Apply by outfit id or legacy name.
    public void ApplySet(string setName)
    {
        UDADebug.Log($"[OutfitManager] ApplySet(string) called with setName: '{setName}'");

        // Try to find outfit by id in JSON (LogOnly will still check and report)
        if (outfitsCollection != null && outfitsCollection.outfits != null && !string.IsNullOrEmpty(setName))
        {
            foreach (var def in outfitsCollection.outfits)
            {
                if (def != null && def.id == setName)
                {
                    if (!string.IsNullOrEmpty(def.legs) && spriteMap != null && spriteMap.TryGetValue(def.legs, out var sp))
                    {
                        legs.sprite = sp;
                        UDADebug.Log($"[OutfitManager] Applied sprite from JSON by id: {sp.name}");
                        return;
                    }
                    else
                    {
                        Debug.LogWarning($"[OutfitManager] Sprite key '{def?.legs}' not found in sprite map for id '{setName}'!");
                    }
                }
            }
        }

        // Legacy string matching fallback
        Sprite fallback = null;
        switch (setName)
        {
            case "Paladin": fallback = legsPaladin; break;
            case "Rogue": fallback = legsRogue; break;
            case "Slave": fallback = legsSlave; break;
            case "Hermit": fallback = legsHermit; break;
        }

        if (fallback != null)
        {
            legs.sprite = fallback;
            UDADebug.Log($"[OutfitManager] Applied legacy fallback sprite for id '{setName}': {fallback.name}");
        }
        else
        {
            Debug.LogWarning($"[OutfitManager] No legacy fallback sprite defined for id '{setName}'");
        }
    }
}

