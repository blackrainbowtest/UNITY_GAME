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

using UnityEngine;
using WorldLogic.Cities;
using WorldLogic;

/// <summary>
/// Рендерит маркеры городов на глобальной карте.
/// Не трогает генерацию/сейвы, только визуализация.
/// </summary>
public class CityWorldRenderer : MonoBehaviour
{
    [SerializeField] private GameObject cityMarkerPrefab;

    private CityManager cityManager;
    private WorldGenerator worldGenerator;
    private int worldSeed;

    private void Start()
    {
        cityManager = FindFirstObjectByType<CityManager>();
        worldGenerator = FindFirstObjectByType<WorldMapController>()?.GetWorldGenerator();
        var director = FindFirstObjectByType<WorldLogicDirector>();
        worldSeed = director != null ? director.WorldSeed : 0;

        if (cityManager == null || worldGenerator == null || cityMarkerPrefab == null)
        {
            Debug.LogError("[CityWorldRenderer] Missing references (CityManager/WorldGenerator/Prefab)");
            return;
        }

        RenderCities();
    }

    private void RenderCities()
    {
        foreach (var city in cityManager.GetAllCities())
        {
            Vector2Int tilePos = city.state.position.ToVector2Int();

            // Допущение: 1 юнит = 1 тайл. При другой сетке заменить конвертацию.
            Vector3 worldPos = new Vector3(tilePos.x, tilePos.y, 0f);

            var go = Instantiate(cityMarkerPrefab, worldPos, Quaternion.identity, transform);

            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = PickCityIcon(city, worldGenerator);
            }
        }
    }

    private Sprite PickCityIcon(CityInstance city, WorldGenerator gen)
    {
        var tile = gen.GetTile(city.state.position.X, city.state.position.Y);
        string biomeId = tile?.biomeId ?? "unknown";

        var sprites = Resources.LoadAll<Sprite>($"WorldUI/CityIcons/city_{biomeId}");
        if (sprites != null && sprites.Length > 0)
        {
            int idx = Mathf.Abs(DeterministicHash.Hash(worldSeed, city.state.generationIndex * 101 + biomeId.GetHashCode())) % sprites.Length;
            return sprites[idx];
        }

        return Resources.Load<Sprite>("WorldUI/CityIcons/city_default");
    }
}
