/* ************************************************************************** */
/*                                                                            */
/*   File: CityGenerator.cs                               /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/11                                                     */
/*   Updated: 2025/12/11                                                     */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using UnityEngine;
using WorldLogic;
using WorldLogic.Cities;

namespace WorldLogic.Generators
{
    /// <summary>
    /// Детерминированный генератор городов.
    /// Использует seed для получения одинаковых координат
    /// при каждом входе в мир.
    /// </summary>
    public class CityGenerator : IGenerator
    {
        private const int TARGET_CITY_COUNT = 8;   // количество городов
        private const int MIN_CITY_DISTANCE = 20;  // дистанция между городами
        private const int MIN_DISTANCE_FROM_UNIQUES = 35;
        private const int MAX_PLACEMENT_ATTEMPTS = 2000;
        private const int MAP_SIZE = 200;

        private List<CityDef> defs;
        public static List<CityState> GeneratedStates { get; private set; }

        public void Generate(int seed, WorldGenerator worldGen)
        {
            Debug.Log($"[CityGenerator] Starting city generation, seed={seed}");

            LoadDefs();
            GeneratedStates = new List<CityState>();

            if (defs.Count == 0)
            {
                Debug.LogWarning("[CityGenerator] No CityDef assets found!");
                return;
            }

            GenerateCities(seed, worldGen);

            Debug.Log($"[CityGenerator] DONE: generated {GeneratedStates.Count} cities.");
        }

        // ============================
        // 1. Load defs
        // ============================
        private void LoadDefs()
        {
            defs = new List<CityDef>(
                Resources.LoadAll<CityDef>("WorldData/Cities")
            );

            if (defs.Count == 0)
                Debug.LogWarning("[CityGenerator] No city defs found in Resources/WorldData/Cities.");
        }

        // ============================
        // 2. Main generation loop
        // ============================
        private void GenerateCities(int seed, WorldGenerator gen)
        {
            int defIndex = 0;

            foreach (var def in defs)
            {
                if (GeneratedStates.Count >= TARGET_CITY_COUNT)
                    break;

                WorldTilePos pos = FindValidCityTile(def, gen, seed, defIndex);
                defIndex++;

                // Детерминированное население
                int population = def.minPopulation + (DeterministicHash.Hash(seed + defIndex * 31, defIndex) % 
                    (def.maxPopulation - def.minPopulation + 1));

                CityState st = new CityState
                {
                    id = def.id,
                    position = pos,
                    currentPopulation = population,
                    factionId = def.factionId,
                    discovered = false,
                    lastVisitedDay = -1,
                    additionalData = ""
                };

                GeneratedStates.Add(st);

                Debug.Log($"[CityGenerator] City '{def.id}' placed at {pos.X},{pos.Y}");
            }
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

            // Fallback: используем центр карты
            Debug.LogWarning($"[CityGenerator] Fallback for '{def.id}' using center ({MAP_SIZE/2},{MAP_SIZE/2})");
            return new WorldTilePos(MAP_SIZE / 2, MAP_SIZE / 2);
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

        // Детерминированный выбор координаты
        private WorldTilePos GetDeterministicCoord(int seed, int salt)
        {
            int x = DeterministicHash.Hash(seed, salt * 41) % MAP_SIZE;
            int y = DeterministicHash.Hash(seed, salt * 113) % MAP_SIZE;
            return new WorldTilePos(x, y);
        }
    }
}
