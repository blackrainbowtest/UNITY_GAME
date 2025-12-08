using UnityEngine;
using System.Collections;

public class MapUIController : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private float teleportDistanceThreshold = 50f; // tiles
    [SerializeField] private GameObject loadingOverlay; // Optional: UI overlay for fade

    private bool isCentering = false; // Prevent multiple simultaneous center commands

    public void OnCenterCamera()
    {
        if (GameData.CurrentPlayer == null) return;
        if (CameraMaster.Instance == null) return;
        if (isCentering) return; // Ignore if already centering

        Vector2Int playerTilePos = new Vector2Int(
            Mathf.RoundToInt(GameData.CurrentPlayer.mapPosX),
            Mathf.RoundToInt(GameData.CurrentPlayer.mapPosY)
        );

        Vector2Int cameraTilePos = new Vector2Int(
            Mathf.RoundToInt(Camera.main.transform.position.x / WorldMapController.Instance.tileSize),
            Mathf.RoundToInt(Camera.main.transform.position.y / WorldMapController.Instance.tileSize)
        );

        float distance = Vector2Int.Distance(playerTilePos, cameraTilePos);

        if (distance > teleportDistanceThreshold)
        {
            // Far away: instant teleport with loading screen
            StartCoroutine(TeleportWithLoading());
        }
        else
        {
            // Close: smooth movement
            isCentering = true;
            CameraMaster.Instance.CenterToPlayer();
            
            // Reset flag after animation completes (adjust time based on your autoCenter duration)
            StartCoroutine(ResetCenteringAfterDelay(0.5f));
        }
    }

    private IEnumerator TeleportWithLoading()
    {
        isCentering = true;
        
        // Show loading overlay (if assigned)
        if (loadingOverlay != null)
            loadingOverlay.SetActive(true);

        // Teleport camera instantly
        CameraMaster.Instance.TeleportToPlayer();

        // Wait until all rendering complete (Update + LateUpdate finished)
        yield return new WaitForEndOfFrame();

        // Hide loading overlay
        if (loadingOverlay != null)
            loadingOverlay.SetActive(false);

        isCentering = false;
    }

    private IEnumerator ResetCenteringAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isCentering = false;
    }
}
