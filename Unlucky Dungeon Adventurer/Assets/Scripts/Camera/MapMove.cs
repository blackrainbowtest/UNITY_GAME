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

	// Current inertial velocity applied after drag/touch release
	private Vector3 velocity;
	// Screen position where dragging started
	private Vector3 dragOrigin;
	// Target orthographic size for smooth zooming
	private float targetZoom;

	// Reference to the map sprite used for clamping camera position
	private SpriteRenderer mapRenderer;

	void Start()
	{
		// Initialize zoom target and find the map renderer in children
		targetZoom = Camera.main.orthographicSize;
		mapRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	void Update()
	{
		HandleDrag();
		HandleZoom();
		ApplyInertia();
	}

	// Handle mouse drag and single-touch panning
	void HandleDrag()
	{
		// Mouse drag start
		if (Input.GetMouseButtonDown(0))
		{
			dragOrigin = Input.mousePosition;
			velocity = Vector3.zero;
			return;
		}

		// Mouse dragging (desktop)
		if (Input.GetMouseButton(0))
		{
			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
			// Make movement frame-rate independent and scale by zoom
			float scale = moveSpeed * Time.deltaTime * Camera.main.orthographicSize;
			Vector3 move = new Vector3(pos.x * scale, pos.y * scale, 0);
			transform.Translate(move, Space.World);
			// Scale velocity to simulate inertia (independent of FPS)
			velocity = move * 1000f; // larger to compensate for deltaTime scaling
			dragOrigin = Input.mousePosition;
		}

		// Single-touch panning (mobile)
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

	// Handle mouse wheel zoom and two-finger pinch zoom
	void HandleZoom()
	{
		// Mouse wheel zoom (desktop)
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (Mathf.Abs(scroll) > 0.01f)
			targetZoom -= scroll * zoomSpeed;

		// Two-finger pinch zoom (mobile)
		if (Input.touchCount == 2)
		{
			Touch t0 = Input.GetTouch(0);
			Touch t1 = Input.GetTouch(1);

			float prevDist = (t0.position - t0.deltaPosition - (t1.position - t1.deltaPosition)).magnitude;
			float currentDist = (t0.position - t1.position).magnitude;
			float diff = prevDist - currentDist;

			targetZoom += diff * Time.deltaTime * 0.5f;
		}

		// Clamp and smoothly apply zoom
		targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
		Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime * zoomSmooth);

		// Keep camera inside map bounds after zoom changes
		ClampPosition();
	}

	// Apply inertial movement after drag ends
	void ApplyInertia()
	{
		if (velocity.magnitude > 0.01f)
		{
			transform.Translate(velocity * Time.deltaTime, Space.World);
			velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * dragDamping);
			ClampPosition();
		}
	}

	// Clamp camera position to the bounds of the map sprite
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
