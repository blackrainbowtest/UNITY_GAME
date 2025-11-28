using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMarkerController : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Sprite Setup")]
    public Sprite playerSprite;
    private SpriteRenderer spriteRenderer;

    [Header("Long Press Settings")]
    public float longPressThreshold = 0.35f;

    private bool isPressing = false;
    private float pressTimer = 0f;

    // текущая позиция игрока в мировой сетке
    public Vector2Int mapCoords;

    private void Awake()
    {
        // создаём визуальную часть
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = playerSprite;
        spriteRenderer.sortingOrder = 50; // поверх тайлов
    }

    private void Update()
    {
        HandleLongPress();
    }

    // =============================
    //         ПЕРЕДВИЖЕНИЕ
    // =============================

    public void SetPosition(Vector2Int coords, float tileSize)
    {
        mapCoords = coords;
        transform.position = new Vector3(
            coords.x * tileSize,
            coords.y * tileSize,
            -0.1f
        );
    }

    public void MoveTo(Vector2Int newCoords, float tileSize)
    {
        // Debug.Log($"[PlayerMarker] MoveTo called: {newCoords}");
        SetPosition(newCoords, tileSize);
    }

    // =============================
    //      КЛИКИ / УДЕРЖАНИЕ
    // =============================

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("[PlayerMarker] CLICK on player marker");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressing = true;
        pressTimer = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressing && pressTimer < longPressThreshold)
        {
            // короткое нажатие (но обработка клика уже есть выше)
        }

        isPressing = false;
        pressTimer = 0f;
    }

    private void HandleLongPress()
    {
        if (!isPressing)
            return;

        pressTimer += Time.deltaTime;

        if (pressTimer >= longPressThreshold)
        {
            // Debug.Log("[PlayerMarker] LONG PRESS on player marker");
            isPressing = false;
        }
    }
}
