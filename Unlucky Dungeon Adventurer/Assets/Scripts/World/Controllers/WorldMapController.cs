using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// WorldMapController is responsible for:
/// - Creating and managing world tiles around the camera.
/// - Maintaining a cache of visible tiles to avoid redundant instantiation.
/// - Using WorldGenerator to fetch TileData (biome, sub-biomes, structures).
/// - Rendering tiles via TileRenderer (multi-layer rendering).
/// - Updating minimap (if assigned).
/// - Tracking player position and updating UI.
/// 
/// This controller does NOT handle:
/// - Tile art logic (TileGenerator + TileRenderer handle that)
/// - Biome/SubBiome logic (BiomeInfluence, BiomeMaskUtils)
/// - Save/load logic (GameManager, SaveManager)
///
/// Architecture goal:
/// A clean MVC-like approach where:
/// - WorldGenerator = MODEL
/// - TileRenderer   = VIEW
/// - WorldMapCtrl   = CONTROLLER
/// </summary>
public class WorldMapController : MonoBehaviour
{
    // ============================================================
    // UI & References
    // ============================================================

    [Header("UI")]
    public TextMeshProUGUI locationText;

    [Header("Tile Rendering Settings")]
    public int viewRadius = 15;          // how many tiles around camera to render
    public GameObject tilePrefab;        // base tile prefab (must contain TileRenderer)
    public Transform tileContainer;      // parent for tile instances
    public float tileSize = 1f;          // world size of a tile

    [Header("Player Marker")]
    public PlayerMarkerController playerMarker;

    [Header("Minimap")]
    [SerializeField] private MinimapController minimap;

    // ============================================================
    // Internal State
    // ============================================================

    private Dictionary<Vector2Int, GameObject> visibleTiles; // active world tiles
    private WorldGenerator generator;                        // generates TileData
    private Vector2Int playerTilePos;                        // tile under camera
    private bool waitingForPlayer = false;

    private static GameObject runtimeTilePrefabTemplate = null;


    // ============================================================
    // Initialization
    // ============================================================

    private void Start()
    {
        // Protect from duplicate controllers
        var controllers = FindObjectsByType<WorldMapController>(FindObjectsSortMode.None);
        if (controllers.Length > 1 && controllers[0] != this)
        {
            Debug.LogWarning($"[WorldMap] Duplicate controller found – disabling this instance.");
            this.enabled = false;
            return;
        }

        // If game not initialized yet — wait for event
        if (!GameInitializer.IsInitialized())
        {
            Debug.Log("[WorldMap] Waiting for GameInitializer...");
            UIEvents.OnGameInitialized += OnGameInitialized;
            return;
        }

        ContinueInitialization();
    }

    private void OnGameInitialized()
    {
        UIEvents.OnGameInitialized -= OnGameInitialized;
        ContinueInitialization();
    }


    // ============================================================
    // Main Initialization (after GameManager & Player loaded)
    // ============================================================

