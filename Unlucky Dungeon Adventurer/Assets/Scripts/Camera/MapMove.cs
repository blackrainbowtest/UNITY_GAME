using UnityEngine;

public class MapMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 0.5f;
    public float dragDamping = 8f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float zoomSmooth = 10f;
    public float minZoom = 2f;
    public float maxZoom = 8f;

    private Vector3 velocity;
    private Vector3 dragOrigin;
    private float targetZoom;

    private SpriteRenderer mapRenderer;

    void Start()
    {
        targetZoom = Camera.main.orthographicSize;
        mapRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        HandleDrag();
        HandleZoom();
        ApplyInertia();
    }

    void HandleDrag()
    {
        // мышь (ПК)
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            velocity = Vector3.zero;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * moveSpeed, pos.y * moveSpeed, 0);
            transform.Translate(move, Space.World);
            velocity = move * 50f;
            dragOrigin = Input.mousePosition;
        }

        // палец (тач)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 delta = touch.deltaPosition;
                Vector3 move = new Vector3(-delta.x * moveSpeed * Time.deltaTime, -delta.y * moveSpeed * Time.deltaTime, 0);
                transform.Translate(move, Space.World);
                velocity = move * 500f;
            }
        }

        ClampPosition();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
            targetZoom -= scroll * zoomSpeed;

        // пинч на Android
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            float prevDist = (t0.position - t0.deltaPosition - (t1.position - t1.deltaPosition)).magnitude;
            float currentDist = (t0.position - t1.position).magnitude;
            float diff = prevDist - currentDist;

            targetZoom += diff * Time.deltaTime * 0.5f;
        }

        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime * zoomSmooth);

        ClampPosition(); // пересчитываем границы при зуме
    }

    void ApplyInertia()
    {
        if (velocity.magnitude > 0.01f)
        {
            transform.Translate(velocity * Time.deltaTime, Space.World);
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * dragDamping);
            ClampPosition();
        }
    }

    void ClampPosition()
    {
        if (mapRenderer == null) return;

        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;

        float mapWidth = mapRenderer.bounds.size.x;
        float mapHeight = mapRenderer.bounds.size.y;

        float limitX = Mathf.Max(0, (mapWidth - camWidth) / 2f);
        float limitY = Mathf.Max(0, (mapHeight - camHeight) / 2f);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -limitX, limitX);
        pos.y = Mathf.Clamp(pos.y, -limitY, limitY);
        transform.position = pos;
    }
}
