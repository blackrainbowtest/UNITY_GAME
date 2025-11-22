using System.Collections.Generic;
using UnityEngine;

public class WorldMapController : MonoBehaviour
{
    // how many tiles are visible around the player
    public int viewRadius = 15;
    public GameObject tilePrefab;
    public Transform tileContainer;
    // Size (world units) of a single tile. Use this to space tiles and avoid overlap.
    public float tileSize = 1f;

    private Dictionary<Vector2Int, GameObject> visibleTiles;
    private WorldGenerator generator;
    private Vector2Int playerTilePos;
    private static GameObject runtimeTilePrefabTemplate = null;

    private void Start()
    {
        // Diagnostic: check for multiple WorldMapController instances in the scene
        var controllers = FindObjectsByType<WorldMapController>(FindObjectsSortMode.None);
        if (controllers != null && controllers.Length > 1)
        {
            string names = "";
            foreach (var c in controllers)
            {
                names += $"{c.gameObject.name} (tilePrefab assigned: {c.tilePrefab != null}), ";
            }
            Debug.LogWarning($"[WorldMap] Multiple WorldMapController instances detected: {names}");

            // Ensure only the first controller runs to prevent duplicate spawning
            if (controllers[0] != this)
            {
                Debug.LogWarning($"[WorldMap] Disabling additional WorldMapController on '{gameObject.name}' to avoid duplicate behavior.");
                this.enabled = false;
                return;
            }
        }

        // Ensure GameManager exists before attempting to load player/save data
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[WorldMap] GameManager отсутствует — загружаю Preloader");
            SceneLoader.LoadScene("Preloader");
            return;
        }

        // If CurrentPlayer isn't set yet, try to load pending save (if any)
        if (GameData.CurrentPlayer == null)
        {
            Debug.LogWarning("[WorldMap] CurrentPlayer отсутствует — пытаюсь загрузить отложенный сейв");
            if (TempSaveCache.pendingSave != null)
            {
                GameManager.Instance.LoadGameData(TempSaveCache.pendingSave);
                TempSaveCache.pendingSave = null;
                Debug.Log("[WorldMap] Отложенный сейв загружен");
            }
        }

        // Now initialize generator using the (possibly newly loaded) player's seed.
        int seed = GameData.CurrentPlayer != null
            ? GameData.CurrentPlayer.worldSeed
            : Random.Range(0, 99999999);
        Debug.Log($"[WorldMap] Using worldSeed = {seed}");
        generator = new WorldGenerator(seed);
        visibleTiles = new Dictionary<Vector2Int, GameObject>();

        TryAutoSaveOnEnter();
        UpdateMapAroundPlayer();

