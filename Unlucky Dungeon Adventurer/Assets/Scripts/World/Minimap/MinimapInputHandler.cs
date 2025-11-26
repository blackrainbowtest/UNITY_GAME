using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles input detection for the minimap, distinguishing between clicks and drags.
/// Uses a small pixel threshold to prevent accidental drag activation on taps.
/// </summary>
public class MinimapInputHandler
{
    private bool isDragging;
    private Vector2 pointerDownPosition;
    
    /// <summary>Minimum pixel distance to activate drag (prevents accidental drag on tap)</summary>
    private const float dragThreshold = 2f;

    /// <summary>Whether drag operation is currently active</summary>
    public bool IsDragging => isDragging;

    /// <summary>
    /// Records initial pointer position when pressed.
    /// Call this from OnPointerDown.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false;
        pointerDownPosition = eventData.position;
    }

    /// <summary>
    /// Checks if pointer has moved far enough to activate drag.
    /// Returns true once threshold is exceeded.
    /// </summary>
    /// <param name="eventData">Current pointer event data</param>
    /// <returns>True if drag should be processed</returns>
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
    /// Resets drag state. Call this from OnPointerUp.
    /// </summary>
    public void Reset()
    {
        isDragging = false;
    }
}
