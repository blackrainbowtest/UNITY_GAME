using System.Collections.Generic;
using UnityEngine;

public class WorldMapController : MonoBehaviour
{
    // how many tiles are visible around the player
    public int viewRadius = 15;
    public GameObject tilePrefab;
    public Transform tileContainer;

    private Dictionary<Vector2Int, GameObject> visibleTiles;
    private WorldGenerator generator;
    private Vector2Int playerTilePos;

    private void Start()
    {
        int seed = GameData.CurrentPlayer != null
            ? GameData.CurrentPlayer.worldSeed
            : Random.Range(0, 99999999);
        generator = new WorldGenerator(seed);
        visibleTiles = new Dictionary<Vector2Int, GameObject>();


        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[WorldMap] GameManager отсутствует — загружаю Preloader");
            SceneLoader.LoadScene("Preloader");
            return;
        }

        if (GameData.CurrentPlayer == null)
        {
            Debug.LogWarning("[WorldMap] CurrentPlayer отсутствует — загружаю сохранение");
            if (TempSaveCache.pendingSave != null)
            {
                GameManager.Instance.LoadGameData(TempSaveCache.pendingSave);
                TempSaveCache.pendingSave = null;
            }
        }

        TryAutoSaveOnEnter();
        UpdateMapAroundPlayer();
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
        // Обновление только если игрок реально переместился
        var newPos = new Vector2Int(
            Mathf.FloorToInt(GameData.CurrentPlayer.mapPosX),
            Mathf.FloorToInt(GameData.CurrentPlayer.mapPosY)
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

        // Remove those tiles that are no longer needed
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

        GameObject obj = Instantiate(tilePrefab, tileContainer);
        obj.transform.localPosition = new Vector3(pos.x, pos.y, 0);

        obj.GetComponent<TileRenderer>().RenderTile(data);

        visibleTiles[pos] = obj;
    }
}
