#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class TileSpriteDBTools
{
    private const string DefaultDBPath = "Assets/Resources/WorldData/TileSpriteDB.asset";
    private const string RequiredIdsPath = "Assets/Resources/WorldData/required_tile_sprite_ids.txt";

    [MenuItem("Tools/World/TileSpriteDB/Populate From Folder...")]
    public static void PopulateFromFolder()
    {
        string abs = EditorUtility.OpenFolderPanel("Select folder with tile sprites", Application.dataPath, "");
        if (string.IsNullOrEmpty(abs)) return;

        if (!abs.Replace('\\','/').StartsWith(Application.dataPath.Replace('\\','/')))
        {
            EditorUtility.DisplayDialog("Invalid folder", "Please select a folder inside this Unity project (Assets/).", "OK");
            return;
        }

        string rel = "Assets" + abs.Replace('\\','/').Substring(Application.dataPath.Replace('\\','/').Length);
        var sprites = FindSpritesInFolder(rel);

        var db = LoadOrCreateDB();
        int updated = 0, added = 0;

        var map = db.entries.ToDictionary(e => e.id, e => e);
        foreach (var sp in sprites)
        {
            string id = sp.name; // must match TileData.spriteId exactly
            if (map.TryGetValue(id, out var entry))
            {
                if (entry.sprite != sp)
                {
                    entry.sprite = sp;
                    map[id] = entry;
                    updated++;
                }
            }
            else
            {
                map[id] = new TileSpriteDB.Entry { id = id, sprite = sp };
                added++;
            }
        }

        db.entries = map.Values.OrderBy(e => e.id).ToList();
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("TileSpriteDB", $"Populate complete\nAdded: {added}\nUpdated: {updated}\nTotal entries: {db.entries.Count}", "OK");
    }

    [MenuItem("Tools/World/TileSpriteDB/Sync From required_tile_sprite_ids.txt")]
    public static void SyncFromRequiredIds()
    {
        var db = LoadOrCreateDB();
        var idsAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(RequiredIdsPath);
        if (idsAsset == null)
        {
            EditorUtility.DisplayDialog("Missing file", $"Not found: {RequiredIdsPath}", "OK");
            return;
        }

        var requiredIds = ParseIds(idsAsset.text);
        var spritesByName = FindAllSpritesByName();

        var map = db.entries.ToDictionary(e => e.id, e => e);
        int added = 0, linked = 0, missing = 0;

        foreach (var id in requiredIds)
        {
            spritesByName.TryGetValue(id, out var sp);
            if (map.TryGetValue(id, out var entry))
            {
                if (sp != null && entry.sprite != sp)
                {
                    entry.sprite = sp;
                    map[id] = entry;
                    linked++;
                }
            }
            else
            {
                map[id] = new TileSpriteDB.Entry { id = id, sprite = sp };
                if (sp != null) linked++; else missing++;
                added++;
            }
        }

        db.entries = map.Values.OrderBy(e => e.id).ToList();
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "TileSpriteDB",
            $"Sync complete\nIDs: {requiredIds.Count}\nAdded entries: {added}\nLinked sprites: {linked}\nMissing sprites: {missing}",
            "OK");
    }

    private static TileSpriteDB LoadOrCreateDB()
    {
        var db = AssetDatabase.LoadAssetAtPath<TileSpriteDB>(DefaultDBPath);
        if (db != null) return db;

        // Try find any TileSpriteDB
        string guid = AssetDatabase.FindAssets("t:TileSpriteDB").FirstOrDefault();
        if (!string.IsNullOrEmpty(guid))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            db = AssetDatabase.LoadAssetAtPath<TileSpriteDB>(path);
            if (db != null) return db;
        }

        // Create new at default path if missing
        db = ScriptableObject.CreateInstance<TileSpriteDB>();
        Directory.CreateDirectory(Path.GetDirectoryName(DefaultDBPath));
        AssetDatabase.CreateAsset(db, DefaultDBPath);
        AssetDatabase.SaveAssets();
        return db;
    }

    private static List<Sprite> FindSpritesInFolder(string folder)
    {
        var list = new List<Sprite>();
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folder });
        foreach (var g in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            var sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sp != null) list.Add(sp);
        }
        return list;
    }

    private static Dictionary<string, Sprite> FindAllSpritesByName()
    {
        var dict = new Dictionary<string, Sprite>();
        string[] guids = AssetDatabase.FindAssets("t:Sprite");
        foreach (var g in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            var sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sp == null) continue;
            if (!dict.ContainsKey(sp.name))
                dict[sp.name] = sp;
        }
        return dict;
    }

    private static List<string> ParseIds(string text)
    {
        var result = new List<string>();
        using (var sr = new StringReader(text))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("#")) continue;
                result.Add(line);
            }
        }
        return result;
    }
}
#endif
