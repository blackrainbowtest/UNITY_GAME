/* ************************************************************************** */
/*                                                                            */
/*   File: CityWorldRenderer.cs                           /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/13                                                     */
/*   Updated: 2025/12/13                                                     */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using UnityEngine;
using WorldLogic.Cities;
using WorldLogic;

/// <summary>
/// Рендер городов как оверлей к спаунящимся тайлам (аналогично уникальным локациям).
/// Маркеры вешаются на объект тайла, чтобы совпадали позиция/масштаб/слои.
/// </summary>
public class CityWorldRenderer : MonoBehaviour
{
    public static CityWorldRenderer Instance { get; private set; }

    [SerializeField] private GameObject cityMarkerPrefab;

    private CityManager cityManager;
    private WorldGenerator worldGenerator;
    private int worldSeed;

    private readonly System.Collections.Generic.Dictionary<WorldTilePos, GameObject> activeMarkers = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Initialize(CityManager manager, WorldGenerator gen, int seed)
    {
        cityManager = manager;
        worldGenerator = gen;
        worldSeed = seed;

        // Fallback на ресурс, если префаб не задан в инспекторе
        if (cityMarkerPrefab == null)
            cityMarkerPrefab = Resources.Load<GameObject>("Prefabs/World/CityMarker");

        if (cityMarkerPrefab == null)
        {
            Debug.LogError("[CityWorldRenderer] CityMarker prefab not assigned and not found in Resources");
        }
    }

    /// <summary>
    /// Вызывается из WorldMapController при спауне тайла.
    /// </summary>
    public void OnTileSpawned(WorldTilePos pos, GameObject tileObj)
    {
        if (cityManager == null || tileObj == null)
            return;

        // Уже отрисовано
        if (activeMarkers.ContainsKey(pos))
            return;

        // Есть ли город на этом тайле
        var city = cityManager.GetCityAt(pos.ToVector2Int());
        if (city == null)
            return;

        if (cityMarkerPrefab == null)
        {
            Debug.LogWarning("[CityWorldRenderer] cityMarkerPrefab is missing");
            return;
        }

        var marker = Instantiate(cityMarkerPrefab, tileObj.transform);
        marker.name = $"City_{city.displayName}";
        marker.transform.localPosition = new Vector3(0, 0, -0.05f);
        marker.transform.localScale = Vector3.one; // наследует масштаб тайла от родителя

        var sr = marker.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = PickCityIcon(city, worldGenerator);
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 110; // чуть выше тайла/уникалок
        }

        activeMarkers[pos] = marker;
    }

    /// <summary>
    /// Снять маркер при удалении тайла.
    /// </summary>
    public void OnTileDespawned(WorldTilePos pos)
    {
        if (activeMarkers.TryGetValue(pos, out var marker))
        {
            Destroy(marker);
            activeMarkers.Remove(pos);
        }
    }

    private Sprite PickCityIcon(CityInstance city, WorldGenerator gen)
    {
        // Предпочитаем иконку из дефиниции
        if (city.def != null && city.def.icon != null)
            return city.def.icon;

        var defaultSprite = Resources.Load<Sprite>("WorldData/Cities/city_default");
        return defaultSprite;
    }
}
