/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/CameraSystem/CameraZoom.cs                          */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 10:23:06 by UDA                                      */
/*   Updated: 2025/12/02 10:23:06 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public class CameraZoom
{
	private Camera cam;

	// дискретные уровни зума (можешь менять под вкус игры)
	private int[] zoomSteps = new int[] { 8, 10, 12, 14, 16, 20, 24 };
	private int zoomIndex = 2; // стартуем где-то по середине

	private float pinchThreshold = 10f; // чувствительность для мобилки

	// для pinch zoom
	private float lastPinchDistance = 0f;

	public CameraZoom(Camera camera)
	{
		cam = camera;
		zoomIndex = ClosestIndex(cam.orthographicSize);
		cam.orthographicSize = zoomSteps[zoomIndex];
	}

	public void ProcessZoom()
	{
		ProcessMouseWheel();
		ProcessPinch();
	}

	// ================================
	// PC Zoom (mouse wheel)
	// ================================
	private void ProcessMouseWheel()
	{
		float scroll = Input.mouseScrollDelta.y;
		if (Mathf.Abs(scroll) < 0.01f)
			return;

		if (scroll > 0f)
			zoomIndex = Mathf.Max(0, zoomIndex - 1);
		else
			zoomIndex = Mathf.Min(zoomSteps.Length - 1, zoomIndex + 1);

		cam.orthographicSize = zoomSteps[zoomIndex];
	}

	// ================================
	// Mobile Pinch Zoom (two fingers)
	// ================================
	private void ProcessPinch()
	{
		if (Input.touchCount != 2)
			return;

		Touch t0 = Input.GetTouch(0);
		Touch t1 = Input.GetTouch(1);

		if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
		{
			lastPinchDistance = Vector2.Distance(t0.position, t1.position);
			return;
		}

		float curDist = Vector2.Distance(t0.position, t1.position);
		float delta = curDist - lastPinchDistance;
		lastPinchDistance = curDist;

		if (Mathf.Abs(delta) < pinchThreshold)
			return;

		if (delta > 0f)
			zoomIndex = Mathf.Max(0, zoomIndex - 1);
		else
			zoomIndex = Mathf.Min(zoomSteps.Length - 1, zoomIndex + 1);

		cam.orthographicSize = zoomSteps[zoomIndex];
	}

	private int ClosestIndex(float size)
	{
		int best = 0;
		float min = float.MaxValue;

		for (int i = 0; i < zoomSteps.Length; i++)
		{
			float d = Mathf.Abs(zoomSteps[i] - size);
			if (d < min)
			{
				min = d;
				best = i;
			}
		}
		return best;
	}
}
