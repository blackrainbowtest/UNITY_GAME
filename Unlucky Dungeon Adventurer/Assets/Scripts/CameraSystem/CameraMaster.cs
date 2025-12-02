/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/CameraSystem/CameraMaster.cs                        */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 10:40:32 by UDA                                      */
/*   Updated: 2025/12/02 10:40:32 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

/// <summary>
/// Central camera manager.
/// Responsibilities:
/// - Initialize camera subsystems
/// - Dispatch Update()
/// - Provide public API for other systems
/// </summary>
public class CameraMaster : MonoBehaviour
{
	public static CameraMaster Instance;

	private CameraPanController pan;
	private CameraAutoCenter autoCenter;
	private CameraZoomModule zoom;

	private Camera cam;

	[Header("Autocenter Settings")]
	public float autoCenterSpeed = 12f;				// camera flight speed
	public float autoCenterStopRadius = 0.05f;

	[Header("Zoom Settings")]
	public float minZoom = 4f;
	public float maxZoom = 22f;
	public float zoomSmooth = 0.15f;

	private void Awake()
	{
		Instance = this;
		cam = Camera.main;

		// Create subsystems
		pan = new CameraPanController(cam);
		autoCenter = new CameraAutoCenter(cam, autoCenterSpeed, autoCenterStopRadius);

		zoom = new CameraZoomModule(
			cam,
			minZoom,
			maxZoom,
			zoomSmooth
		);
	}

	private void Update()
	{
		// Handle input and set targets, but don't move camera yet
		if (autoCenter.IsActive)
		{
			// Auto-center runs in Update to capture target
			return;
		}

		pan.Update();		// Just processes input, doesn't move camera directly
	}

	private void LateUpdate()
	{
		// Apply camera updates AFTER all other game logic
		if (autoCenter.IsActive)
		{
			autoCenter.UpdateAutoCenter();
		}

		// Always update zoom in LateUpdate for smooth rendering
		zoom.UpdateZoom();
	}

	// ================================================================
	//                           PUBLIC API
	// ================================================================

	public void CenterToWorldPos(Vector3 worldPos)
	{
		autoCenter.StartAutoCenter(worldPos);
		pan.CancelInertia();
	}

	public void CenterToPlayer()
	{
		var p = GameData.CurrentPlayer;
		if (p == null) return;

		// Find the player GameObject to get its actual world position
		var playerObj = GameObject.FindGameObjectWithTag("Player");
		if (playerObj != null)
		{
			Vector3 target = new Vector3(
				playerObj.transform.position.x,
				playerObj.transform.position.y,
				cam.transform.position.z
			);
			CenterToWorldPos(target);
		}
		else
		{
			// Fallback: assume tileSize of 10 (standard in this project)
			const float tileSize = 10f;
			Vector3 target = new Vector3(
				p.mapPosX * tileSize,
				p.mapPosY * tileSize,
				cam.transform.position.z
			);
			CenterToWorldPos(target);
		}
	}

	public void CenterToTile(Vector2Int tile)
	{
		Vector3 pos = new Vector3(tile.x, tile.y, cam.transform.position.z);
		CenterToWorldPos(pos);
	}

	public void DisablePan()
	{
		pan.CancelInertia();
		enabled = false;
	}

	public void EnablePan()
	{
		enabled = true;
	}

	// === Optional public zoom control for UI buttons ===
	public void ZoomIn(float amount = 1f)
	{
		zoom.ZoomIn(amount);
	}

	public void ZoomOut(float amount = 1f)
	{
		zoom.ZoomOut(amount);
	}

	public float GetCurrentZoom() => zoom.GetZoom();
}
