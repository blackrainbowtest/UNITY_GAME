using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float tileSize = 10f;
    public int minutesPerTile = 30;

    public GameManager gameManager;

    private SaveData Save => gameManager.GetCurrentGameData();
    private PlayerSaveData Player => Save.player;
    private WorldSaveData World => Save.world;

    private List<Vector2Int> _currentPath;
    private int _staminaCost;
    private bool _isMoving;

    private void Start()
    {
        Vector3 startPos = new Vector3(Player.mapPosX * tileSize, Player.mapPosY * tileSize, 0f);
        transform.position = startPos;
    }

    public void PreparePathTo(Vector2Int targetTile)
    {
        if (_isMoving) return;

        Vector2Int start = new Vector2Int(Mathf.RoundToInt(Player.mapPosX),
                                          Mathf.RoundToInt(Player.mapPosY));

        var path = Pathfinding.FindPath(start, targetTile);
        if (path == null || path.Count <= 1)
        {
            PathRenderer.ClearAll();
            UIEvents.OnPathPreview?.Invoke(0, 0, false);
            return;
        }

        _currentPath = path;
        _staminaCost = PathCostCalculator.GetStaminaCost(path);
        int timeCost = PathCostCalculator.GetTimeCost(path, minutesPerTile);

        bool enough = Player.currentStamina >= _staminaCost;

        PathRenderer.Show(path);

        UIEvents.OnPathPreview?.Invoke(_staminaCost, timeCost, enough);
        UIEvents.OnRestAvailable?.Invoke(Player.currentStamina < Player.baseMaxStamina);
    }

    public void StartWalk()
    {
        if (_currentPath == null || _currentPath.Count <= 1) return;
        if (Player.currentStamina < _staminaCost) return;
        if (_isMoving) return;

        StartCoroutine(WalkRoutine());
    }

    private IEnumerator WalkRoutine()
    {
        _isMoving = true;
        UIEvents.OnMovementStarted?.Invoke();

        for (int i = 1; i < _currentPath.Count; i++)
        {
            Vector2Int tile = _currentPath[i];

            float moveCost = TileGenerator.GetTileMoveCost(tile.x, tile.y);
            int staminaCost = Mathf.CeilToInt(moveCost);

            if (Player.currentStamina < staminaCost)
                break;

            PlayerStatsController.Instance.ModifyStamina(-staminaCost);
            MovementTimeController.ApplyTime(World, minutesPerTile);
            MovementEventResolver.ProcessTileEvent(tile);

            Vector3 target = new Vector3(tile.x * tileSize, tile.y * tileSize, 0f);
            yield return MoveTo(target);

            Player.mapPosX = tile.x;
            Player.mapPosY = tile.y;

            PathRenderer.ConsumeFirst();
        }

        PathRenderer.ClearAll();
        _currentPath = null;
        _staminaCost = 0;
        _isMoving = false;

        UIEvents.OnMovementEnded?.Invoke();
        UIEvents.InvokePlayerStatsChanged();
        UIEvents.OnRestAvailable?.Invoke(Player.currentStamina < Player.baseMaxStamina);
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        float speed = 10f;
        while ((transform.position - target).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, target,
                speed * Time.deltaTime
            );
            yield return null;
        }
        transform.position = target;
    }
}
