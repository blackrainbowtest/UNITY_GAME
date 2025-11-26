using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MinimapController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("References")]
    [SerializeField] private RawImage minimapImage;

    [Header("Settings")]
    [SerializeField] private Color emptyColor = new Color(0f, 0f, 0f, 0f);
    [SerializeField] private Color cameraFrameColor = Color.yellow;
    [SerializeField] private Color playerMarkerColor = Color.red;

    // Кол-во тайлов по одной стороне (31 при viewRadius = 15)
    private int tilesPerSide;

    // Текстура миникарты
    private Texture2D minimapTexture;

    // В каком участке мира сейчас "окно" миникарты
    private int originX;
    private int originY;

    // Флаг "есть изменения, нужно Apply"
    private bool dirty;

    // Обработчик ввода
    private MinimapInputHandler inputHandler;
    
    // Для отслеживания drag движения
    private Vector2Int lastDragTile;
    private Vector2Int dragStartCenterTile;

    // Для будущей навигации: мир-тайл, по которому кликнули
    public event Action<Vector2Int> OnMinimapTileClicked;

    // Глобальная блокировка ввода мира, пока удерживается указатель над миникартой
    public static bool InputCaptured { get; private set; }

    /// <summary>
    /// Инициализация миникарты.
    /// Вызываем из WorldMapController после того,
    /// как знаем viewRadius и стартовый тайл игрока.
    /// </summary>
    public void Initialize(int viewRadius, Vector2Int playerTile)
    {
        tilesPerSide = viewRadius * 2 + 1;
        inputHandler = new MinimapInputHandler();

        // Создаём текстуру 1 тайл = 1 пиксель
        minimapTexture = new Texture2D(
            tilesPerSide,
            tilesPerSide,
            TextureFormat.RGBA32,
            false
        );
        minimapTexture.filterMode = FilterMode.Point;
        minimapTexture.wrapMode = TextureWrapMode.Clamp;

        MinimapRenderer.ClearTexture(minimapTexture, emptyColor);

        if (minimapImage == null)
        {
            // Если забыли проставить через инспектор — пробуем взять с того же объекта
            minimapImage = GetComponent<RawImage>();
        }

        if (minimapImage != null)
        {
            minimapImage.texture = minimapTexture;
        }
        else
        {
            Debug.LogError("[Minimap] RawImage reference is missing!");
        }

        SetCenter(playerTile);
    }

    /// <summary>
    /// Устанавливаем центр миникарты по мировому тайлу (координата игрока).
    /// </summary>
    public void SetCenter(Vector2Int centerTile)
    {
        // Окно миникарты = квадрат tilesPerSide × tilesPerSide
        originX = centerTile.x - tilesPerSide / 2;
        originY = centerTile.y - tilesPerSide / 2;
    }

    /// <summary>
    /// Обновляем цвет конкретного тайла на миникарте.
    /// worldTile — координата в мире (та же, что у тайл-генератора).
    /// </summary>
    public void UpdateTile(Vector2Int worldTile, Color color)
    {
        if (MinimapRenderer.UpdateTile(minimapTexture, worldTile, originX, originY, tilesPerSide, color))
        {
            dirty = true;
        }
    }

    /// <summary>
    /// Рисуем рамку камеры по тайлам мира.
    /// Сейчас просто обводка. Вызовешь позже, когда захочешь.
    /// </summary>
    public void DrawCameraFrame(Vector2Int camMinTile, Vector2Int camMaxTile)
    {
        MinimapRenderer.DrawCameraFrame(
            minimapTexture,
            camMinTile,
            camMaxTile,
            originX,
            originY,
            tilesPerSide,
            cameraFrameColor
        );
        dirty = true;
    }

    public void DrawPlayerMarker(Vector2Int playerTile)
    {
        if (MinimapRenderer.DrawPlayerMarker(
            minimapTexture,
            playerTile,
            originX,
            originY,
            tilesPerSide,
            playerMarkerColor))
        {
            dirty = true;
        }
    }

    /// <summary>
    /// Полностью очищает текстуру миникарты указанным цветом.
    /// </summary>
    public void Clear()
    {
        MinimapRenderer.ClearTexture(minimapTexture, emptyColor);
        dirty = true;
    }

    /// <summary>
    /// Применяем накопленные изменения к текстуре раз в кадр.
    /// </summary>
    private void LateUpdate()
    {
        if (dirty && minimapTexture != null)
        {
            minimapTexture.Apply();
            dirty = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        inputHandler?.OnPointerDown(eventData);
        InputCaptured = true; // блокируем ввод мира на всё время удержания
        
        // Запоминаем начальную позицию для drag
        if (MinimapCoordinateConverter.TryGetWorldTile(
            eventData,
            minimapImage,
            tilesPerSide,
            originX,
            originY,
            out Vector2Int startTile))
        {
            lastDragTile = startTile;
            dragStartCenterTile = new Vector2Int(
                originX + tilesPerSide / 2,
                originY + tilesPerSide / 2
            );
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Если не было перетаскивания - обрабатываем как клик
        if (inputHandler != null && !inputHandler.IsDragging)
        {
            ProcessMinimapInteraction(eventData);
        }
        inputHandler?.Reset();
        InputCaptured = false; // снимаем блокировку
    }

    private void ProcessMinimapInteraction(PointerEventData eventData)
    {
        if (MinimapCoordinateConverter.TryGetWorldTile(
            eventData,
            minimapImage,
            tilesPerSide,
            originX,
            originY,
            out Vector2Int worldTile))
        {
            OnMinimapTileClicked?.Invoke(worldTile);
        }
    }

    /// <summary>
    /// Обработка перетаскивания по миникарте.
    /// Постоянно вызывается при движении курсора/пальца во время drag.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (inputHandler == null) return;

        // Проверяем, достаточно ли сдвинулись для активации drag
        if (!inputHandler.CheckDragStart(eventData))
        {
            return; // Ещё не начали drag
        }

        // Получаем текущую позицию на миникарте
        if (MinimapCoordinateConverter.TryGetWorldTile(
            eventData,
            minimapImage,
            tilesPerSide,
            originX,
            originY,
            out Vector2Int currentTile))
        {
            // Вычисляем смещение от начальной позиции drag
            Vector2Int delta = lastDragTile - currentTile;
            
            // Инвертируем движение: если тянем вверх на миникарте (currentTile.y больше),
            // камера должна двигаться вниз (в сторону меньших Y)
            Vector2Int targetTile = dragStartCenterTile + delta;
            
            OnMinimapTileClicked?.Invoke(targetTile);
        }
        // Если вышли за пределы миникарты — просто игнорируем
    }
}
