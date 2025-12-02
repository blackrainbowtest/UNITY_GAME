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
/// The camera's central control module.
/// What happens here:
/// - Initialization of all camera subsystems.
/// - Update handling.
/// - Public methods for other parts of the game.
/// </summary>
public class CameraMaster : MonoBehaviour
{
	public static CameraMaster Instance;

	private CameraPanController pan;
	private CameraAutoCenter autoCenter;

	private Camera cam;

	[Header("Autocenter Settings")]
	public float autoCenterSpeed = 12f;				// camera flight speed
	public float autoCenterStopRadius = 0.05f;

	private void Awake()
	{
		Instance = this;
		cam = Camera.main;

		// Create subsystems
		pan = new CameraPanController(cam);
		autoCenter = new CameraAutoCenter(cam, autoCenterSpeed, autoCenterStopRadius);
	}

	private void Update()
	{
		// If auto-centering is active, pan is disabled
		if (autoCenter.IsActive)
		{
			autoCenter.UpdateAutoCenter();
			return;
		}

		pan.Update();								// normal control
	}

	// ================================================================
	//                           PUBLIC API
	// ================================================================

	/// <summary>
	/// Smoothly centers the camera on the tile (coordinates in world units).
	/// </summary>
	public void CenterToWorldPos(Vector3 worldPos)
	{
		autoCenter.StartAutoCenter(worldPos);
		pan.CancelInertia();
	}

	/// <summary>
	/// Centering the camera on the player
	/// </summary>
	public void CenterToPlayer()
	{
		var p = GameData.CurrentPlayer;
		if (p == null) return;

		Vector3 target = new Vector3(
			Mathf.Round(p.mapPosX),
			Mathf.Round(p.mapPosY),
			cam.transform.position.z
		);

		CenterToWorldPos(target);
	}

	/// <summary>
	/// Centering the camera on a tile coordinate
	/// </summary>
	public void CenterToTile(Vector2Int tile)
	{
		Vector3 pos = new Vector3(tile.x, tile.y, cam.transform.position.z);
		CenterToWorldPos(pos);
	}

	/// <summary>
	/// Completely disables manual camera control
	/// (for example, during dialogues or cutscenes)
	/// </summary>
	public void DisablePan()
	{
		pan.CancelInertia();
		enabled = false;
	}

	/// <summary>
	/// Returns camera control to the player
	/// </summary>
	public void EnablePan()
	{
		enabled = true;
	}
}
