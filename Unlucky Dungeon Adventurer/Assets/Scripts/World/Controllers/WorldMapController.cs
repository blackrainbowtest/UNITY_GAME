/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/World/Controllers/WorldMapController.cs             */
/*                                                        /\_/\               */
/*                                                       ( вЂў.вЂў )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 09:47:31 by UDA                                      */
/*   Updated: 2025/12/02 09:47:31 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using WorldLogic;

/// <summary>
/// WorldMapController is responsible for:
/// - Creating and managing world tiles around the camera.
/// - Maintaining a cache of visible tiles to avoid redundant instantiation.
/// - Using WorldGenerator to fetch TileData (biomes, edge blending, structures).
/// - Rendering tiles via TileRenderer (multi-layer rendering).
/// - Updating minimap (if assigned).
/// - Tracking player position and updating UI.
///
/// This controller does NOT handle:
/// - Tile art logic (TileGenerator + TileRenderer handle that)
/// - Biome blending logic (BiomeInfluence, BiomeMaskUtils)
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
    // Singleton Instance
    // ============================================================

    public static WorldMapController Instance { get; private set; }

    public PlayerMovementController movementController;

    // ============================================================
    // UI & References
    // ============================================================

    [Header("UI")]
    public TextMeshProUGUI LocationText; // InfoBar text for tile info

    [Header("Tile Rendering Settings")]
    public int viewRadius = 15;
    public GameObject tilePrefab;
    public Transform tileContainer;
    public float tileSize = 1f;

    [Header("Player Marker")]
    public PlayerMarkerController playerMarker;

    [Header("Minimap")]
    [SerializeField] private MinimapController minimap;

    // ============================================================
    // Internal State
    // ============================================================

    private Dictionary<Vector2Int, GameObject> visibleTiles;
    private WorldGenerator generator;
    private Vector2Int playerTilePos;
    private bool waitingForPlayer = false;

    // Track mouse position for detecting clicks vs drags
    private Vector3 mouseDownPosition;
    private const float clickThreshold = 5f; // pixels

    private static GameObject runtimeTilePrefabTemplate = null;

    // ============================================================
    // Public Accessors
    // ============================================================

    public WorldGenerator GetWorldGenerator() => generator;

    // ============================================================
    // Initialization
    // ============================================================

    private void Start()
    {
        Instance = this;

        var controllers = FindObjectsByType<WorldMapController>(FindObjectsSortMode.None);
        if (controllers.Length > 1 && controllers[0] != this)
        {
            Debug.LogWarning("[WorldMap] Duplicate controller found вЂ“ disabling this instance.");
            this.enabled = false;
            return;
        }

        if (!GameInitializer.IsInitialized())
        {
            UDADebug.Log("[WorldMap] Waiting for GameInitializer...");
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
    // Main Initialization
    // ============================================================

    private void ContinueInitialization()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[WorldMap] GameManager missing вЂ” returning to Preloader scene.");
            SceneLoader.LoadScene("Preloader");
            return;
        }

        if (GameData.CurrentPlayer == null)
        {
            if (!waitingForPlayer)
            {
                waitingForPlayer = true;
                UDADebug.Log("[WorldMap] Waiting for CurrentPlayer...");
                UIEvents.OnPlayerLoaded += OnPlayerLoaded;
            }
            return;
        }

        if (TempSaveCache.pendingSave != null)
        {
            UDADebug.Log("[WorldMap] Applying pending save...");
            GameManager.Instance.LoadGameData(TempSaveCache.pendingSave);
            TempSaveCache.pendingSave = null;
            UIEvents.InvokePlayerLoaded();
        }

        int seed = GameData.CurrentPlayer.worldSeed;
        if (seed < 10000)
        {
            Debug.LogError("[WorldMap] INVALID SEED (<10000). Forcing seed=10000.");
            seed = 10000;
        }

        Debug.Log("[WorldMap] DIRECTOR START: seed=" + seed);
        generator = new WorldGenerator(seed);

        // Initialize WorldLogicDirector
        var director = FindFirstObjectByType<WorldLogic.WorldLogicDirector>();
        if (director != null)
        {
            director.Initialize(seed, generator);
            Debug.Log("[WorldMap] WorldLogicDirector initialized.");
        }
        else
        {
            Debug.LogWarning("[WorldMap] WorldLogicDirector not found in scene!");
        }

        visibleTiles = new Dictionary<Vector2Int, GameObject>();

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

        TryAutoSaveOnEnter();

        UpdateMapAroundCamera();
        InitializePlayerMarker();

        RefreshMinimapTiles();
        minimap?.DrawPlayerMarker(playerTile);

        // Initialize InfoBar with player's starting tile
        TileData startTile = generator.GetTile(playerTile.x, playerTile.y);
        if (startTile != null)
            InfoBar_Update(startTile, playerTile);

        // Instantly teleport camera to player at game start (no animation)
        if (CameraMaster.Instance != null)
        {
            CameraMaster.Instance.TeleportToPlayer();
        }
    }

    private void OnPlayerLoaded()
    {
        UIEvents.OnPlayerLoaded -= OnPlayerLoaded;
        waitingForPlayer = false;
        ContinueInitialization();
    }


    // ============================================================
    // Per-frame updates
    // ============================================================

    private void Update()
    {
        if (generator == null) return;

        // Track mouse down position to distinguish clicks from drags
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPosition = Input.mousePosition;
        }

        // Handle tile click ONLY if CameraMaster is not dragging
        if (Input.GetMouseButtonUp(0) && !IsPointerOverUI() && !MinimapController.InputCaptured)
        {
            // Check if CameraMaster handled this as a drag
            if (CameraMaster.Instance != null)
            {
                // Access pan controller to check if it's dragging
                // For now, use simple distance check
                float dragDistance = Vector3.Distance(Input.mousePosition, mouseDownPosition);
                if (dragDistance < clickThreshold)
                {
                    HandleTileClick();
                }
            }
            else
            {
                // Fallback if no CameraMaster
                float dragDistance = Vector3.Distance(Input.mousePosition, mouseDownPosition);
                if (dragDistance < clickThreshold)
                {
                    HandleTileClick();
                }
            }
        }

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

    private void HandleTileClick()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // Tiles are centered at pos*tileSize; use RoundToInt to map correctly
        int tx = Mathf.RoundToInt(mouseWorld.x / tileSize);
        int ty = Mathf.RoundToInt(mouseWorld.y / tileSize);
        // Use instance generator (WorldGenerator has no static GetTile)
        if (generator == null)
        {
            Debug.LogWarning("[WorldMap] Generator not initialized yet.");
            return;
        }
        TileData tile = generator.GetTile(tx, ty);
        if (tile == null)
        {
            UDADebug.Log($"[Click] Tile not found at {tx}, {ty}");
            return;
        }
        Vector2Int coords = new Vector2Int(tx, ty);
        InfoBar_Update(tile, coords);
        if (movementController != null)
            movementController.PreparePathTo(coords);
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private void InfoBar_Update(TileData tile, Vector2Int coords)
    {
        if (LocationText == null)
        {
            Debug.LogError("LocationText is NULL вЂ” РЅРµ РїСЂРёРІСЏР·Р°РЅ РІ РёРЅСЃРїРµРєС‚РѕСЂРµ!");
            return;
        }

        // Use biomeId (canonical field in TileData)
        string biome = tile.biomeId;

        LocationText.text =
            $"Biome: {biome}\n" +
            $"Coords: ({coords.x}, {coords.y})\n" +
            $"Move Cost: {tile.moveCost:F1}";

        UDADebug.Log("INFOBAR UPDATED: " + LocationText.text);
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
    // Tile update logic
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

        List<Vector2Int> toRemove = new();
        foreach (var kv in visibleTiles)
        {
            if (!desiredTiles.Contains(kv.Key))
                toRemove.Add(kv.Key);
        }

        foreach (Vector2Int pos in toRemove)
        {
            // === OverlayRenderer integration ===
            UniqueLocationOverlayRenderer.Instance?.OnTileDespawned(
                new WorldTilePos(pos.x, pos.y)
            );

            Destroy(visibleTiles[pos]);
            visibleTiles.Remove(pos);
        }
    }


    // ============================================================
    // Tile spawning
    // ============================================================

    private void SpawnTile(Vector2Int pos)
    {
        TileData data = generator.GetTile(pos.x, pos.y);

        if (tilePrefab == null)
        {
            tilePrefab = Resources.Load<GameObject>("WorldData/TilePrefab") ??
                         Resources.Load<GameObject>("TilePrefab");

            if (tilePrefab == null)
            {
                if (runtimeTilePrefabTemplate == null)
                {
                    runtimeTilePrefabTemplate = new GameObject("TilePrefabRuntime");
                    runtimeTilePrefabTemplate.AddComponent<TileRenderer>();
                    runtimeTilePrefabTemplate.SetActive(false);
                }
                tilePrefab = runtimeTilePrefabTemplate;
            }
        }

        Transform parent = tileContainer != null ? tileContainer : this.transform;
        GameObject obj = Instantiate(tilePrefab, parent);

        obj.transform.localPosition = new Vector3(pos.x * tileSize, pos.y * tileSize, 0);
        obj.transform.localScale = new Vector3(tileSize, tileSize, 1f);
        obj.SetActive(true);

        TileRenderer tr = obj.GetComponent<TileRenderer>();
        if (tr == null)
        {
            Debug.LogError("[WorldMap] TilePrefab missing TileRenderer!");
        }
        else
        {
            tr.RenderTile(data);
        }

        // === OverlayRenderer integration ===
        UniqueLocationOverlayRenderer.Instance?.OnTileSpawned(
            new WorldTilePos(pos.x, pos.y),
            obj
        );

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
        if (LocationText == null || generator == null)
            return;

        var p = GameData.CurrentPlayer;
        if (p == null) return;

        Vector2Int coords = new(
            Mathf.RoundToInt(p.mapPosX),
            Mathf.RoundToInt(p.mapPosY)
        );

        TileData tile = generator.GetTile(coords.x, coords.y);

        LanguageManager.LoadLanguage("biomes_" + LanguageManager.CurrentLanguage);
        string biomeName = LanguageManager.Get(tile.biomeId);

        string transition = "";
        if (tile.edgeMask != 0 && !string.IsNullOrEmpty(tile.edgeBiome))
        {
            string edgeName = LanguageManager.Get(tile.edgeBiome);
            transition = $" в†’ {edgeName}";
        }

        LocationText.text = $"{biomeName}{transition}\n(X: {coords.x}, Y: {coords.y})";
        BiomeFXController.Instance?.OnBiomeChanged(tile.biomeId);
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

    private void HandleMinimapDrag(Vector2 worldCenter)
    {
        if (Camera.main == null)
            return;

        Vector3 target = new Vector3(
            worldCenter.x * tileSize,
            worldCenter.y * tileSize,
            Camera.main.transform.position.z
        );

        Camera.main.transform.position = Vector3.Lerp(
            Camera.main.transform.position,
            target,
            0.15f
        );

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

