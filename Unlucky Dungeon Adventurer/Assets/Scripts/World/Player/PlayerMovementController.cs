using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Глобальные настройки")]
    public float tileSize = 10f;
    public int minutesPerTile = 30;
    public int restMinutesMin = 240; // 4 часа базово

    [Header("Ссылка на GameManager")]
    public GameManager gameManager;

    private SaveData _save => gameManager.GetCurrentGameData();
    private PlayerSaveData Player => _save.player;
    private WorldSaveData World => _save.world;

    private List<Vector2Int> _currentPath;
    private int _currentPathStaminaCost;
    private bool _isMoving;

    private void Start()
    {
        // Поставим маркер игрока в позицию из сейва
        Vector3 startPos = new Vector3(Player.mapPosX * tileSize, Player.mapPosY * tileSize, 0f);
        transform.position = startPos;
    }

    /// <summary>
    /// Вызывается миром при клике по тайлу.
    /// </summary>
    public void PreparePathTo(Vector2Int targetTile)
    {
        if (_isMoving) return;

        Vector2Int startTile = new Vector2Int(
            Mathf.RoundToInt(Player.mapPosX),
            Mathf.RoundToInt(Player.mapPosY)
        );

        var path = Pathfinding.FindPath(startTile, targetTile);
        if (path == null || path.Count <= 1)
        {
            Debug.Log("[Move] Нет пути до цели");
            PathRenderer.ClearAll();
            _currentPath = null;
            UIEvents.OnPathPreview?.Invoke(0, 0, false);
            return;
        }

        _currentPath = path;

        // Посчитаем стоимость и время
        float totalCost = 0f;
        for (int i = 1; i < path.Count; i++)
        {
            var p = path[i];
            float cost = TileGenerator.GetTileMoveCost(p.x, p.y);
            totalCost += cost;
        }

        _currentPathStaminaCost = Mathf.CeilToInt(totalCost);

        int totalMinutes = minutesPerTile * (path.Count - 1);
        bool hasEnough = Player.currentStamina >= _currentPathStaminaCost;

        Debug.Log($"[Move] Путь найден. Тайлов: {path.Count - 1}, Стамина: {_currentPathStaminaCost}, Минут: {totalMinutes}, Хватает: {hasEnough}");

        // Показать шарики
        PathRenderer.Show(path);

        // Уведомляем UI
        UIEvents.OnPathPreview?.Invoke(_currentPathStaminaCost, totalMinutes, hasEnough);
        UIEvents.OnRestAvailable?.Invoke(Player.currentStamina < Player.baseMaxStamina);
    }

	private int CalculateStaminaCost(List<Vector2Int> path)
	{
		float cost = 0f;

		for (int i = 1; i < path.Count; i++)
		{
			Vector2Int p = path[i];
			cost += TileGenerator.GetTileMoveCost(p.x, p.y);
		}

		return Mathf.CeilToInt(cost);
	}

	private int CalculateTimeCost(List<Vector2Int> path)
	{
		return minutesPerTile * (path.Count - 1);
	}

    /// <summary>
    /// Нажата кнопка "Ходьба".
    /// </summary>
    public void StartWalk()
    {
        if (_currentPath == null || _currentPath.Count <= 1)
        {
            Debug.Log("[Move] Нет подготовленного пути");
            return;
        }

        if (Player.currentStamina < _currentPathStaminaCost)
        {
            Debug.Log("[Move] Недостаточно стамины для пути");
            return;
        }

        if (_isMoving) return;

        StartCoroutine(WalkRoutine());
    }

    private IEnumerator WalkRoutine()
    {
        _isMoving = true;
        UIEvents.OnMovementStarted?.Invoke();

        // идём по всем точкам пути, кроме [0] (стартовый тайл)
        for (int i = 1; i < _currentPath.Count; i++)
        {
            Vector2Int tile = _currentPath[i];

            // стоимость тайла
            float cost = TileGenerator.GetTileMoveCost(tile.x, tile.y);
            int staminaCost = Mathf.CeilToInt(cost);

            if (Player.currentStamina < staminaCost)
            {
                Debug.Log("[Move] Стамина закончилась во время пути");
                break;
            }

            // списываем стамину
            PlayerStatsController.Instance.ModifyStamina(-staminaCost);

            // добавляем время
            World.AddMinutes(minutesPerTile);

            // шанс события (пока просто лог)
            HandleRandomEvent(tile);

            // двигаем маркер
            Vector3 targetPos = new Vector3(tile.x * tileSize, tile.y * tileSize, 0f);
            yield return MoveToPosition(targetPos);

            // обновляем позицию игрока в сейве
            Player.mapPosX = tile.x;
            Player.mapPosY = tile.y;

            // удаляем первый шарик
            PathRenderer.ConsumeFirst();
        }

        // очистка пути
        PathRenderer.ClearAll();
        _currentPath = null;
        _currentPathStaminaCost = 0;
        _isMoving = false;

        UIEvents.OnMovementEnded?.Invoke();
        UIEvents.OnRestAvailable?.Invoke(Player.currentStamina < Player.baseMaxStamina);
        UIEvents.OnPlayerStatsChanged?.Invoke();
    }

    private IEnumerator MoveToPosition(Vector3 targetPos)
    {
        const float speed = 10f; // можно потом вынести в поле

        while ((transform.position - targetPos).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPos;
    }

    private void HandleRandomEvent(Vector2Int tile)
    {
        // Пока только для отладки
        Debug.Log($"[Event] Проверка события на тайле {tile.x}, {tile.y}");
    }

    // Кнопка "Отдых" (простой отдых)
    public void StartRest()
    {
        // простой отдых: фиксированные 4 часа и фулл стамина
        World.AddMinutes(restMinutesMin);
        int missing = Player.baseMaxStamina - Player.currentStamina;
		PlayerStatsController.Instance.ModifyStamina(missing);

        Debug.Log($"[Rest] Отдых: +{restMinutesMin} минут, стамина восстановлена");

        UIEvents.OnPlayerStatsChanged?.Invoke();
        UIEvents.OnRestAvailable?.Invoke(false);
    }
}
