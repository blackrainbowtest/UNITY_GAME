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
	private float zoomSmooth = 0.15f;  // smoothing time (lower = faster)
	private float pinchSensitivity = 0.005f;
	private float wheelSensitivity = 3f;

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
	// Update every frame
	// =============================================================
	public void UpdateZoom()
	{
		ProcessMouseWheel();
		ProcessPinch();

		// Smooth damp zoom
		cam.orthographicSize = Mathf.SmoothDamp(
			cam.orthographicSize,
			targetZoom,
			ref currentVelocity,
			zoomSmooth
		);
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