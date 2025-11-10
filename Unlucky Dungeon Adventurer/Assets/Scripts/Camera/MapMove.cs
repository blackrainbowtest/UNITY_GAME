using UnityEngine;

public class MapMove : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public Vector2 minBounds = new Vector2(-1000, -1000); // минимальные координаты
    public Vector2 maxBounds = new Vector2(1000, 1000);   // максимальные координаты

    private Vector3 dragOrigin;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * moveSpeed, pos.y * moveSpeed, 0);

        transform.Translate(move, Space.World);

        // ограничиваем движение по границам
        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minBounds.x, maxBounds.x);
        clampedPos.y = Mathf.Clamp(clampedPos.y, minBounds.y, maxBounds.y);
        transform.position = clampedPos;

        dragOrigin = Input.mousePosition;
    }
}
