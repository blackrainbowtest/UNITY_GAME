/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/CameraSystem/Modules/CameraZoomModule.cs            */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 11:45:05 by UDA                                      */
/*   Updated: 2025/12/02 11:45:05 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public class CameraZoomModule
{
	private Camera cam;

	// --- zoom state ---
	private float targetZoom;          // desired zoom
	private float currentVelocity;     // smooth damp velocity

	// --- settings ---
	private float zoomSmooth = 0.1f;   // smoothing time (lower = faster, less jitter)
	private float pinchSensitivity = 0.005f;
	private float wheelSensitivity = 2f;   // reduced for smoother zoom

	private float minZoom;
	private float maxZoom;

	// for pinch zoom
	private float lastPinchDistance;

	public CameraZoomModule(Camera camera, float minZoom, float maxZoom, float smooth = 0.15f)
	{
		cam = camera;
		this.minZoom = minZoom;
		this.maxZoom = maxZoom;
		zoomSmooth = smooth;

		targetZoom = cam.orthographicSize;
	}

	// =============================================================
	// Update every frame (call from LateUpdate for smooth rendering)
	// =============================================================
	public void UpdateZoom()
	{
		ProcessMouseWheel();
		ProcessPinch();

		// Smooth damp zoom - clamp to prevent overshooting
		float newSize = Mathf.SmoothDamp(
			cam.orthographicSize,
			targetZoom,
			ref currentVelocity,
			zoomSmooth
		);

		// Additional clamping to prevent jitter at boundaries
		cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
	}

	// =============================================================
	// PC mouse wheel zoom
	// =============================================================
	private void ProcessMouseWheel()
	{
		float scroll = Input.mouseScrollDelta.y;
		if (Mathf.Abs(scroll) < 0.01f)
			return;

		targetZoom -= scroll * wheelSensitivity;
		ClampZoom();
	}

	// =============================================================
	// Mobile pinch zoom
	// =============================================================
	private void ProcessPinch()
	{
		if (Input.touchCount != 2)
			return;

		Touch t0 = Input.GetTouch(0);
		Touch t1 = Input.GetTouch(1);

		float curDist = Vector2.Distance(t0.position, t1.position);

		if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
		{
			lastPinchDistance = curDist;
			return;
		}

		float delta = curDist - lastPinchDistance;
		lastPinchDistance = curDist;

		targetZoom -= delta * pinchSensitivity;
		ClampZoom();
	}

	// =============================================================
	// Helpers
	// =============================================================
	private void ClampZoom()
	{
		targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
	}

	// =============================================================
	// Public API (CameraMaster calls these)
	// =============================================================
	public void ZoomIn(float amount)
	{
		targetZoom -= amount;
		ClampZoom();
	}

	public void ZoomOut(float amount)
	{
		targetZoom += amount;
		ClampZoom();
	}

	public void SetZoom(float value)
	{
		targetZoom = Mathf.Clamp(value, minZoom, maxZoom);
	}

	public float GetZoom() => targetZoom;
}