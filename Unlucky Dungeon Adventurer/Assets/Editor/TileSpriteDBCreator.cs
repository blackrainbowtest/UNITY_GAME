using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public static class TileSpriteDBCreator
{
    [MenuItem("Tools/Generate TileSpriteDB from required ids")]
    public static void GenerateTileSpriteDB()
    {
        string txtPath = "Assets/Resources/WorldData/required_tile_sprite_ids.txt";
        if (!File.Exists(txtPath))
        {
            Debug.LogError($"[TileSpriteDBCreator] required ids file not found: {txtPath}");
            return;
        }

        var lines = File.ReadAllLines(txtPath);
        var ids = new List<string>();
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;
            if (line.StartsWith("#")) continue;
            ids.Add(line);
        }

        if (ids.Count == 0)
        {
            Debug.LogWarning("[TileSpriteDBCreator] No ids found in required_tile_sprite_ids.txt");
        }

        // Create folder if missing
        string outDir = "Assets/Resources/WorldData";
        if (!AssetDatabase.IsValidFolder(outDir))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.CreateFolder("Assets/Resources", "WorldData");
        }

        string assetPath = outDir + "/TileSpriteDB.asset";

        // If an existing asset exists, back it up by renaming
        var existing = AssetDatabase.LoadAssetAtPath<TileSpriteDB>(assetPath);
        if (existing != null)
        {
            string backupPath = outDir + "/TileSpriteDB.backup.asset";
            AssetDatabase.DeleteAsset(backupPath);
            AssetDatabase.CopyAsset(assetPath, backupPath);
            Debug.Log($"[TileSpriteDBCreator] Existing TileSpriteDB backed up to {backupPath}");
        }

        var db = ScriptableObject.CreateInstance<TileSpriteDB>();
        db.entries = new List<TileSpriteDB.Entry>();

        foreach (var id in ids)
        {
            TileSpriteDB.Entry e = new TileSpriteDB.Entry();
            e.id = id;
            e.sprite = null;
            db.entries.Add(e);
        }

        AssetDatabase.CreateAsset(db, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[TileSpriteDBCreator] Created TileSpriteDB at {assetPath} with {ids.Count} entries. Open the asset and assign sprites for each id.");
        Selection.activeObject = db;
    }
}
#endif
