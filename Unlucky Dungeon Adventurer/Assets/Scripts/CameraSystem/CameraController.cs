/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/CameraSystem/CameraController.cs                    */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 09:57:10 by UDA                                      */
/*   Updated: 2025/12/02 09:57:10 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController Instance { get; private set; }

	private CameraMovement movement;
	private CameraZoom zoom;
	private CameraAuto auto;

	private Camera cam;

	void Awake()
	{
		Instance = this;

		cam = Camera.main;

		movement = new CameraMovement(cam);
		zoom = new CameraZoom(cam);
		auto = new CameraAuto(cam);
	}

	void Update()
	{
		// If the camera is in auto-move mode, it doesn't respond to clicks.
		if (auto.IsActive)
		{
			auto.UpdateAuto();
			return;
		}

		movement.ProcessDrag();
		zoom.ProcessZoom();
	}

	// ====== PUBLIC API for calling from other parts of the game ======

	public void MoveTo(Vector3 worldPos)
	{
		auto.StartAutoMove(worldPos);
	}

	public void FocusPlayer(Vector2Int tilePos, float tileSize)
	{
		Vector3 target = new Vector3(tilePos.x * tileSize, tilePos.y * tileSize, cam.transform.position.z);
		auto.StartAutoMove(target);
	}

	public void SetZoomLevel(int level)
	{
		zoom.SetZoomIndex(level);
	}

	public int GetZoomLevel() => zoom.ZoomIndex;
}
