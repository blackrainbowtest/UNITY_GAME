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
	private float dragSpeed = 0.005f;			// sensitivity (PC)
	private float touchDragSpeed = 0.002f;		// sensitivity (mobile)
	private float inertiaDamping = 6f;
	private Vector3 inertia = Vector3.zero;

	private Vector3 lastScreenPos;
	private bool dragging = false;

	// --- ZOOM SETTINGS ---
	private int[] zoomSteps = new int[] { 6, 8, 10, 12, 14, 16, 18 };
	private int zoomIndex = 2;
	private float pinchLastDistance;

	public CameraPanController(Camera c)
	{
		cam = c;
		cam.orthographicSize = zoomSteps[zoomIndex];
	}

	// =====================================================================
	//                           PUBLIC API
	// =====================================================================

	public void Update()
	{
		HandleMousePan();
		HandleTouchPan();
		HandleMouseZoom();
		HandlePinchZoom();
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
			lastScreenPos = Input.mousePosition;
			inertia = Vector3.zero;
		}

		if (Input.GetMouseButton(0) && dragging)
		{
			Vector3 cur = Input.mousePosition;
			Vector3 delta = cur - lastScreenPos;
			lastScreenPos = cur;

			Vector3 worldDelta =
				cam.ScreenToWorldPoint(delta) -
				cam.ScreenToWorldPoint(Vector3.zero);

			cam.transform.position -= worldDelta * dragSpeed;
			inertia = worldDelta * 50f; 
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
			Vector3 delta = t.position - lastScreenPos;
			lastScreenPos = t.position;

			Vector3 worldDelta =
				cam.ScreenToWorldPoint(delta) -
				cam.ScreenToWorldPoint(Vector3.zero);

			cam.transform.position -= worldDelta * touchDragSpeed;
			inertia = worldDelta * 20f;
		}
		else if (t.phase == TouchPhase.Ended)
		{
			dragging = false;
		}
	}

	// =====================================================================
	//                           PC: ZOOM WHEEL
	// =====================================================================

	private void HandleMouseZoom()
	{
		float scroll = Input.mouseScrollDelta.y;
		if (Mathf.Abs(scroll) < 0.01f) return;

		if (scroll > 0 && zoomIndex > 0)
			zoomIndex--;
		else if (scroll < 0 && zoomIndex < zoomSteps.Length - 1)
			zoomIndex++;

		cam.orthographicSize = zoomSteps[zoomIndex];
	}

	// =====================================================================
	//                           MOBILE: PINCH-ZOOM
	// =====================================================================

	private void HandlePinchZoom()
	{
		if (Input.touchCount < 2) return;

		Touch t0 = Input.GetTouch(0);
		Touch t1 = Input.GetTouch(1);

		float curDist = Vector2.Distance(t0.position, t1.position);

		if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
		{
			pinchLastDistance = curDist;
			return;
		}

		float delta = curDist - pinchLastDistance;
		pinchLastDistance = curDist;

		if (Mathf.Abs(delta) < 6f) return; 		// bounce protection

		if (delta > 0 && zoomIndex > 0)
			zoomIndex--;
		else if (delta < 0 && zoomIndex < zoomSteps.Length - 1)
			zoomIndex++;

		cam.orthographicSize = zoomSteps[zoomIndex];
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
