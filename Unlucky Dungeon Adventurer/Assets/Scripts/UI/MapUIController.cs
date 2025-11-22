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

        CameraPan cp = Camera.main.GetComponent<CameraPan>();
        if (cp != null)
            cp.CenterToPlayer(playerWorld);
    }
}
