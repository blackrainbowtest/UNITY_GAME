using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq; // For OfType and ToArray LINQ extensions
using System.Collections.Generic;
using UnityEditor.U2D.Sprites; // New sprite slicing API

public class TileSet47AutoGenerator : EditorWindow
{
    private Texture2D sourceTexture;
    private string biomeId = "forest";
    private string outputFolder = "Assets/Resources/WorldData/Tilesets";

    [MenuItem("Tools/Autotile/Generate TileSet47")]
    private static void OpenWindow()
    {
        GetWindow<TileSet47AutoGenerator>("TileSet47 Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("47-Tile Autotile Generator", EditorStyles.boldLabel);

        sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Autotile Texture", sourceTexture, typeof(Texture2D), false);
        biomeId = EditorGUILayout.TextField("Biome ID", biomeId);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

        if (GUILayout.Button("Generate TileSet47"))
        {
            if (sourceTexture == null)
            {
                EditorUtility.DisplayDialog("Error", "Select source texture!", "OK");
                return;
            }

            GenerateTileSet47();
        }
    }

    private void GenerateTileSet47()
    {
        string path = AssetDatabase.GetAssetPath(sourceTexture);
        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);

        if (importer == null)
        {
            EditorUtility.DisplayDialog("Error", "TextureImporter not found!", "OK");
            return;
        }

        // --- РќР°СЃС‚СЂРѕР№РєР° С‚РµРєСЃС‚СѓСЂС‹ РїРѕРґ slicing ---
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;

        int sliceSize = sourceTexture.width / 8; // РїСЂРµРґРїРѕР»Р°РіР°РµРј 8x6 РёР»Рё 8x_(РґРѕ 48)
        int columns = 8;
        int rows = Mathf.CeilToInt(sourceTexture.height / (float)sliceSize);

        // --- Legacy slicing approach (using spritesheet) with warning suppressed ---
        SpriteMetaData[] metas = new SpriteMetaData[47];
        int index = 0;
        for (int y = rows - 1; y >= 0 && index < 47; y--)
        {
            for (int x = 0; x < columns && index < 47; x++)
            {
                metas[index] = new SpriteMetaData
                {
                    name = $"{biomeId}_autotile_{index}",
                    rect = new Rect(x * sliceSize, y * sliceSize, sliceSize, sliceSize),
                    alignment = (int)SpriteAlignment.Center,
                    pivot = new Vector2(0.5f, 0.5f)
                };
                index++;
            }
        }

#pragma warning disable 618
        importer.spritesheet = metas; // Using legacy API for broad Unity version compatibility
#pragma warning restore 618
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        // --- РЎРѕР·РґР°РЅРёРµ TileSet47 ---
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        string assetPath = $"{outputFolder}/{biomeId}_TileSet47.asset";

        TileSet47 tileset = ScriptableObject.CreateInstance<TileSet47>();
        tileset.tiles = new Sprite[47];

        // Remove unused spritePath loop (handled by direct name matching below)

        Sprite[] allSprites = AssetDatabase.LoadAllAssetsAtPath(path)
            .OfType<Sprite>()
            .ToArray();

        for (int i = 0; i < 47; i++)
        {
            string spriteName = $"{biomeId}_autotile_{i}";
            tileset.tiles[i] = System.Array.Find(allSprites, s => s.name == spriteName);

            if (tileset.tiles[i] == null)
                Debug.LogWarning($"[TileSet47Auto] Missing sprite index {i}");
        }

        AssetDatabase.CreateAsset(tileset, assetPath);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Success", $"TileSet47 generated:\n{assetPath}", "OK");
        UDADebug.Log($"[TileSet47Auto] SUCCESS! Created {assetPath}");
    }
}