        // For diagnostics: compute a small deterministic region hash to compare between runs
        try
        {
            int hash = ComputeRegionHash(playerTilePos.x, playerTilePos.y, 6);
            Debug.Log($"[WorldMap] Map hash at start (seed={seed}): {hash}");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[WorldMap] Failed to compute map hash: " + ex.Message);
        }
    }

    // Compute a simple deterministic hash of tiles in a square region using the generator's seed.
    private int ComputeRegionHash(int centerX, int centerY, int radius)
    {
        unchecked
        {
            int h = 17;
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    int x = centerX + dx;
                    int y = centerY + dy;
                    var t = generator.GetTile(x, y);
                    // include biomeId, subBiomeId and spriteId into hash
                    h = h * 31 + (t.biomeId != null ? t.biomeId.GetHashCode() : 0);
                    h = h * 31 + (t.subBiomeId != null ? t.subBiomeId.GetHashCode() : 0);
                    h = h * 31 + (t.spriteId != null ? t.spriteId.GetHashCode() : 0);
                    h = h * 31 + Mathf.RoundToInt(t.moveCost * 100f);
                }
            }
            return h;
        }
    }

    private void TryAutoSaveOnEnter()
    {
        // if the player is loaded correctly
        if (GameData.CurrentPlayer != null)
        {
            SaveManager.SaveAuto(GameManager.Instance.GetCurrentGameData());
        }
    }

    void Update()
    {
        // === ВМЕСТО игрока — следуем за позицией КАМЕРЫ ===
        Vector3 camPos = Camera.main.transform.position;

        // Переводим мировые координаты камеры → в координаты тайла
        var newPos = new Vector2Int(
            Mathf.FloorToInt(camPos.x / tileSize),
            Mathf.FloorToInt(camPos.y / tileSize)
        );

        if (newPos != playerTilePos)
        {
            playerTilePos = newPos;
            UpdateMapAroundPlayer();
        }
    }

    private void UpdateMapAroundPlayer()
    {
        HashSet<Vector2Int> desiredTiles = new HashSet<Vector2Int>();

        // Генерация тайлов вокруг центра КАМЕРЫ
        for (int dx = -viewRadius; dx <= viewRadius; dx++)
        {
            for (int dy = -viewRadius; dy <= viewRadius; dy++)
            {
                var pos = new Vector2Int(playerTilePos.x + dx, playerTilePos.y + dy);
                desiredTiles.Add(pos);

                if (!visibleTiles.ContainsKey(pos))
                    SpawnTile(pos);
            }
        }

        // Удаляем тайлы вне зоны видимости
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kv in visibleTiles)
        {
            if (!desiredTiles.Contains(kv.Key))
                toRemove.Add(kv.Key);
        }

        foreach (var pos in toRemove)
        {
            Destroy(visibleTiles[pos]);
            visibleTiles.Remove(pos);
        }
    }

    private void SpawnTile(Vector2Int pos)
    {
        TileData data = generator.GetTile(pos.x, pos.y);

        // Ensure we have a prefab to instantiate. Try Resources, else create a runtime placeholder template.
        if (tilePrefab == null)
        {
            tilePrefab = Resources.Load<GameObject>("WorldData/TilePrefab") ?? Resources.Load<GameObject>("TilePrefab");
            if (tilePrefab == null)
            {
                // Create runtime placeholder once
                if (runtimeTilePrefabTemplate == null)
                {
                    Debug.LogWarning($"[WorldMap] tilePrefab is not assigned and no fallback TilePrefab found in Resources. Creating runtime placeholder tile prefab.");
                    runtimeTilePrefabTemplate = new GameObject("TilePrefabRuntime");
                    var sr = runtimeTilePrefabTemplate.AddComponent<SpriteRenderer>();
                    var trComp = runtimeTilePrefabTemplate.AddComponent<TileRenderer>();
                    trComp.sr = sr;

                    // try to auto-load TileSpriteDB from resources
                    TileSpriteDB db = Resources.Load<TileSpriteDB>("WorldData/TileSpriteDB") ?? Resources.Load<TileSpriteDB>("TileSpriteDB");
                    if (db == null)
                    {
                        db = ScriptableObject.CreateInstance<TileSpriteDB>();
                        Debug.Log("[WorldMap] Created runtime empty TileSpriteDB for placeholder tiles.");
                    }
                    else
                    {
                        Debug.Log("[WorldMap] Loaded TileSpriteDB from Resources for runtime placeholder.");
                    }
                    trComp.spriteDB = db;
                    // keep the runtime template inactive so instantiated tiles are not visible until positioned
                    runtimeTilePrefabTemplate.SetActive(false);
                }

                tilePrefab = runtimeTilePrefabTemplate;
            }
            else
            {
                Debug.Log("[WorldMap] Auto-loaded TilePrefab from Resources.");
            }
        }

        Transform parent = tileContainer != null ? tileContainer : this.transform;
        GameObject obj = Instantiate(tilePrefab, parent);
        if (obj == null)
        {
            Debug.LogError($"[WorldMap] Instantiate returned null for tilePrefab at {pos}.");
            return;
        }

        obj.transform.localPosition = new Vector3(pos.x * tileSize, pos.y * tileSize, 0);
        obj.transform.localScale = new Vector3(tileSize, tileSize, 1f);
        obj.SetActive(true);

        var tr = obj.GetComponent<TileRenderer>();
        if (tr == null)
        {
            Debug.LogWarning($"[WorldMap] Spawned tile at {pos} has no TileRenderer component.");
        }
        else
        {
            tr.RenderTile(data);
        }

        visibleTiles[pos] = obj;
    }
}
