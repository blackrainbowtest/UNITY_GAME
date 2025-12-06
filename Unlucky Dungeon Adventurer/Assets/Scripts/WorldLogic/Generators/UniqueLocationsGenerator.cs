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

        // Р’СЃРµ 50 СѓРЅРёРєР°Р»СЊРЅС‹С… Р»РѕРєР°С†РёР№ РїРѕСЃР»Рµ РіРµРЅРµСЂР°С†РёРё
        public static List<UniqueLocationInstance> Instances { get; private set; }

        // Р’СЃРµ РґРµС„С‹
        private List<UniqueLocationDef> defs;

        public void Generate(int seed, WorldGenerator worldGen)
        {
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
        // 4) РџРѕРёСЃРє РїРѕРґС…РѕРґСЏС‰РµРіРѕ С‚Р°Р№Р»Р°
        // ============================================================

        private WorldTilePos FindValidTileFor(
            UniqueLocationDef def,
            WorldGenerator worldGen,
            int seed,
            ref int index)
        {
            while (true)
            {
                // Р“РµРЅРµСЂРёСЂСѓРµРј РґРµС‚РµСЂРјРёРЅРёСЂРѕРІР°РЅРЅСѓСЋ РєРѕРѕСЂРґРёРЅР°С‚Сѓ
                WorldTilePos pos = GetDeterministicCoord(seed, index);
                index++;

                if (TileIsValid(def, worldGen, pos))
                    return pos;

                // fallback: СЃРјРѕС‚СЂРёРј РѕРєСЂРµСЃС‚РЅРѕСЃС‚СЊ
                WorldTilePos? near = TryFindNearbyValidTile(def, worldGen, pos);
                if (near.HasValue)
                    return near.Value;

                // РёРЅР°С‡Рµ РїСЂРѕРґРѕР»Р¶Р°РµРј РїРѕРёСЃРє
            }
        }

        // РџСЂРѕРІРµСЂРєР°, РїРѕРґС…РѕРґРёС‚ Р»Рё С‚Р°Р№Р»
        private bool TileIsValid(UniqueLocationDef def, WorldGenerator gen, WorldTilePos pos)
        {
            var tile = gen.GetTile(pos.X, pos.Y);
            if (tile == null)
                return false;

            // Р‘Р°Р·РѕРІС‹Р№ Р±РёРѕРј
            if (!string.IsNullOrEmpty(def.requiredBiome))
                if (tile.biomeId != def.requiredBiome)
                    return false;

            // Р”РѕРї. СѓСЃР»РѕРІРёСЏ
            if (def.nearMountains && !IsNearBiome(gen, pos, "mountain"))
                return false;

            if (def.nearWater && !IsNearBiome(gen, pos, "water"))
                return false;

            return true;
        }

        // РџРѕРёСЃРє РїРѕРґС…РѕРґСЏС‰РµРіРѕ С‚Р°Р№Р»Р° РїРѕР±Р»РёР·РѕСЃС‚Рё
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
            int x = DeterministicHash.Hash(seed, index * 17) % 200;
            int y = DeterministicHash.Hash(seed, index * 31) % 200;

            return new WorldTilePos(x, y);
        }

        private const int defSpawnCheckRange = 2;
    }
}


