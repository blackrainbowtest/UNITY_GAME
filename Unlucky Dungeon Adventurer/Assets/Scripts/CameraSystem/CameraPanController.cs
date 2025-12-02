/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/CameraSystem/CameraPanController.cs                 */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 10:34:29 by UDA                                      */
/*   Updated: 2025/12/02 10:34:29 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public class CameraPanController
{
	private Camera cam;

	// --- PAN SETTINGS ---
	private float dragSpeed = 16f;				// reduced sensitivity (was 50)
	private float touchDragSpeed = 0.002f;		// sensitivity (mobile)
	private float inertiaDamping = 4f;			// Lower = longer inertia (was 6)
	private float inertiaMultiplier = 0.2f;		// Stronger inertia capture
	private float zoomScaleFactor = 0.05f;		// Scale movement with zoom level
	private Vector3 inertia = Vector3.zero;

	private Vector3 lastScreenPos;
	private bool dragging = false;
	
	// Track if this was a drag or a click
	private Vector3 dragStartPos;
	private const float dragThreshold = 5f;  // pixels
	public bool IsDragging => dragging && Vector3.Distance(Input.mousePosition, dragStartPos) > dragThreshold;

	public CameraPanController(Camera c)
	{
		cam = c;
	}

	// =====================================================================
	//                           PUBLIC API
	// =====================================================================

	public void Update()
	{
		HandleMousePan();
		HandleTouchPan();
		// Zoom removed - handled by CameraZoomModule in CameraMaster
		ApplyInertia();
	}

	public void CancelInertia()
	{
		inertia = Vector3.zero;
	}

	// =====================================================================
	//                           PC: MOVEMENT
	// =====================================================================

	private void HandleMousePan()
	{
		if (Input.touchCount > 0) return;		// mobile input takes priority

		if (Input.GetMouseButtonDown(0))
		{
			dragging = true;
			dragStartPos = Input.mousePosition;
			lastScreenPos = Input.mousePosition;
			inertia = Vector3.zero;
		}

		if (Input.GetMouseButton(0) && dragging)
		{
			Vector3 cur = Input.mousePosition;
			Vector3 delta = cur - lastScreenPos;
			lastScreenPos = cur;

			// Scale movement by current zoom level (orthographicSize)
			float zoomScale = cam.orthographicSize * zoomScaleFactor;

			// Move camera directly in screen space
			Vector3 move = new Vector3(-delta.x, -delta.y, 0) * dragSpeed * Time.deltaTime * zoomScale;
			cam.transform.position += cam.transform.TransformDirection(move);
			
			// Capture velocity for inertia (stronger capture), also scaled by zoom
			inertia = delta * inertiaMultiplier * zoomScale;
		}

		if (Input.GetMouseButtonUp(0))
			dragging = false;
	}

	// =====================================================================
	//                           MOBILE: MOVEMENT
	// =====================================================================

	private void HandleTouchPan()
	{
		if (Input.touchCount != 1)
			return;

		Touch t = Input.GetTouch(0);

		if (t.phase == TouchPhase.Began)
		{
			dragging = true;
			lastScreenPos = t.position;
			inertia = Vector3.zero;
		}
		else if (t.phase == TouchPhase.Moved)
		{
			// Explicitly cast Vector2 to Vector3 to avoid ambiguous operator
			Vector3 delta = (Vector3)t.position - lastScreenPos;
			lastScreenPos = (Vector3)t.position;

			Vector3 worldDelta =
				cam.ScreenToWorldPoint(delta) -
				cam.ScreenToWorldPoint(Vector3.zero);

			// Scale by zoom level for consistent feel
			float zoomScale = cam.orthographicSize * zoomScaleFactor;
			
			cam.transform.position -= worldDelta * touchDragSpeed;
			// Capture velocity for inertia, scaled by zoom
			inertia = worldDelta * 30f * zoomScale;
		}
		else if (t.phase == TouchPhase.Ended)
		{
			dragging = false;
		}
	}

	// =====================================================================
	//                         INERTION
	// =====================================================================

	private void ApplyInertia()
	{
		if (dragging) return;

		if (inertia.magnitude > 0.01f)
		{
			cam.transform.position -= inertia * Time.deltaTime;
			inertia = Vector3.Lerp(inertia, Vector3.zero, Time.deltaTime * inertiaDamping);
		}
	}
}
