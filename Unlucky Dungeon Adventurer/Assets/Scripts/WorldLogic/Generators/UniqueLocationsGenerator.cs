/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Generators/UniqueLocationsGenerator.cs   */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 14:34:51 by UDA                                      */
/*   Updated: 2025/12/02 14:34:51 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using UnityEngine;

namespace WorldLogic
{
    public class UniqueLocationsGenerator : IGenerator
    {
        private static readonly int TARGET_COUNT = 50;

        // Здесь будет храниться всё, что сгенерировали
        public static List<UniqueLocationInstance> Instances { get; private set; }

        // Вспомогательный список дефов
        private List<UniqueLocationDef> defs;

        public void Generate(int seed, WorldGenerator worldGen)
        {
            Random.InitState(seed + 777777); // смещение, чтобы не совпадало с биомами

            LoadAllDefs();
            PrepareDefOrder();
            GenerateInstances(worldGen);

            Debug.Log($"[UniqueLocationsGenerator] Generated {Instances.Count} unique locations.");
        }

        // ============================================================
        // 1) Загрузка всех дефов из Resources
        // ============================================================

        private void LoadAllDefs()
        {
            defs = new List<UniqueLocationDef>(
                Resources.LoadAll<UniqueLocationDef>("WorldData/UniqueLocations")
            );

            if (defs.Count == 0)
                Debug.LogError("[UniqueLocationsGenerator] No UniqueLocationDef assets found!");
        }

        // ============================================================
        // 2) Сортировка по rarity (вес распределения)
        // чтобы более редкие появлялись реже
        // ============================================================

        private void PrepareDefOrder()
        {
            defs.Sort((a, b) => b.rarity.CompareTo(a.rarity));
        }

        // ============================================================
        // 3) Генерация 50 локаций
        // ============================================================

        private void GenerateInstances(WorldGenerator worldGen)
        {
            Instances = new List<UniqueLocationInstance>();

            int generatedCount = 0;
            int defsIndex = 0;

            while (generatedCount < TARGET_COUNT)
            {
                if (defsIndex >= defs.Count)
                {
                    Debug.LogWarning("[UniqueLocationsGenerator] Not enough defs, repeating list.");
                    defsIndex = 0;
                }

                var def = defs[defsIndex];
                defsIndex++;

                WorldTilePos pos = FindValidTileFor(worldGen, def);

                var state = new UniqueLocationState(def.id, pos);
                var instance = new UniqueLocationInstance(def, state);

                Instances.Add(instance);
                generatedCount++;
            }
        }

        // ============================================================
        // 4) Подбор корректного тайла под локацию
        // ============================================================

        private WorldTilePos FindValidTileFor(WorldGenerator gen, UniqueLocationDef def)
        {
            // Сильно ограничивать не будем – мир большой, найдём
            for (int i = 0; i < def.spawnAttempts; i++)
            {
                int x = Random.Range(0, 200);
                int y = Random.Range(0, 200);

                var tile = gen.GetTile(x, y);
                if (tile == null)
                    continue;

                // 1) Проверка биома
                if (!string.IsNullOrEmpty(def.requiredBiome))
                {
                    if (tile.biomeId != def.requiredBiome)
                        continue;
                }

                // 2) Доп. правила
                if (def.nearMountains && !IsNearBiome(gen, x, y, "mountain")) continue;
                if (def.nearWater     && !IsNearBiome(gen, x, y, "water")) continue;

                // 3) Можно добавить проверку "farFromCities", когда будут города
                // 4) TODO: проверка минимальной дистанции между другими уникалками

                return new WorldTilePos(x, y);
            }

            Debug.LogWarning($"[UniqueLocationsGenerator] FAILED to place {def.id}, using fallback.");
            return new WorldTilePos(100, 100); // безопасный центр
        }

        // ============================================================
        // 5) Вспомогательная проверка биома вокруг
        // ============================================================

        private bool IsNearBiome(WorldGenerator gen, int x, int y, string biome)
        {
            for (int dx = -defSpawnCheckRange; dx <= defSpawnCheckRange; dx++)
                for (int dy = -defSpawnCheckRange; dy <= defSpawnCheckRange; dy++)
                {
                    var t = gen.GetTile(x + dx, y + dy);
                    if (t != null && t.biomeId == biome)
                        return true;
                }

            return false;
        }

        private const int defSpawnCheckRange = 2;
    }
}

