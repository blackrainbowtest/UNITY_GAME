using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Обработчик ввода для миникарты (клики, перетаскивание)
/// </summary>
public class MinimapInputHandler
{
    private bool isDragging;
    private Vector2 pointerDownPosition;
    private const float dragThreshold = 2f; // Reduced threshold for better sensitivity

    public bool IsDragging => isDragging;

    /// <summary>
    /// Вызывается при нажатии на миникарту
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false;
        pointerDownPosition = eventData.position;
    }

    /// <summary>
    /// Проверяет, начался ли drag (превышен порог движения)
    /// </summary>
    public bool CheckDragStart(PointerEventData eventData)
    {
        if (isDragging)
            return true;

        float distance = Vector2.Distance(pointerDownPosition, eventData.position);
        if (distance > dragThreshold)
        {
            isDragging = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Сбрасывает состояние drag
    /// </summary>
    public void Reset()
    {
        isDragging = false;
    }
}
