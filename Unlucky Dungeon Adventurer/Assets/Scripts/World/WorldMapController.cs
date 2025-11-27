using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldMapController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI locationText;

    public int viewRadius = 15;
    public GameObject tilePrefab;
    public Transform tileContainer;
    public PlayerMarkerController playerMarker;
    public float tileSize = 1f;

    [Header("Minimap")]
    [SerializeField] private MinimapController minimap;

    private Dictionary<Vector2Int, GameObject> visibleTiles;
    private WorldGenerator generator;
    private Vector2Int playerTilePos;
    private static GameObject runtimeTilePrefabTemplate = null;

    private bool waitingForPlayer = false;

    private void Start()
    {
        // Detect duplicate controllers
        var controllers = FindObjectsByType<WorldMapController>(FindObjectsSortMode.None);
        if (controllers.Length > 1)
        {
            if (controllers[0] != this)
            {
                this.enabled = false;
                Debug.LogWarning($"[WorldMap] Duplicate controller '{gameObject.name}' disabled.");
                return;
            }
        }

        // Wait for initializer
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

    private void ContinueInitialization()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[WorldMap] GameManager missing. Reloading preload scene.");
            SceneLoader.LoadScene("Preloader");
            return;
        }

        if (GameData.CurrentPlayer == null)
        {
            if (!waitingForPlayer)
            {
                waitingForPlayer = true;
                Debug.Log("[WorldMap] Waiting for CurrentPlayer to finish loading...");
                UIEvents.OnPlayerLoaded += OnPlayerLoaded;
            }
            return;
        }

        if (TempSaveCache.pendingSave != null)
        {
            Debug.Log("[WorldMap] Found TempSaveCache.pendingSave — applying save before world init");
            GameManager.Instance.LoadGameData(TempSaveCache.pendingSave);
            TempSaveCache.pendingSave = null;
            UIEvents.InvokePlayerLoaded();
        }

        int seed = GameData.CurrentPlayer.worldSeed;
        if (seed < 10000)
        {
            Debug.LogError("[WorldMap] INVALID SEED IN SAVE! Seed must be >10000. Fix character creation.");
            seed = 10000;
        }

        Debug.Log($"[WorldMap] Using worldSeed = {seed}");

        // 1) Создаём генератор мира
        generator = new WorldGenerator(seed);

        // 2) Подготавливаем словарь отрендеренных тайлов
        visibleTiles = new Dictionary<Vector2Int, GameObject>();

        // ------------------------------------
        // >>> 3) ТУТ ДОБАВЛЯЕМ МИНИКАРТУ <<<
        // ------------------------------------
        Vector2Int playerTile = new Vector2Int(
            Mathf.RoundToInt(GameData.CurrentPlayer.mapPosX),
            Mathf.RoundToInt(GameData.CurrentPlayer.mapPosY)
        );

        if (minimap != null)
        {
            minimap.Initialize(viewRadius, playerTile);
            minimap.OnMinimapTileClicked += HandleMinimapClick;

            Debug.Log("[WorldMap] Minimap initialized.");
        }
        else
        {
            Debug.LogWarning("[WorldMap] Minimap reference is missing!");
        }
        // ------------------------------------


        TryAutoSaveOnEnter();

        // 4) Генерация тайлов вокруг камеры
        UpdateMapAroundCamera();

        try
        {
            int hash = ComputeRegionHash(playerTilePos.x, playerTilePos.y, 6);
            Debug.Log($"[WorldMap] Region hash (seed={seed}): {hash}");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[WorldMap] Failed region hash: " + ex.Message);
        }

        // 5) Создание маркера игрока
        InitializePlayerMarker();
        
        // 6) Инициализация миникарты с начальными тайлами и маркером игрока
        RefreshMinimapTiles();
        minimap?.DrawPlayerMarker(playerTile);
    }


    private void OnPlayerLoaded()
    {
        UIEvents.OnPlayerLoaded -= OnPlayerLoaded;
        waitingForPlayer = false;
        ContinueInitialization();
    }

    private void TryAutoSaveOnEnter()
    {
        if (GameData.CurrentPlayer != null)
        {
            if (GameData.CurrentPlayer.worldSeed >= 10000)
            {
                SaveManager.SaveAuto(GameManager.Instance.GetCurrentGameData());
            }
            else
            {
                Debug.LogWarning("[WorldMap] Autosave skipped due to invalid worldSeed.");
            }
        }
    }

    private void Update()
    {
        if (generator == null) return;

        Vector3 camPos = Camera.main.transform.position;

        var newPos = new Vector2Int(
            Mathf.FloorToInt(camPos.x / tileSize),
            Mathf.FloorToInt(camPos.y / tileSize)
        );

        if (newPos != playerTilePos)
        {
            playerTilePos = newPos;
            UpdateMapAroundCamera();
            
            // Update minimap center
            minimap?.SetCenter(playerTilePos);
            RefreshMinimapTiles();
        }

        // Update minimap display every frame
        UpdateMinimapDisplay();
    }

    /// <summary>
    /// Updates minimap display (tiles + camera frame + player marker)
    /// </summary>
    private void UpdateMinimapDisplay()
    {
        if (minimap == null) return;

        // Clear texture first, then redraw tiles and frame
        // to prevent old frame from persisting
        minimap.Clear();

        // Update tiles in current minimap window
        RefreshMinimapTiles();

            // Calculate visible camera bounds in tiles
            Vector2Int cameraViewBounds = GetCameraViewBoundsInTiles();
            Vector2Int camMinTile = playerTilePos - cameraViewBounds;
            Vector2Int camMaxTile = playerTilePos + cameraViewBounds;
        
            // Draw camera frame on minimap
            minimap.DrawCameraFrame(camMinTile, camMaxTile);

            // Draw player marker at actual player position
            if (GameData.CurrentPlayer != null)
            {
                Vector2Int actualPlayerPos = new Vector2Int(
                    Mathf.RoundToInt(GameData.CurrentPlayer.mapPosX),
                    Mathf.RoundToInt(GameData.CurrentPlayer.mapPosY)
                );
                minimap.DrawPlayerMarker(actualPlayerPos);
            }
    }

    private void UpdateMapAroundCamera()
    {
        HashSet<Vector2Int> desiredTiles = new HashSet<Vector2Int>();

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

        if (tilePrefab == null)
        {
            tilePrefab = Resources.Load<GameObject>("WorldData/TilePrefab") ??
                         Resources.Load<GameObject>("TilePrefab");

            if (tilePrefab == null)
            {
                if (runtimeTilePrefabTemplate == null)
                {
                    Debug.LogWarning("[WorldMap] No tilePrefab in inspector or resources. Creating runtime template.");
                    runtimeTilePrefabTemplate = new GameObject("TilePrefabRuntime");
                    var sr = runtimeTilePrefabTemplate.AddComponent<SpriteRenderer>();
                    var trComp = runtimeTilePrefabTemplate.AddComponent<TileRenderer>();
                    trComp.sr = sr;

                    TileSpriteDB db = Resources.Load<TileSpriteDB>("WorldData/TileSpriteDB") ??
                                      Resources.Load<TileSpriteDB>("TileSpriteDB");

                    if (db == null)
                        db = ScriptableObject.CreateInstance<TileSpriteDB>();

                    trComp.spriteDB = db;

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

        var tr = obj.GetComponent<TileRenderer>();
        if (tr != null)
        {
            tr.RenderTile(data);
        }

        if (minimap != null)
        {
            // Use tile's biome color from TileData, not SpriteRenderer color
            // (SpriteRenderer.color is white when sprite is used)
            minimap.UpdateTile(pos, data.color);
        }

        visibleTiles[pos] = obj;
    }

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
                    h = h * 31 + (t.biomeId?.GetHashCode() ?? 0);
                    h = h * 31 + (t.subBiomeId?.GetHashCode() ?? 0);
                    h = h * 31 + (t.spriteId?.GetHashCode() ?? 0);
                    h = h * 31 + Mathf.RoundToInt(t.moveCost * 100f);
                }
            }
            return h;
        }
    }

    private void InitializePlayerMarker()
    {
        if (playerMarker == null)
        {
            Debug.LogWarning("[WorldMap] PlayerMarker reference is not set!");
            return;
        }

        var p = GameData.CurrentPlayer;

        Vector2Int coords = new Vector2Int(
            Mathf.RoundToInt(p.mapPosX),
            Mathf.RoundToInt(p.mapPosY)
        );

        playerMarker.SetPosition(coords, tileSize);

        Debug.Log($"[WorldMap] Player marker placed at {coords}");
        UpdatePlayerLocationInfo();
    }

    private void UpdatePlayerLocationInfo()
    {
        if (locationText == null || generator == null)
            return;

        var p = GameData.CurrentPlayer;
        if (p == null)
            return;

        Vector2Int coords = new Vector2Int(
            Mathf.RoundToInt(p.mapPosX),
            Mathf.RoundToInt(p.mapPosY)
        );

        TileData tile = generator.GetTile(coords.x, coords.y);

        // Загружаем локализацию биомов, если ещё не загружена
        LanguageManager.LoadLanguage("biomes_" + LanguageManager.CurrentLanguage);
        string biome = tile.biomeId != null ? LanguageManager.Get(tile.biomeId) : LanguageManager.Get("Unknown");
        string subBiome = tile.subBiomeId != null ? LanguageManager.Get(tile.subBiomeId) : "";
        string spriteId = tile.spriteId ?? "";

        locationText.text = 
            $"{biome}: {subBiome}\n(X: {coords.x}, Y: {coords.y})";
    }

        /// <summary>
        /// Вычисляет размер видимой области камеры в тайлах
        /// </summary>
        private Vector2Int GetCameraViewBoundsInTiles()
        {
            if (Camera.main == null) return Vector2Int.zero;

            // Получаем размер камеры в world units
            float camHeight = Camera.main.orthographicSize * 2f;
            float camWidth = camHeight * Camera.main.aspect;

            // Конвертируем в тайлы (округляем вверх чтобы захватить все видимые тайлы)
            int tilesWidth = Mathf.CeilToInt(camWidth / tileSize / 2f);
            int tilesHeight = Mathf.CeilToInt(camHeight / tileSize / 2f);

                // Debug log (раскомментируй для отладки)
                // Debug.Log($"[WorldMap] Camera bounds: {tilesWidth}x{tilesHeight} tiles (orthoSize={Camera.main.orthographicSize}, aspect={Camera.main.aspect})");

            return new Vector2Int(tilesWidth, tilesHeight);
        }

    private void HandleMinimapClick(Vector2Int tilePos)
    {
        Debug.Log($"[WorldMap] Minimap clicked at tile {tilePos}");
        MoveCameraTo(tilePos);
    }

    private void MoveCameraTo(Vector2Int worldTile)
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("[WorldMap] Main camera not found!");
            return;
        }

        // Convert tile position to world position
        Vector3 targetPos = new Vector3(
            worldTile.x * tileSize,
            worldTile.y * tileSize,
            Camera.main.transform.position.z
        );

        // Smooth camera movement with Lerp for drag
        Camera.main.transform.position = Vector3.Lerp(
            Camera.main.transform.position,
            targetPos,
            0.3f // Smooth factor - adjust for more/less smoothness
        );
        
        // Update player tile position based on camera
        Vector2Int newTilePos = new Vector2Int(
            Mathf.FloorToInt(Camera.main.transform.position.x / tileSize),
            Mathf.FloorToInt(Camera.main.transform.position.y / tileSize)
        );
        
        if (newTilePos != playerTilePos)
        {
            playerTilePos = newTilePos;
            UpdateMapAroundCamera();
            
            // Update minimap
            minimap?.SetCenter(playerTilePos);
            RefreshMinimapTiles();
        }
        
        // Draw player marker at actual player position
        if (GameData.CurrentPlayer != null)
        {
            Vector2Int actualPlayerPos = new Vector2Int(
                Mathf.RoundToInt(GameData.CurrentPlayer.mapPosX),
                Mathf.RoundToInt(GameData.CurrentPlayer.mapPosY)
            );
            minimap?.DrawPlayerMarker(actualPlayerPos);
        }
    }

    private void RefreshMinimapTiles()
    {
        if (minimap == null || generator == null) return;

        // Update all tiles in minimap view radius
        for (int dx = -viewRadius; dx <= viewRadius; dx++)
        {
            for (int dy = -viewRadius; dy <= viewRadius; dy++)
            {
                Vector2Int pos = new Vector2Int(playerTilePos.x + dx, playerTilePos.y + dy);
                TileData tileData = generator.GetTile(pos.x, pos.y);
                
                // Always use biome color from TileData for minimap
                // (not SpriteRenderer color, which is white when sprite is used)
                minimap.UpdateTile(pos, tileData.color);
            }
        }
    }
}
