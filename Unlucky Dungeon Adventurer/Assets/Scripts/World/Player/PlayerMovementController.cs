using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float tileSize = 10f;
    public int minutesPerTile = 30;

    public GameManager gameManager;

    private PlayerData Player => GameData.CurrentPlayer;

    private List<Vector2Int> _currentPath;
    private Vector2Int _lastTargetTile;
    private int _staminaCost;
    private bool _isMoving;

    private void Start()
    {
        if (Player == null)
        {
            Debug.LogError("[PlayerMovement] GameData.CurrentPlayer is null!");
            return;
        }

        Vector3 startPos = new Vector3(Player.mapPosX * tileSize, Player.mapPosY * tileSize, 0f);
        transform.position = startPos;
    }

    public void PreparePathTo(Vector2Int targetTile)
    {
        if (_isMoving) return;

        // Cancel if clicking same target tile
        if (_currentPath != null && _currentPath.Count > 0 && _lastTargetTile == targetTile)
        {
            CancelPath();
            return;
        }

        Vector2Int start = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / tileSize),
            Mathf.RoundToInt(transform.position.y / tileSize)
        );

        var path = Pathfinding.FindPath(start, targetTile);
        if (path == null || path.Count <= 1)
        {
            CancelPath();
            return;
        }

        _currentPath = path;
        _lastTargetTile = targetTile;
        _staminaCost = PathCostCalculator.GetStaminaCost(path);
        int timeCost = PathCostCalculator.GetTimeCost(path, minutesPerTile);

        bool enough = Player != null && Player.currentStamina >= _staminaCost;

        if (!enough)
        {
            // Показываем маршрут, но не даём идти и выводим сообщение о недостатке стамины
            PathRenderer.Show(path);
            UIEvents.OnPathPreview?.Invoke(_staminaCost, timeCost, false);
            // Специальное событие для UI — недостаточно стамины
            UIEvents.OnNotEnoughStamina?.Invoke();
            if (Player != null)
                UIEvents.OnRestAvailable?.Invoke(Player.currentStamina < Player.finalMaxStamina);
            return;
        }

        PathRenderer.Show(path);

        UIEvents.OnPathPreview?.Invoke(_staminaCost, timeCost, true);
        if (Player != null)
            UIEvents.OnRestAvailable?.Invoke(Player.currentStamina < Player.finalMaxStamina);
    }

    public void CancelPath()
    {
        PathRenderer.ClearAll();
        _currentPath = null;
        _lastTargetTile = new Vector2Int(-9999, -9999);
        _staminaCost = 0;
        UIEvents.OnPathPreview?.Invoke(0, 0, false);
    }

    public void StartWalk()
    {
        if (Player == null)
        {
            Debug.LogError("[PlayerMovement] Cannot start walk - Player is null!");
            return;
        }

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
            if (Player == null)
            {
                Debug.LogError("[PlayerMovement] Player became null during movement!");
                break;
            }

            Vector2Int tile = _currentPath[i];

            float moveCost = TileGenerator.GetTileMoveCost(tile.x, tile.y);
            int staminaCost = Mathf.CeilToInt(moveCost);

            if (Player.currentStamina < staminaCost)
                break;

            if (PlayerStatsController.Instance != null)
            {
                PlayerStatsController.Instance.ModifyStamina(-staminaCost);
            }
            else
            {
                Debug.LogWarning("[Movement] PlayerStatsController.Instance is null! Using direct modification.");
                Player.currentStamina -= staminaCost;
                if (Player.currentStamina < 0) Player.currentStamina = 0;
            }

            if (PlayerStatsController.Instance != null)
            {
                PlayerStatsController.Instance.UpdateMapPosition(tile.x, tile.y);
            }
            else
            {
                Player.mapPosX = tile.x;
                Player.mapPosY = tile.y;
            }

            if (GameManager.Instance != null)
            {
                var worldData = GameManager.Instance.GetCurrentGameData()?.world;
                if (worldData != null)
                {
                    MovementTimeController.ApplyTime(worldData, minutesPerTile);
                }
            }

            MovementEventResolver.ProcessTileEvent(tile);

            Vector3 target = new Vector3(tile.x * tileSize, tile.y * tileSize, 0f);
            yield return MoveTo(target);

            PathRenderer.ConsumeFirst();
        }

        PathRenderer.ClearAll();
        _currentPath = null;
        _lastTargetTile = new Vector2Int(-9999, -9999);
        _staminaCost = 0;
        _isMoving = false;

        UIEvents.OnMovementEnded?.Invoke();
        UIEvents.InvokePlayerStatsChanged();
        if (Player != null)
            UIEvents.OnRestAvailable?.Invoke(Player.currentStamina < Player.finalMaxStamina);
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        float speed = 10f;
        while ((transform.position - target).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );
            yield return null;
        }
        transform.position = target;
    }
}
