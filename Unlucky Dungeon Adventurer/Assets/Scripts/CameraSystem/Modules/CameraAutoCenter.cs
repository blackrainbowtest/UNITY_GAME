/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/CameraSystem/Modules/CameraAutoCenter.cs            */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 10:30:39 by UDA                                      */
/*   Updated: 2025/12/02 11:44:16 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public class CameraAutoCenter
{
	private Camera cam;

	private bool active = false;
	private Vector3 target;

	private float moveSpeed;
	private float stopDistance;

	public bool IsActive => active;

	public CameraAutoCenter(Camera camera, float speed, float stopDist)
	{
		cam = camera;
		moveSpeed = speed;
		stopDistance = stopDist;
	}

	public void StartAutoCenter(Vector3 worldPos)
	{
		// Cancel any ongoing animation to prevent stacking
		if (active)
		{
			active = false;
		}
		
		target = new Vector3(worldPos.x, worldPos.y, cam.transform.position.z);
		active = true;
	}

	public void Cancel()
	{
		active = false;
	}

	public void UpdateAutoCenter()
	{
		if (!active) return;

		// If the player tries to interact, cancel the autoflight
		if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
		{
			active = false;
			return;
		}

		Vector3 current = cam.transform.position;

		cam.transform.position = Vector3.MoveTowards(
			current,
			target,
			moveSpeed * Time.deltaTime
		);

		if (Vector3.Distance(cam.transform.position, target) <= stopDistance)
		{
			cam.transform.position = target;
			active = false;
		}
	}
}