    private void ContinueInitialization()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[WorldMap] GameManager missing — returning to Preloader scene.");
            SceneLoader.LoadScene("Preloader");
            return;
        }

        // Wait for player data
        if (GameData.CurrentPlayer == null)
        {
            if (!waitingForPlayer)
            {
                waitingForPlayer = true;
                Debug.Log("[WorldMap] Waiting for CurrentPlayer...");
                UIEvents.OnPlayerLoaded += OnPlayerLoaded;
            }
            return;
        }

        // If loading from autosave-in-progress
        if (TempSaveCache.pendingSave != null)
        {
            Debug.Log("[WorldMap] Applying pending save...");
            GameManager.Instance.LoadGameData(TempSaveCache.pendingSave);
            TempSaveCache.pendingSave = null;
            UIEvents.InvokePlayerLoaded();
        }

        // Validate seed
        int seed = GameData.CurrentPlayer.worldSeed;
        if (seed < 10000)
        {
            Debug.LogError("[WorldMap] INVALID SEED (<10000). Forcing seed=10000.");
            seed = 10000;
        }

        Debug.Log($"[WorldMap] Initializing world with seed = {seed}");

        // 1) Create world generator
        generator = new WorldGenerator(seed);

        // 2) Prepare cache for active tiles
        visibleTiles = new Dictionary<Vector2Int, GameObject>();

        // 3) Init minimap
        Vector2Int playerTile = new Vector2Int(
            Mathf.RoundToInt(GameData.CurrentPlayer.mapPosX),
            Mathf.RoundToInt(GameData.CurrentPlayer.mapPosY)
        );

        if (minimap != null)
        {
            minimap.Initialize(viewRadius, playerTile);
            minimap.OnMinimapTileClicked += HandleMinimapClick;
            minimap.OnMinimapCenterDragged += HandleMinimapDrag;
        }

        // 4) Autosave
        TryAutoSaveOnEnter();

        // 5) Populate world around camera
        UpdateMapAroundCamera();

        // 6) Create player marker
        InitializePlayerMarker();

        // 7) Initial minimap render
        RefreshMinimapTiles();
        minimap?.DrawPlayerMarker(playerTile);
    }

    private void OnPlayerLoaded()
    {
        UIEvents.OnPlayerLoaded -= OnPlayerLoaded;
        waitingForPlayer = false;
        ContinueInitialization();
    }


    // ============================================================
    // Per-frame updates (camera movement)
    // ============================================================

    private void Update()
    {
        if (generator == null) return;

        Vector3 camPos = Camera.main.transform.position;

        Vector2Int newPos = new Vector2Int(
            Mathf.FloorToInt(camPos.x / tileSize),
            Mathf.FloorToInt(camPos.y / tileSize)
        );

        if (newPos != playerTilePos)
        {
            playerTilePos = newPos;
            UpdateMapAroundCamera();

            minimap?.SetCenter(playerTilePos);
            RefreshMinimapTiles();
        }

        UpdateMinimapDisplay();
    }


    // ============================================================
    // Minimap Core
    // ============================================================

    private void UpdateMinimapDisplay()
    {
        if (minimap == null) return;

        minimap.Clear();
        RefreshMinimapTiles();

        Vector2Int cameraBounds = GetCameraViewBoundsInTiles();
        Vector2Int camMin = playerTilePos - cameraBounds;
        Vector2Int camMax = playerTilePos + cameraBounds;

        minimap.DrawCameraFrame(camMin, camMax);

        if (GameData.CurrentPlayer != null)
        {
            Vector2Int actualPlayerPos = new(
                Mathf.RoundToInt(GameData.CurrentPlayer.mapPosX),
                Mathf.RoundToInt(GameData.CurrentPlayer.mapPosY)
            );
            minimap.DrawPlayerMarker(actualPlayerPos);
        }
    }


    // ============================================================
    // Main tile update logic (spawning & removing tiles)
    // ============================================================

    private void UpdateMapAroundCamera()
    {
        HashSet<Vector2Int> desiredTiles = new();

        for (int dx = -viewRadius; dx <= viewRadius; dx++)
        {
            for (int dy = -viewRadius; dy <= viewRadius; dy++)
            {
                Vector2Int pos = new(playerTilePos.x + dx, playerTilePos.y + dy);
                desiredTiles.Add(pos);

                if (!visibleTiles.ContainsKey(pos))
                    SpawnTile(pos);
            }
        }

        // Remove tiles that left the view radius
        List<Vector2Int> toRemove = new();
        foreach (var kv in visibleTiles)
        {
            if (!desiredTiles.Contains(kv.Key))
                toRemove.Add(kv.Key);
        }

        foreach (Vector2Int pos in toRemove)
        {
            Destroy(visibleTiles[pos]);
            visibleTiles.Remove(pos);
        }
    }

    // ============================================================
    // Tile spawning (instantiating the prefab + rendering)
    // ============================================================

    private void SpawnTile(Vector2Int pos)
    {
        // 1) Acquire tile data
        TileData data = generator.GetTile(pos.x, pos.y);

        // 2) Ensure tile prefab available
        if (tilePrefab == null)
        {
            tilePrefab = Resources.Load<GameObject>("WorldData/TilePrefab") ??
                         Resources.Load<GameObject>("TilePrefab");

            if (tilePrefab == null)
            {
                if (runtimeTilePrefabTemplate == null)
                {
                    Debug.LogWarning("[WorldMap] No tilePrefab assigned. Creating runtime prefab template.");
                    runtimeTilePrefabTemplate = new GameObject("TilePrefabRuntime");

                    // Add TileRenderer only; it will auto-create internal render layers
                    runtimeTilePrefabTemplate.AddComponent<TileRenderer>();

                    runtimeTilePrefabTemplate.SetActive(false);
                }

                tilePrefab = runtimeTilePrefabTemplate;
            }
        }

        // 3) Instantiate tile
        Transform parent = tileContainer != null ? tileContainer : this.transform;
        GameObject obj = Instantiate(tilePrefab, parent);

        obj.transform.localPosition = new Vector3(pos.x * tileSize, pos.y * tileSize, 0);
        obj.transform.localScale = new Vector3(tileSize, tileSize, 1f);
        obj.SetActive(true);

        // 4) Render using new multi-layer TileRenderer
        TileRenderer tr = obj.GetComponent<TileRenderer>();
        if (tr == null)
        {
            Debug.LogError("[WorldMap] TilePrefab missing TileRenderer!");
        }
        else
        {
            tr.RenderTile(data);
        }

        // 5) Update minimap tile color
        minimap?.UpdateTile(pos, data.color);

        visibleTiles[pos] = obj;
    }


    // ============================================================
    // Player marker & UI
    // ============================================================

    private void InitializePlayerMarker()
    {
        if (playerMarker == null)
        {
            Debug.LogWarning("[WorldMap] PlayerMarker not assigned!");
            return;
        }

        var p = GameData.CurrentPlayer;
        Vector2Int coords = new(
            Mathf.RoundToInt(p.mapPosX),
            Mathf.RoundToInt(p.mapPosY)
        );

        playerMarker.SetPosition(coords, tileSize);
        UpdatePlayerLocationInfo();
    }

    private void UpdatePlayerLocationInfo()
    {
        if (locationText == null || generator == null)
            return;

        var p = GameData.CurrentPlayer;
        if (p == null) return;

        Vector2Int coords = new(
            Mathf.RoundToInt(p.mapPosX),
            Mathf.RoundToInt(p.mapPosY)
        );

        TileData tile = generator.GetTile(coords.x, coords.y);

        // Get localized biome name
        LanguageManager.LoadLanguage("biomes_" + LanguageManager.CurrentLanguage);
        string biomeName = LanguageManager.Get(tile.biomeId);

        // Determine dominant sub-biome (if any)
        string subBiome = "";
        if (tile.subBiomeIds != null && tile.subBiomeIds.Count > 0)
        {
            subBiome = LanguageManager.Get(tile.subBiomeIds[0]);
        }

        locationText.text = $"{biomeName} {subBiome}\n(X: {coords.x}, Y: {coords.y})";
    }


    // ============================================================
    // Minimap helpers
    // ============================================================

    private Vector2Int GetCameraViewBoundsInTiles()
    {
        if (Camera.main == null) return Vector2Int.zero;

        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;

        int tilesWidth = Mathf.CeilToInt(camWidth / tileSize / 2f);
        int tilesHeight = Mathf.CeilToInt(camHeight / tileSize / 2f);

        return new Vector2Int(tilesWidth, tilesHeight);
    }

    private void RefreshMinimapTiles()
    {
        if (minimap == null || generator == null) return;

        for (int dx = -viewRadius; dx <= viewRadius; dx++)
        {
            for (int dy = -viewRadius; dy <= viewRadius; dy++)
            {
                Vector2Int pos = new(playerTilePos.x + dx, playerTilePos.y + dy);
                TileData tileData = generator.GetTile(pos.x, pos.y);

                minimap.UpdateTile(pos, tileData.color);
            }
        }
    }


    // ============================================================
    // Camera Control
    // ============================================================

    private void HandleMinimapClick(Vector2Int tilePos)
    {
        MoveCameraTo(tilePos);
    }

    private void MoveCameraTo(Vector2Int worldTile)
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("[WorldMap] MainCamera not found!");
            return;
        }

        Vector3 target = new(
            worldTile.x * tileSize,
            worldTile.y * tileSize,
            Camera.main.transform.position.z
        );

        Camera.main.transform.position =
            Vector3.Lerp(Camera.main.transform.position, target, 0.3f);

        Vector2Int newTilePos = new(
            Mathf.FloorToInt(Camera.main.transform.position.x / tileSize),
            Mathf.FloorToInt(Camera.main.transform.position.y / tileSize)
        );

        if (newTilePos != playerTilePos)
        {
            playerTilePos = newTilePos;
            UpdateMapAroundCamera();
            minimap?.SetCenter(playerTilePos);
            RefreshMinimapTiles();
        }

        if (GameData.CurrentPlayer != null)
        {
            Vector2Int actual = new(
                Mathf.RoundToInt(GameData.CurrentPlayer.mapPosX),
                Mathf.RoundToInt(GameData.CurrentPlayer.mapPosY)
            );
            minimap?.DrawPlayerMarker(actual);
        }
    }

    // NEW: плавное перетаскивание миникарты
    private void HandleMinimapDrag(Vector2 worldCenter)
    {
        if (Camera.main == null)
            return;

        // worldCenter — координаты центра карты в тайлах (могут быть дробные)
        float tx = worldCenter.x;
        float ty = worldCenter.y;

        // конвертация тайлов → мировые координаты
        Vector3 target = new Vector3(
            tx * tileSize,
            ty * tileSize,
            Camera.main.transform.position.z
        );

        // Плавное перемещение камеры к точке
        Camera.main.transform.position = Vector3.Lerp(
            Camera.main.transform.position,
            target,
            0.15f   // степень плавности
        );

        // Обновление логики карты вокруг камеры
        Vector2Int newTilePos = new(
            Mathf.FloorToInt(Camera.main.transform.position.x / tileSize),
            Mathf.FloorToInt(Camera.main.transform.position.y / tileSize)
        );

        if (newTilePos != playerTilePos)
        {
            playerTilePos = newTilePos;
            UpdateMapAroundCamera();
            minimap?.SetCenter(playerTilePos);
            RefreshMinimapTiles();
        }
    }


    // ============================================================
    // Autosave
    // ============================================================

    private void TryAutoSaveOnEnter()
    {
        if (GameData.CurrentPlayer != null &&
            GameData.CurrentPlayer.worldSeed >= 10000)
        {
            SaveManager.SaveAuto(GameManager.Instance.GetCurrentGameData());
        }
    }
}
