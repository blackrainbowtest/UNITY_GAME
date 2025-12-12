/* ************************************************************************** */
/*                                                                            */
/*   File: CityMinimapRenderer.cs                         /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/13                                                     */
/*   Updated: 2025/12/13                                                     */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using WorldLogic.Cities;

/// <summary>
/// Отрисовщик городов на миникарте.
/// Подписывается на OnMinimapUpdated и рисует маркеры городов.
/// </summary>
public class CityMinimapRenderer : MonoBehaviour
{
    private CityManager cityManager;
    private MinimapController minimap;

    private void Start()
    {
        cityManager = FindFirstObjectByType<CityManager>();
        minimap = FindFirstObjectByType<MinimapController>();

        if (cityManager == null || minimap == null)
        {
            Debug.LogError("[CityMinimapRenderer] CityManager or MinimapController not found");
            return;
        }

        // Рисуем сразу один раз
        RenderAllCities();

        // Подписываемся ТОЛЬКО на обновления миникарты
        minimap.OnMinimapUpdated += RenderAllCities;
    }

    private void OnDestroy()
    {
        if (minimap != null)
            minimap.OnMinimapUpdated -= RenderAllCities;
    }

    private void RenderAllCities()
    {
        if (cityManager == null)
            return;

        foreach (var city in cityManager.GetAllCities())
        {
            Vector2Int pos = city.state.position.ToVector2Int();

            // Временный цвет города (жёлтый)
            Color color = Color.yellow;

            minimap.DrawUniqueMarker(pos, color);
        }
    }
}
