/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/World/Player/PlayerMarkerController.cs              */
/*                                                        /\_/\               */
/*                                                       ( вЂў.вЂў )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 09:48:32 by UDA                                      */
/*   Updated: 2025/12/02 09:48:32 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

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

    // С‚РµРєСѓС‰Р°СЏ РїРѕР·РёС†РёСЏ РёРіСЂРѕРєР° РІ РјРёСЂРѕРІРѕР№ СЃРµС‚РєРµ
    public Vector2Int mapCoords;

    private void Awake()
    {
        // Получить существующий SpriteRenderer или создать новый
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // Установить спрайт, если назначен в инспекторе
        if (playerSprite != null)
        {
            spriteRenderer.sprite = playerSprite;
        }
    }

    private void Update()
    {
        HandleLongPress();
    }

    // =============================
    //         РџР•Р Р•Р”Р’РР–Р•РќРР•
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
        // UDADebug.Log($"[PlayerMarker] MoveTo called: {newCoords}");
        SetPosition(newCoords, tileSize);
    }

    // =============================
    //      РљР›РРљР / РЈР”Р•Р Р–РђРќРР•
    // =============================

    public void OnPointerClick(PointerEventData eventData)
    {
        // UDADebug.Log("[PlayerMarker] CLICK on player marker");
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
            // РєРѕСЂРѕС‚РєРѕРµ РЅР°Р¶Р°С‚РёРµ (РЅРѕ РѕР±СЂР°Р±РѕС‚РєР° РєР»РёРєР° СѓР¶Рµ РµСЃС‚СЊ РІС‹С€Рµ)
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
            // UDADebug.Log("[PlayerMarker] LONG PRESS on player marker");
            isPressing = false;
        }
    }
}

