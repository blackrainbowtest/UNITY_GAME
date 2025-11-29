#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

public class TileSpriteDBAutoSync : AssetPostprocessor
{
    private const string RequiredIdsPath = "Assets/Resources/WorldData/required_tile_sprite_ids.txt";

    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        bool changedRequired = importedAssets.Any(p => p == RequiredIdsPath) || movedAssets.Any(p => p == RequiredIdsPath);
        bool anySpriteChanged = importedAssets.Any(IsSpriteAsset) || movedAssets.Any(IsSpriteAsset);

        if (changedRequired || anySpriteChanged)
        {
            TileSpriteDBTools.SyncFromRequiredIds();
        }
    }

    private static bool IsSpriteAsset(string path)
    {
        // Cheap check: run only for assets under Assets/ and with typical image extensions
        if (string.IsNullOrEmpty(path) || !path.StartsWith("Assets/")) return false;
        string ext = System.IO.Path.GetExtension(path).ToLowerInvariant();
        return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".psd";
    }
}
#endif
