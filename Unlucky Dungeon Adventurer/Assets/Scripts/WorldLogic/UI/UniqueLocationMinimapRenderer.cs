using UnityEngine;
using WorldLogic;

public class UniqueLocationMinimapRenderer : MonoBehaviour
{
    private UniqueLocationManager manager;
    private MinimapController minimap;

    private void Start()
    {
        manager = FindFirstObjectByType<UniqueLocationManager>();
        minimap = FindFirstObjectByType<MinimapController>();

        if (manager == null || minimap == null)
        {
            Debug.LogError("[UL Minimap] Missing manager/minimap");
            return;
        }

        minimap.OnMinimapUpdated += RenderAllMarkers;

        RenderAllMarkers();
    }

    private void OnDestroy()
    {
        if (minimap != null)
            minimap.OnMinimapUpdated -= RenderAllMarkers;
    }

    private void RenderAllMarkers()
    {
        if (manager == null || minimap == null)
            return;

        foreach (var loc in manager.Locations)
        {
            Vector2Int tilePos = loc.State.position.ToVector2Int();

            // пока просто цветной пиксель
            minimap.DrawUniqueMarker(tilePos, Color.magenta);
        }
    }
}
