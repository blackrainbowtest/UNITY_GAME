/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Generators/UniqueLocationsGenerator.cs   */
/*                                                        /\_/\               */
/*                                                       ( вЂў.вЂў )              */
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
        private const int MAP_HALF_SIZE = 100; // symmetric range [-100, +100]

        // Р’СЃРµ 50 СѓРЅРёРєР°Р»СЊРЅС‹С… Р»РѕРєР°С†РёР№ РїРѕСЃР»Рµ РіРµРЅРµСЂР°С†РёРё
        public static List<UniqueLocationInstance> Instances { get; private set; }

        // Р’СЃРµ РґРµС„С‹
        private List<UniqueLocationDef> defs;

        public void Generate(int seed, WorldGenerator worldGen)
        {
            Debug.Log("[UniqueLocationsGenerator] UNIQUE GEN CALLED");
            LoadAllDefs();
            PrepareDefOrder();
            GenerateInstances(worldGen, seed);

            UDADebug.Log($"[UniqueLocationsGenerator] Generated {Instances.Count} unique locations (deterministic).");
        }

        // ============================================================
        // 1) Р—Р°РіСЂСѓР¶Р°РµРј РґРµС„С‹ РёР· Resources
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
        // 2) РЎРѕСЂС‚РёСЂРѕРІРєР° РґРµС„РѕРІ РїРѕ rarity
        // ============================================================

        private void PrepareDefOrder()
        {
            defs.Sort((a, b) => b.rarity.CompareTo(a.rarity));
        }

        // ============================================================
        // 3) Р“РµРЅРµСЂР°С†РёСЏ 50 Р»РѕРєР°С†РёР№ (РґРµС‚РµСЂРјРёРЅРёСЂРѕРІР°РЅРЅРѕ)
        // ============================================================

        private void GenerateInstances(WorldGenerator worldGen, int seed)
        {
            Instances = new List<UniqueLocationInstance>();
            var occupiedPositions = new HashSet<(int, int)>();

            int defIndex = 0;
            foreach (var def in defs)
            {
                WorldTilePos pos = FindValidTileFor(def, worldGen, seed, occupiedPositions);
                
                // Проверяем валидность (не fallback -1,-1)
                // Accept symmetric coordinates; only skip clearly invalid sentinel values
                if (pos.X != int.MinValue && pos.Y != int.MinValue && !occupiedPositions.Contains((pos.X, pos.Y)))
                {
                    occupiedPositions.Add((pos.X, pos.Y));

                    var state = new UniqueLocationState(def.id, pos);
                    var instance = new UniqueLocationInstance(def, state);
                    Instances.Add(instance);
                }

                defIndex++;
                
                if (Instances.Count >= TARGET_COUNT)
                    break;
            }
        }

        // ============================================================
        // 4) РџРѕРёСЃРє РїРѕРґС…РѕРґСЏС‰РµРіРѕ С‚Р°Р№Р»Р°
        // ============================================================

        private WorldTilePos FindValidTileFor(
            UniqueLocationDef def,
            WorldGenerator worldGen,
            int seed,
            HashSet<(int, int)> occupiedPositions)
        {
            const int MAX_CANDIDATES = 50;
            
            // РџРµСЂРµР±РёСЂР°РµРј РІСЃРµ 50 РґРµС‚РµСЂРјРёРЅРёСЂРѕРІР°РЅРЅС‹С… РєР°РЅРґРёРґР°С‚РѕРІ
            for (int candidateIndex = 0; candidateIndex < MAX_CANDIDATES; candidateIndex++)
            {
                WorldTilePos pos = GetDeterministicCoord(seed, candidateIndex);

                // РџСЂРѕРїСѓСЃРєР°РµРј СѓР¶Рµ Р·Р°РЅСЏС‚С‹Рµ РїРѕР·РёС†РёРё
                if (occupiedPositions.Contains((pos.X, pos.Y)))
                    continue;

                if (TileIsValid(def, worldGen, pos))
                {
                    return pos;
                }

                // Fallback: смотрим окрестность (чтобы не пропустить хорошую позицию)
                WorldTilePos? near = TryFindNearbyValidTile(def, worldGen, pos, occupiedPositions);
                if (near.HasValue)
                {
                    return near.Value;
                }
            }

            // Fallback: ни один из 50 кандидатов не подошёл
            return new WorldTilePos(int.MinValue, int.MinValue);
        }

        // Проверка, подходит ли тайл
        private bool TileIsValid(UniqueLocationDef def, WorldGenerator gen, WorldTilePos pos)
        {
            var tile = gen.GetTile(pos.X, pos.Y);
            if (tile == null)
                return false;

            // ОТКЛЮЧЕНА: Проверка по биому
            // if (!string.IsNullOrEmpty(def.requiredBiome))
            // {
            //     if (string.IsNullOrEmpty(tile.biomeId))
            //     {
            //         Debug.LogWarning($"[UniqueLocationsGenerator] Tile at {pos.X},{pos.Y} has null/empty biomeId!");
            //         return false;
            //     }
            //     
            //     if (!tile.biomeId.Contains(def.requiredBiome))
            //     {
            //         return false;
            //     }
            // }

            // ОТКЛЮЧЕНЫ: Доп. условия по биомам
            // if (def.nearMountains && !IsNearBiome(gen, pos, "mountain"))
            //     return false;
            //
            // if (def.nearWater && !IsNearBiome(gen, pos, "water"))
            //     return false;

            // Все тайлы валидны (пока)
            return true;
        }

        private WorldTilePos? TryFindNearbyValidTile(
            UniqueLocationDef def,
            WorldGenerator gen,
            WorldTilePos pos,
            HashSet<(int, int)> occupiedPositions)
        {
            const int R = 3;

            for (int dx = -R; dx <= R; dx++)
                for (int dy = -R; dy <= R; dy++)
                {
                    var p = new WorldTilePos(pos.X + dx, pos.Y + dy);
                    
                    // Не размещаем на занятых позициях
                    if (occupiedPositions.Contains((p.X, p.Y)))
                        continue;
                    
                    if (TileIsValid(def, gen, p))
                        return p;
                }

            return null;
        }

        // РџСЂРѕРІРµСЂРєР° Р±РёРѕРјР° РІРѕРєСЂСѓРі (РґР»СЏ nearMountains Рё nearWater)
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
        // 5) Minecraft-style РґРµС‚РµСЂРјРёРЅРёСЂРѕРІР°РЅРЅР°СЏ РєРѕРѕСЂРґРёРЅР°С‚Р°
        // ============================================================

        private WorldTilePos GetDeterministicCoord(int seed, int index)
        {
            int range = MAP_HALF_SIZE * 2 + 1; // 201

            // Keep distribution symmetric around zero to avoid +X/+Y bias
            int x = (DeterministicHash.Hash(seed, index * 17) & 0x7FFFFFFF) % range - MAP_HALF_SIZE;
            int y = (DeterministicHash.Hash(seed, index * 31) & 0x7FFFFFFF) % range - MAP_HALF_SIZE;

            return new WorldTilePos(x, y);
        }

        private const int defSpawnCheckRange = 2;
    }
}


