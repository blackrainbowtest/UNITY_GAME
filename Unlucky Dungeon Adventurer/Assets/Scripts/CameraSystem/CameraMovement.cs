/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/CameraSystem/CameraMovement.cs                      */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 10:19:14 by UDA                                      */
/*   Updated: 2025/12/02 10:19:14 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public class CameraMovement
{
	private Camera cam;

	private Vector3 dragOrigin;
	private Vector3 velocity;    // инерция
	private float inertiaDamp = 6f;
	private float minInertia = 0.01f;
	private float moveFactor = 0.0025f;

	public CameraMovement(Camera camera)
	{
		cam = camera;
	}

	public void ProcessDrag()
	{
		bool dragging = false;
		Vector3 delta = Vector3.zero;

		// === PC DRAG ===
		if (Input.GetMouseButtonDown(0))
		{
			dragOrigin = Input.mousePosition;
			velocity = Vector3.zero;
		}
		else if (Input.GetMouseButton(0))
		{
			Vector3 cur = Input.mousePosition;
			delta = cur - dragOrigin;
			dragOrigin = cur;
			dragging = true;

			MoveCamera(delta);
		}

		// === MOBILE DRAG ===
		if (Input.touchCount == 1)
		{
			Touch t = Input.GetTouch(0);

			if (t.phase == TouchPhase.Began)
			{
				dragOrigin = t.position;
				velocity = Vector3.zero;
			}
			else if (t.phase == TouchPhase.Moved)
			{
				delta = t.position - dragOrigin;
				dragOrigin = t.position;
				dragging = true;

				MoveCamera(delta);
			}
		}

		// === INERTIA ===
		if (dragging)
		{
			velocity = delta;
		}
		else
		{
			if (velocity.magnitude > minInertia)
			{
				MoveCamera(velocity);
				velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * inertiaDamp);
			}
		}
	}

	private void MoveCamera(Vector3 pixelDelta)
	{
		Vector3 worldDelta =
			cam.ScreenToWorldPoint(pixelDelta) -
			cam.ScreenToWorldPoint(Vector3.zero);

		cam.transform.position -= worldDelta;
	}
}
