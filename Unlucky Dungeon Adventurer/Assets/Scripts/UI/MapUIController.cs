using UnityEngine;

public class MapUIController : MonoBehaviour
{
    public void OnCenterCamera()
    {
        if (GameData.CurrentPlayer == null) return;

        Vector3 playerWorld = new Vector3(
            GameData.CurrentPlayer.mapPosX,
            GameData.CurrentPlayer.mapPosY,
            Camera.main.transform.position.z
        );

        // Use new camera system API
        if (CameraMaster.Instance != null)
        {
            CameraMaster.Instance.CenterToPlayer();
        }
        else
        {
            // Fallback: directly position camera
            Camera.main.transform.position = playerWorld;
        }
    }
}
