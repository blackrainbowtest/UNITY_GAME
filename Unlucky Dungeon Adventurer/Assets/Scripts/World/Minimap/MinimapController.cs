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
    
    [Header("References")]
    [SerializeField] private RawImage minimapImage;

    [Header("Visual Settings")]
    [SerializeField] private Color emptyColor = new Color(0f, 0f, 0f, 0f);
    [SerializeField] private Color cameraFrameColor = Color.yellow;
    [SerializeField] private Color playerMarkerColor = Color.red;
    
    #endregion

    #region Private Fields
    
    /// <summary>Number of tiles per side (e.g., 31 when viewRadius = 15)</summary>
    private int tilesPerSide;

    /// <summary>Texture representing the minimap (1 pixel = 1 tile)</summary>
    private Texture2D minimapTexture;

    /// <summary>World coordinates of the minimap window's bottom-left corner</summary>
    private int originX;
    private int originY;

    /// <summary>Indicates texture needs Apply() call</summary>
    private bool dirty;

    /// <summary>Handles input detection (drag threshold, click vs drag)</summary>
    private MinimapInputHandler inputHandler;
    
    /// <summary>Tracks drag state for camera movement</summary>
    private Vector2Int lastDragTile;
    private Vector2Int dragStartCenterTile;
    
    #endregion

    #region Events & Properties
    
    /// <summary>Invoked when user clicks/drags on minimap to navigate world</summary>
    public event Action<Vector2Int> OnMinimapTileClicked;

    /// <summary>Global flag preventing world input while minimap is being interacted with</summary>
    public static bool InputCaptured { get; private set; }
    
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
        tilesPerSide = viewRadius * 2 + 1;
        inputHandler = new MinimapInputHandler();

        // Create texture: 1 pixel per tile, no mipmaps
        minimapTexture = new Texture2D(
            tilesPerSide,
            tilesPerSide,
            TextureFormat.RGBA32,
            false
        );
        minimapTexture.filterMode = FilterMode.Point; // Crisp pixel art rendering
        minimapTexture.wrapMode = TextureWrapMode.Clamp;

        MinimapRenderer.ClearTexture(minimapTexture, emptyColor);

        if (minimapImage == null)
        {
            minimapImage = GetComponent<RawImage>();
        }

        if (minimapImage != null)
        {
            minimapImage.texture = minimapTexture;
        }

        SetCenter(playerTile);
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
        originX = centerTile.x - tilesPerSide / 2;
        originY = centerTile.y - tilesPerSide / 2;
    }

    /// <summary>
    /// Updates the color of a specific tile on the minimap.
    /// Used to reflect terrain changes or newly revealed areas.
    /// </summary>
    /// <param name="worldTile">World coordinates of the tile</param>
    /// <param name="color">Color to set (typically from tile's SpriteRenderer)</param>
    public void UpdateTile(Vector2Int worldTile, Color color)
    {
        if (MinimapRenderer.UpdateTile(minimapTexture, worldTile, originX, originY, tilesPerSide, color))
        {
            dirty = true;
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

    /// <summary>
    /// Draws the player position marker on the minimap.
    /// </summary>
    /// <param name="playerTile">Current player position in world tiles</param>
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
    /// Clears the entire minimap texture to the empty color.
    /// Called before redrawing to prevent artifacts.
    /// </summary>
    public void Clear()
    {
        MinimapRenderer.ClearTexture(minimapTexture, emptyColor);
        dirty = true;
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

    /// <summary>
    /// Called when pointer is released from the minimap.
    /// Processes click event if drag didn't occur, releases input capture.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (inputHandler != null && !inputHandler.IsDragging)
        {
            ProcessMinimapInteraction(eventData);
        }
        inputHandler?.Reset();
        InputCaptured = false;
    }

    /// <summary>
    /// Handles minimap click event by invoking camera navigation.
    /// </summary>
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
    /// Handles drag operations on the minimap.
    /// Smoothly pans the camera based on minimap drag delta.
    /// Ignores input if pointer leaves minimap bounds.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (inputHandler == null) return;

        if (!inputHandler.CheckDragStart(eventData))
        {
            return;
        }

        if (MinimapCoordinateConverter.TryGetWorldTile(
            eventData,
            minimapImage,
            tilesPerSide,
            originX,
            originY,
            out Vector2Int currentTile))
        {
            Vector2Int delta = lastDragTile - currentTile;
            Vector2Int targetTile = dragStartCenterTile + delta;
            
            OnMinimapTileClicked?.Invoke(targetTile);
        }
    }
    
    #endregion
}
