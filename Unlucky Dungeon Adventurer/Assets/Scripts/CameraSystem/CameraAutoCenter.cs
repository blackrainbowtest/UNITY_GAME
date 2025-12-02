/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/CameraSystem/CameraAutoCenter.cs                    */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 10:30:39 by UDA                                      */
/*   Updated: 2025/12/02 10:30:39 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public class CameraAutoCenter
{
	private Camera cam;

	private bool active = false;
	private Vector3 target;

	// speed units per second
	private float moveSpeed = 22f;

	// at what distance do we consider we have reached
	private float stopDistance = 0.05f;

	public CameraAutoCenter(Camera camera)
	{
		cam = camera;
	}

	public void StartAutoCenter(Vector3 worldPos)
	{
		target = new Vector3(worldPos.x, worldPos.y, cam.transform.position.z);
		active = true;
	}

	public void Cancel()
	{
		active = false;
	}

	public bool IsActive => active;

	public void Update()
	{
		if (!active) return;

		// If the player starts clicking with a finger/mouse, CANCEL
		if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
		{
			active = false;
			return;
		}

		Vector3 current = cam.transform.position;

		// fixed speed movement
		cam.transform.position = Vector3.MoveTowards(
			current,
			target,
			moveSpeed * Time.deltaTime
		);

		// We're checking to see if they arrived.
		if (Vector3.Distance(cam.transform.position, target) < stopDistance)
		{
			cam.transform.position = target;
			active = false;
		}
	}
}
