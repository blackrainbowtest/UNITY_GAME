/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/CameraSystem/Modules/CameraPanModule.cs             */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 11:22:13 by UDA                                      */
/*   Updated: 2025/12/02 11:44:44 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public class CameraPanModule
{
	// === PARAMETERS (configurable from CameraMaster) ===
	private readonly Camera cam;
	private readonly float dragSpeed;      // скорость panning
	private readonly float inertiaDamp;    // затухание инерции
	private readonly float minInertia;     // порог, после которого инерция исчезает

	// === INTERNAL STATE ===
	private Vector3 lastScreenPos;
	private Vector3 inertia;

	public CameraPanModule(Camera camera, float dragSpeed = 0.005f,
		float inertiaDamp = 0.9f, float minInertia = 0.01f)
	{
		cam = camera;
		this.dragSpeed = dragSpeed;
		this.inertiaDamp = inertiaDamp;
		this.minInertia = minInertia;
	}

	// ======================================================================
	//                           MAIN PAN LOGIC
	// ======================================================================

	public void ProcessPan()
	{
		if (cam == null) return;

		// Minimap captures input
		if (MinimapController.InputCaptured)
		{
			inertia = Vector3.zero;
			return;
		}

		bool dragging = false;
		Vector3 delta = Vector3.zero;

		// ------------------------------
		// PC MOUSE DRAG
		// ------------------------------
		if (Input.GetMouseButtonDown(0))
		{
			lastScreenPos = Input.mousePosition;
			inertia = Vector3.zero;
		}
		else if (Input.GetMouseButton(0))
		{
			Vector3 current = Input.mousePosition;
			delta = current - lastScreenPos;
			lastScreenPos = current;
			dragging = true;

			PanCamera(delta);
		}

		// ------------------------------
		// MOBILE TOUCH DRAG
		// ------------------------------
		if (Input.touchCount == 1)
		{
			Touch t = Input.GetTouch(0);

			if (t.phase == TouchPhase.Began)
			{
				lastScreenPos = t.position;
				inertia = Vector3.zero;
			}
			else if (t.phase == TouchPhase.Moved)
			{
				Vector3 current = t.position;
				delta = current - lastScreenPos;
				lastScreenPos = current;
				dragging = true;

				PanCamera(delta);
			}
		}

		// INERTIA UPDATE
		if (dragging)
		{
			inertia = delta * 1.4f;				// we enhance the “weight” effect
		}
	}

	// ======================================================================
	//                             APPLY INERTIA
	// ======================================================================

	public void ApplyInertia()
	{
		if (cam == null) return;

		if (inertia.magnitude > minInertia)
		{
			Vector3 worldDelta = cam.ScreenToWorldPoint(inertia) - cam.ScreenToWorldPoint(Vector3.zero);
			cam.transform.position -= worldDelta;

			inertia *= inertiaDamp;
		}
	}

	// ======================================================================
	//                        HELPERS
	// ======================================================================

	private void PanCamera(Vector3 deltaPixels)
	{
		Vector3 deltaWorld =
			cam.ScreenToWorldPoint(deltaPixels) - cam.ScreenToWorldPoint(Vector3.zero);

		cam.transform.position -= deltaWorld * dragSpeed;
	}

	public void Cancel()
	{
		inertia = Vector3.zero;
	}
}