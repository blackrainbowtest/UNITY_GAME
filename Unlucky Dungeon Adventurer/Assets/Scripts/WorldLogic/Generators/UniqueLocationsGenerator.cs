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

        // Все 50 уникальных локаций после генерации
        public static List<UniqueLocationInstance> Instances { get; private set; }

        // Все дефы
        private List<UniqueLocationDef> defs;

        public void Generate(int seed, WorldGenerator worldGen)
        {
            LoadAllDefs();
            PrepareDefOrder();
            GenerateInstances(worldGen, seed);

            Debug.Log($"[UniqueLocationsGenerator] Generated {Instances.Count} unique locations (deterministic).");
        }

        // ============================================================
        // 1) Загружаем дефы из Resources
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
        // 2) Сортировка дефов по rarity
        // ============================================================

        private void PrepareDefOrder()
        {
            defs.Sort((a, b) => b.rarity.CompareTo(a.rarity));
        }

        // ============================================================
        // 3) Генерация 50 локаций (детерминированно)
        // ============================================================

        private void GenerateInstances(WorldGenerator worldGen, int seed)
        {
            Instances = new List<UniqueLocationInstance>();

            int index = 0;

            foreach (var def in defs)
            {
                WorldTilePos pos = FindValidTileFor(def, worldGen, seed, ref index);

                var state = new UniqueLocationState(def.id, pos);
                var instance = new UniqueLocationInstance(def, state);

                Instances.Add(instance);

                if (Instances.Count >= TARGET_COUNT)
                    break;
            }
        }

        // ============================================================
        // 4) Поиск подходящего тайла
        // ============================================================

        private WorldTilePos FindValidTileFor(
            UniqueLocationDef def,
            WorldGenerator worldGen,
            int seed,
            ref int index)
        {
            while (true)
            {
                // Генерируем детерминированную координату
                WorldTilePos pos = GetDeterministicCoord(seed, index);
                index++;

                if (TileIsValid(def, worldGen, pos))
                    return pos;

                // fallback: смотрим окрестность
                WorldTilePos? near = TryFindNearbyValidTile(def, worldGen, pos);
                if (near.HasValue)
                    return near.Value;

                // иначе продолжаем поиск
            }
        }

        // Проверка, подходит ли тайл
        private bool TileIsValid(UniqueLocationDef def, WorldGenerator gen, WorldTilePos pos)
        {
            var tile = gen.GetTile(pos.X, pos.Y);
            if (tile == null)
                return false;

            // Базовый биом
            if (!string.IsNullOrEmpty(def.requiredBiome))
                if (tile.biomeId != def.requiredBiome)
                    return false;

            // Доп. условия
            if (def.nearMountains && !IsNearBiome(gen, pos, "mountain"))
                return false;

            if (def.nearWater && !IsNearBiome(gen, pos, "water"))
                return false;

            return true;
        }

        // Поиск подходящего тайла поблизости
        private WorldTilePos? TryFindNearbyValidTile(
            UniqueLocationDef def,
            WorldGenerator gen,
            WorldTilePos pos)
        {
            const int R = 3;

            for (int dx = -R; dx <= R; dx++)
                for (int dy = -R; dy <= R; dy++)
                {
                    var p = new WorldTilePos(pos.X + dx, pos.Y + dy);
                    if (TileIsValid(def, gen, p))
                        return p;
                }

            return null;
        }

        // Проверка биома вокруг (для nearMountains и nearWater)
        private bool IsNearBiome(WorldGenerator gen, WorldTilePos pos, string biome)
        {
            for (int dx = -defSpawnCheckRange; dx <= defSpawnCheckRange; dx++)
                for (int dy = -defSpawnCheckRange; dy <= defSpawnCheckRange; dy++)
                {
                    var tile = gen.GetTile(pos.X + dx, pos.Y + dy);
                    if (tile != null && tile.biomeId == biome)
                        return true;
                }

            return false;
        }

        // ============================================================
        // 5) Minecraft-style детерминированная координата
        // ============================================================

        private WorldTilePos GetDeterministicCoord(int seed, int index)
        {
            int x = DeterministicHash.Hash(seed, index * 17) % 200;
            int y = DeterministicHash.Hash(seed, index * 31) % 200;

            return new WorldTilePos(x, y);
        }

        private const int defSpawnCheckRange = 2;
    }
}

