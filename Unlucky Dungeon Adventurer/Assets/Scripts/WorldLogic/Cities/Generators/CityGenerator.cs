using System.Collections.Generic;
using UnityEngine;
using WorldLogic;
using WorldLogic.Cities;

namespace WorldLogic.Generators
{
    public class CityGenerator : IGenerator
    {
        private const int TARGET_CITY_COUNT = 8;   // количество городов
        private const int MIN_CITY_DISTANCE = 10;  // дистанция между городами (уменьшено для тестов)
        private const int MIN_DISTANCE_FROM_UNIQUES = 15; // уменьшено для тестов
        private const int MAX_PLACEMENT_ATTEMPTS = 2000;
        // Диапазон карты: от -MAP_SIZE до +MAP_SIZE (включительно)
        private const int MAP_SIZE = 100; // полуразмер карты

        private List<CityDef> defs;
        public static List<CityState> GeneratedStates { get; private set; }

        public void Generate(int seed, WorldGenerator worldGen)
        {
            LoadDefs();
            GeneratedStates = new List<CityState>();

            if (defs.Count == 0)
                return;

            GenerateCities(seed, worldGen);
        }

        // ============================
        // 1. Load defs
        // ============================
        private void LoadDefs()
        {
            defs = new List<CityDef>(
                Resources.LoadAll<CityDef>("WorldData/Cities")
            );
        }

        // ============================
        // 2. Main generation loop
        // ============================
        private void GenerateCities(int seed, WorldGenerator gen)
        {
            if (defs == null || defs.Count == 0)
                return;

            for (int cityIndex = 0; cityIndex < TARGET_CITY_COUNT; cityIndex++)
            {
                var def = defs[cityIndex % defs.Count];

                WorldTilePos pos = FindValidCityTile(def, gen, seed, cityIndex);
                
                // Пропускаем невалидные позиции (если город не смог разместиться)
                if (pos.X == int.MinValue || pos.Y == int.MinValue)
                {
                    Debug.LogWarning($"[CityGenerator] Skipping city {cityIndex} - no valid position found");
                    continue;
                }

                // Получаем биом из реальной позиции
                var tile = gen.GetTile(pos.X, pos.Y);
                string biomeId = tile?.biomeId ?? "unknown";

                // Детерминированное население
                int population = def.minPopulation + (DeterministicHash.Hash(seed + cityIndex * 31, cityIndex) %
                    (def.maxPopulation - def.minPopulation + 1));

                // Генерируем детерминированный ID города (вместо статичного из CityDef)
                string cityId = $"city_{cityIndex}_{biomeId}";

                CityState st = new CityState
                {
                    id = cityId,
                    position = pos,
                    generationIndex = cityIndex,
                    currentPopulation = population,
                    factionId = def.factionId,
                    discovered = false,
                    lastVisitedDay = -1,
                    additionalData = ""
                };

                GeneratedStates.Add(st);
            }
            
            Debug.Log($"[CityGenerator] Successfully generated {GeneratedStates.Count} cities out of {TARGET_CITY_COUNT} attempts");
        }

        // ============================
        // 3. City placement logic
        // ============================
        private WorldTilePos FindValidCityTile(CityDef def, WorldGenerator gen, int seed, int index)
        {
            for (int attempt = 0; attempt < MAX_PLACEMENT_ATTEMPTS; attempt++)
            {
                WorldTilePos coord = GetDeterministicCoord(seed, index + attempt * 17);

                var tile = gen.GetTile(coord.X, coord.Y);
                if (tile == null)
                    continue;

                // 1) biome check
                if (!IsAllowedBiome(def, tile.biomeId))
                    continue;

                // 2) distance to existing cities
                if (!IsFarFromCities(coord))
                    continue;

                // 3) distance to unique locations
                if (!IsFarFromUniqueLocations(coord))
                    continue;

                return coord;
            }

            // Fallback: пытаемся найти любую свободную позицию вблизи центра
            Debug.LogWarning($"[CityGenerator] City {index} failed validation after {MAX_PLACEMENT_ATTEMPTS} attempts. Using fallback search.");
            
            // Ищем свободное место по спирали от центра
            for (int radius = 0; radius < 50; radius++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        WorldTilePos coord = new WorldTilePos(dx, dy);
                        
                        var tile = gen.GetTile(coord.X, coord.Y);
                        if (tile == null) continue;
                        
                        if (!IsAllowedBiome(def, tile.biomeId)) continue;
                        if (!IsFarFromCities(coord)) continue;
                        if (!IsFarFromUniqueLocations(coord)) continue;
                        
                        Debug.Log($"[CityGenerator] City {index} placed at fallback position ({coord.X}, {coord.Y})");
                        return coord;
                    }
                }
            }
            
            // Крайний случай: если даже спираль не помогла
            Debug.LogError($"[CityGenerator] City {index} could not find any valid position! Skipping.");
            return new WorldTilePos(int.MinValue, int.MinValue); // Невалидная позиция для фильтрации
        }

        private bool IsAllowedBiome(CityDef def, string biome)
        {
            foreach (var b in def.allowedBiomes)
            {
                if (biome.Contains(b))
                    return true;
            }
            return false;
        }

        private bool IsFarFromCities(WorldTilePos pos)
        {
            foreach (var st in GeneratedStates)
            {
                float dist = Vector2Int.Distance(
                    new Vector2Int(st.position.X, st.position.Y),
                    new Vector2Int(pos.X, pos.Y)
                );
                
                if (dist < MIN_CITY_DISTANCE)
                    return false;
            }
            return true;
        }

        private bool IsFarFromUniqueLocations(WorldTilePos pos)
        {
            if (UniqueLocationsGenerator.Instances == null || UniqueLocationsGenerator.Instances.Count == 0)
                return true;

            foreach (var ul in UniqueLocationsGenerator.Instances)
            {
                float dist = Vector2Int.Distance(
                    new Vector2Int(ul.State.position.X, ul.State.position.Y),
                    new Vector2Int(pos.X, pos.Y)
                );
                
                if (dist < MIN_DISTANCE_FROM_UNIQUES)
                    return false;
            }
            return true;
        }

        // Детерминированный выбор координаты в симметричном диапазоне [-MAP_SIZE, +MAP_SIZE]
        private WorldTilePos GetDeterministicCoord(int seed, int salt)
        {
            // Получаем значения в диапазоне [0, 2*MAP_SIZE], затем смещаем в [-MAP_SIZE, +MAP_SIZE]
            int range = MAP_SIZE * 2 + 1;
            
            // Используем побитовое AND с 0x7FFFFFFF для получения неотрицательного значения
            // Это более равномерно распределяет значения, чем Mathf.Abs
            int hx = (DeterministicHash.Hash(seed, salt * 41) & 0x7FFFFFFF) % range;
            int hy = (DeterministicHash.Hash(seed, salt * 113) & 0x7FFFFFFF) % range;

            int x = hx - MAP_SIZE;
            int y = hy - MAP_SIZE;

            return new WorldTilePos(x, y);
        }
    }
}
