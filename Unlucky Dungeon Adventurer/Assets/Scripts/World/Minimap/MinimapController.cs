/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/World/Minimap/MinimapController.cs                  */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 09:48:06 by UDA                                      */
/*   Updated: 2025/12/02 09:48:06 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Main controller for the minimap system.
/// Handles rendering, user input (click/drag), and interaction with the world map.
/// Supports both PC (mouse) and mobile (touch) input.
/// </summary>
public class MinimapController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    #region Inspector Fields
    /// <summary>
    /// Событие, вызываемое после любого обновления миникарты (перерисовка, изменение тайлов, очистка и т.д.)
    /// </summary>
    public event Action OnMinimapUpdated;

    [Header("References")]
    [SerializeField] private RawImage minimapImage;

    [Header("Visual Settings")]
    [SerializeField] private Color emptyColor = new Color(0f, 0f, 0f, 0f);
    [SerializeField] private Color cameraFrameColor = Color.yellow;
    [SerializeField] private Color playerMarkerColor = Color.red;
    
    #endregion

    #region Private Fields
    
    /// <summary>Максимальный радиус миникарты (при максимальном zoom out)</summary>
    private const int MAX_VIEW_RADIUS = 30;
    
    /// <summary>Текущий радиус видимости (меняется при зуме)</summary>
    private int currentViewRadius;
    
    /// <summary>Number of tiles per side в текущем радиусе (e.g., 31 when viewRadius = 15)</summary>
    private int tilesPerSide;

    /// <summary>Texture representing the minimap (1 pixel = 1 tile) - всегда максимального размера</summary>
    private Texture2D minimapTexture;

    /// <summary>World coordinates of the minimap window's bottom-left corner</summary>
    private int originX;
    private int originY;

    /// <summary>Indicates texture needs Apply() call</summary>
    private bool dirty;

    /// <summary>Handles input detection (drag threshold, click vs drag)</summary>
    private MinimapInputHandler inputHandler;
    
    // ---- Новые поля для плавного drag ----
    private Vector2 dragStartLocalPos;      // точка, где зажали мышь/палец в локальных координатах миникарты
    private Vector2 dragStartCenterWorld;   // центр мира (в тайлах, но float) в момент начала drag
    
    #endregion

    #region Events & Properties
    
    /// <summary>Invoked when user clicks on minimap to jump camera to a tile</summary>
    public event Action<Vector2Int> OnMinimapTileClicked;

    /// <summary>
    /// Invoked when user drags on minimap.
    /// Param: desired world center (in tile coordinates, float) for camera/minimap.
    /// </summary>
    public event Action<Vector2> OnMinimapCenterDragged;

    /// <summary>Global flag preventing world input while minimap is being interacted with</summary>
    public static bool InputCaptured { get; private set; }

    /// <summary>Текущий радиус видимой области миникарты (read-only)</summary>
    public int ViewRadius => currentViewRadius;

    /// <summary>Текущее количество тайлов по стороне (read-only)</summary>
    public int TilesPerSide => tilesPerSide;
    
    #endregion

    #region Initialization
    
    /// <summary>
    /// Initializes the minimap system with the given view radius and player position.
    /// Creates the minimap texture (1 pixel = 1 world tile) and sets up input handlers.
    /// </summary>
    /// <param name="viewRadius">Radius of visible tiles around player</param>
    /// <param name="playerTile">Initial player tile position in world coordinates</param>
    public void Initialize(int viewRadius, Vector2Int playerTile)
    {
        currentViewRadius = viewRadius;
        tilesPerSide = viewRadius * 2 + 1;
        inputHandler = new MinimapInputHandler();

        // Создаём текстуру ОДИН РАЗ максимального размера
        if (minimapTexture == null)
        {
            int maxSize = MAX_VIEW_RADIUS * 2 + 1;
            minimapTexture = new Texture2D(
                maxSize,
                maxSize,
                TextureFormat.RGBA32,
                false
            );
            minimapTexture.filterMode = FilterMode.Point; // Crisp pixel art rendering
            minimapTexture.wrapMode = TextureWrapMode.Clamp;

            MinimapRenderer.ClearTexture(minimapTexture, emptyColor);
        }

        if (minimapImage == null)
        {
            minimapImage = GetComponent<RawImage>();
        }

        if (minimapImage != null)
        {
            minimapImage.texture = minimapTexture;
        }

        SetCenter(playerTile);
        UpdateUVRect();
    }
    
    /// <summary>
    /// Обновляет UV Rect для отображения только нужной области текстуры при текущем зуме.
    /// Работает как viewport в Canvas.
    /// </summary>
    private void UpdateUVRect()
    {
        if (minimapImage == null) return;
        
        int maxSize = MAX_VIEW_RADIUS * 2 + 1;
        int currentSize = tilesPerSide;
        
        // Показываем текущую область от центра текстуры
        // uvSize = какую долю текстуры показываем (1.0 = вся текстура, 0.5 = половина)
        float uvSize = (float)currentSize / maxSize;
        
        // uvOffset = смещение от левого нижнего угла, чтобы центрировать область
        float uvOffset = (1f - uvSize) / 2f;
        
        // Rect(x, y, width, height) где x,y - левый нижний угол в UV пространстве [0..1]
        minimapImage.uvRect = new Rect(uvOffset, uvOffset, uvSize, uvSize);
    }
    
    /// <summary>
    /// Изменяет радиус видимости миникарты без пересоздания текстуры.
    /// Вызывается при изменении зума.
    /// </summary>
    public void SetViewRadius(int newRadius)
    {
        newRadius = Mathf.Clamp(newRadius, 10, MAX_VIEW_RADIUS);
        
        if (currentViewRadius == newRadius) return;
        
        currentViewRadius = newRadius;
        tilesPerSide = newRadius * 2 + 1;
        
        UpdateUVRect();
    }
    
    #endregion

    #region Public Methods - Rendering
    
    /// <summary>
    /// Centers the minimap window on the specified world tile.
    /// Typically called when camera moves or player position changes.
    /// </summary>
    /// <param name="centerTile">World tile to center minimap on</param>
    public void SetCenter(Vector2Int centerTile)
    {
        // Используем MAX_VIEW_RADIUS для расчёта origin, чтобы текстура всегда охватывала максимальную область
        originX = centerTile.x - MAX_VIEW_RADIUS;
        originY = centerTile.y - MAX_VIEW_RADIUS;
    }

    /// <summary>
    /// Updates the color of a specific tile on the minimap.
    /// Used to reflect terrain changes or newly revealed areas.
    /// </summary>
    /// <param name="worldTile">World coordinates of the tile</param>
    /// <param name="color">Color to set (typically from tile's SpriteRenderer)</param>
    public void UpdateTile(Vector2Int worldTile, Color color)
    {
        int textureSize = MAX_VIEW_RADIUS * 2 + 1;
        if (MinimapRenderer.UpdateTile(minimapTexture, worldTile, originX, originY, textureSize, color))
        {
            dirty = true;
            OnMinimapUpdated?.Invoke();
        }
    }

    /// <summary>
    /// Draws the camera viewport frame on the minimap.
    /// Shows player what area is currently visible on screen.
    /// </summary>
    /// <param name="camMinTile">Bottom-left corner of camera view in world tiles</param>
    /// <param name="camMaxTile">Top-right corner of camera view in world tiles</param>
    public void DrawCameraFrame(Vector2Int camMinTile, Vector2Int camMaxTile)
    {
        int textureSize = MAX_VIEW_RADIUS * 2 + 1;
        MinimapRenderer.DrawCameraFrame(
            minimapTexture,
            camMinTile,
            camMaxTile,
            originX,
            originY,
            textureSize,
            cameraFrameColor
        );
        dirty = true;
        OnMinimapUpdated?.Invoke();
    }

    /// <summary>
    /// Draws the player position marker on the minimap.
    /// </summary>
    /// <param name="playerTile">Current player position in world tiles</param>
    public void DrawPlayerMarker(Vector2Int playerTile)
    {
        int textureSize = MAX_VIEW_RADIUS * 2 + 1;
        if (MinimapRenderer.DrawPlayerMarker(
            minimapTexture,
            playerTile,
            originX,
            originY,
            textureSize,
            playerMarkerColor))
        {
            dirty = true;
            OnMinimapUpdated?.Invoke();
        }
    }

    /// <summary>
    /// Clears the entire minimap texture to the empty color.
    /// Called before redrawing to prevent artifacts.
    /// </summary>
    public void Clear()
    {
        MinimapRenderer.ClearTexture(minimapTexture, emptyColor);
        dirty = true;
        OnMinimapUpdated?.Invoke();
    }
    
    #endregion

    #region Unity Lifecycle
    
    /// <summary>
    /// Applies accumulated texture changes once per frame for performance.
    /// Uses LateUpdate to ensure all rendering operations complete before Apply().
    /// </summary>
    private void LateUpdate()
    {
        if (dirty && minimapTexture != null)
        {
            minimapTexture.Apply();
            dirty = false;
            OnMinimapUpdated?.Invoke();
        }
    }
    
    #endregion

    #region Input Handling - EventSystem Callbacks
    
    /// <summary>
    /// Called when pointer is pressed down on the minimap.
    /// Captures input to prevent world interactions and prepares drag tracking.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        inputHandler?.OnPointerDown(eventData);
        InputCaptured = true;
        
        // Запоминаем стартовый центр мира (с учётом полной текстуры MAX_VIEW_RADIUS)
        int maxSize = MAX_VIEW_RADIUS * 2 + 1;
        dragStartCenterWorld = new Vector2(
            originX + maxSize * 0.5f,
            originY + maxSize * 0.5f
        );

        // И стартовую локальную позицию указателя относительно миникарты
        if (minimapImage != null)
        {
            RectTransform rectTransform = minimapImage.rectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out dragStartLocalPos
            );
        }
    }

    /// <summary>
    /// Called when pointer is released from the minimap.
    /// Processes click event if drag didn't occur, releases input capture.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (inputHandler != null && !inputHandler.IsDragging)
        {
            // короткий tap по миникарте — прыжок камеры в тайл
            ProcessMinimapClick(eventData);
        }

        inputHandler?.Reset();
        InputCaptured = false;
    }

    /// <summary>
    /// Handles minimap click event by invoking camera navigation.
    /// </summary>
    private void ProcessMinimapClick(PointerEventData eventData)
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
    /// Handles drag operations on the minimap.
    /// Smoothly pans the camera based on minimap drag delta.
    /// Ignores input if pointer leaves minimap bounds.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (inputHandler == null) 
            return;

        if (!inputHandler.CheckDragStart(eventData))
            return;

        if (minimapImage == null)
            return;

        RectTransform rectTransform = minimapImage.rectTransform;

        // Если ушли за пределы миникарты — игнорируем
        if (!RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera))
        {
            return;
        }

        // текущая локальная позиция курсора/пальца
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 currentLocalPos))
        {
            return;
        }

        // смещение в пикселях внутри миникарты
        Vector2 localDelta = currentLocalPos - dragStartLocalPos;

        // переводим пиксели → тайловые единицы
        // UI-Rect показывает часть текстуры через uvRect
        Rect rect = rectTransform.rect;
        if (Mathf.Approximately(rect.width, 0f) || Mathf.Approximately(rect.height, 0f))
            return;

        // Учитываем uvRect: видимая область текстуры
        Rect uvRect = minimapImage.uvRect;
        int maxSize = MAX_VIEW_RADIUS * 2 + 1;
        
        // Количество тайлов, видимых в текущем uvRect
        float visibleTilesX = maxSize * uvRect.width;
        float visibleTilesY = maxSize * uvRect.height;

        float tilesPerPixelX = visibleTilesX / rect.width;
        float tilesPerPixelY = visibleTilesY / rect.height;

        // Направление: тянем карту — камера двигается в противоположную сторону
        Vector2 worldDelta = new Vector2(
            -localDelta.x * tilesPerPixelX,
            -localDelta.y * tilesPerPixelY
        );

        Vector2 targetCenterWorld = dragStartCenterWorld + worldDelta;

        // Сообщаем наружу: "хочу, чтобы центр мира был вот здесь"
        OnMinimapCenterDragged?.Invoke(targetCenterWorld);
    }
    
    #endregion

    // Draws a unique marker (icon/color) on a minimap tile
    public void DrawUniqueMarker(Vector2Int tilePos, Color color)
    {
        Vector2Int localPos = tilePos - new Vector2Int(originX, originY);

        if (minimapTexture == null) return;
        
        // Проверяем границы по реальному размеру текстуры (MAX_VIEW_RADIUS)
        int textureSize = MAX_VIEW_RADIUS * 2 + 1;
        if (localPos.x < 0 || localPos.x >= textureSize || localPos.y < 0 || localPos.y >= textureSize)
            return;

        minimapTexture.SetPixel(localPos.x, localPos.y, color);
        dirty = true;
    }
}
