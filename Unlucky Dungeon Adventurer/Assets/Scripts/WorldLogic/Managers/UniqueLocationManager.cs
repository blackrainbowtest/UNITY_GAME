/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Managers/UniqueLocationManager.cs        */
/*                                                        /\_/\               */
/*                                                       ( вЂў.вЂў )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 14:36:35 by UDA                                      */
/*   Updated: 2025/12/02 14:36:35 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using UnityEngine;

namespace WorldLogic
{
    /// <summary>
    /// Р“Р»Р°РІРЅС‹Р№ РјРµРЅРµРґР¶РµСЂ СѓРЅРёРєР°Р»СЊРЅС‹С… Р»РѕРєР°С†РёР№.
    /// РЈРїСЂР°РІР»СЏРµС‚ РІСЃРµРјРё 50 РёРЅСЃС‚Р°РЅСЃР°РјРё РїРѕСЃР»Рµ РіРµРЅРµСЂР°С†РёРё.
    /// РЎРѕС…СЂР°РЅСЏРµС‚/Р·Р°РіСЂСѓР¶Р°РµС‚ state Рё РїСЂРµРґРѕСЃС‚Р°РІР»СЏРµС‚ API РґР»СЏ РёРіСЂС‹.
    /// </summary>
    public class UniqueLocationManager : MonoBehaviour, IWorldManager
    {
        public static UniqueLocationManager Instance { get; private set; }

        // Р’СЃРµ 50 СѓРЅРёРєР°Р»СЊРЅС‹С… Р»РѕРєР°С†РёР№ (РїРѕСЃР»Рµ РіРµРЅРµСЂР°С†РёРё)
        public List<UniqueLocationInstance> Locations { get; private set; }

        // Р‘С‹СЃС‚СЂС‹Р№ РґРѕСЃС‚СѓРї: ID в†’ Instance
        private Dictionary<string, UniqueLocationInstance> lookup;

        public void Awake()
        {
            Instance = this;
        }

        // Р’С‹Р·С‹РІР°РµС‚СЃСЏ РёР· WorldLogicDirector
        public void Initialize()
        {
            LoadGeneratedLocations();

            BuildLookup();

            UDADebug.Log($"[UniqueLocationManager] Loaded {Locations.Count} unique locations.");
        }

        // ============================================================
        // Р—Р°РіСЂСѓР·РєР° СЃРіРµРЅРµСЂРёСЂРѕРІР°РЅРЅС‹С… РґР°РЅРЅС‹С… РїРѕСЃР»Рµ WorldLogicDirector.RunGenerators
        // ============================================================

        private void LoadGeneratedLocations()
        {
            // Р‘РµСЂС‘С‚СЃСЏ СЃС‚Р°С‚РёС‡РµСЃРєРёР№ СЃРїРёСЃРѕРє РёР· РіРµРЅРµСЂР°С‚РѕСЂР°
            Locations = UniqueLocationsGenerator.Instances;

            if (Locations == null)
            {
                Debug.LogError("[UniqueLocationManager] No generated unique locations found!");
                Locations = new List<UniqueLocationInstance>();
            }
        }

        private void BuildLookup()
        {
            lookup = new Dictionary<string, UniqueLocationInstance>();

            foreach (var loc in Locations)
                lookup[loc.Id] = loc;
        }

        // ============================================================
        //             Р”РћРЎРўРЈРџ Рљ Р›РћРљРђР¦РРЇРњ
        // ============================================================

        public UniqueLocationInstance GetById(string id)
        {
            lookup.TryGetValue(id, out var inst);
            return inst;
        }

        public UniqueLocationInstance GetAtTile(WorldTilePos pos)
        {
            foreach (var loc in Locations)
            {
                if (loc.Position.X == pos.X && loc.Position.Y == pos.Y)
                    return loc;
            }
            return null;
        }

        // ============================================================
        //         РћР‘РќРћР’Р›Р•РќРР• РЎРћРЎРўРћРЇРќРРЇ Р›РћРљРђР¦РР™
        // ============================================================

        public void Discover(string id)
        {
            if (lookup.TryGetValue(id, out var inst))
            {
                inst.MarkDiscovered();
                UDADebug.Log($"[UniqueLocationManager] Discovered {id}");
            }
        }

        public void Clear(string id, long timestamp)
        {
            if (lookup.TryGetValue(id, out var inst))
            {
                inst.MarkCleared(timestamp);
                UDADebug.Log($"[UniqueLocationManager] Cleared {id}");
            }
        }

        // ============================================================
        //      РџРћР›Р•Р—РќР«Р• Р¤РЈРќРљР¦РР Р”Р›РЇ UI/Р“Р•Р™РњРџР›Р•РЇ
        // ============================================================

        public int CountCleared()
        {
            int count = 0;
            foreach (var loc in Locations)
                if (loc.IsCleared) count++;
            return count;
        }

        public int CountRemaining()
        {
            int count = 0;
            foreach (var loc in Locations)
                if (!loc.IsCleared) count++;
            return count;
        }

        public IEnumerable<UniqueLocationInstance> GetDiscoveredLocations()
        {
            foreach (var loc in Locations)
                if (loc.IsDiscovered || loc.IsCleared)
                    yield return loc;
        }

        public IEnumerable<UniqueLocationInstance> GetActiveLocations()
        {
            foreach (var loc in Locations)
                if (!loc.IsCleared)
                    yield return loc;
        }

        // ============================================================
        //         РЎРѕС…СЂР°РЅРµРЅРёРµ / Р·Р°РіСЂСѓР·РєР° СЃРѕСЃС‚РѕСЏРЅРёСЏ РёРіСЂРѕРєР°
        // ============================================================

        public List<UniqueLocationState> GetStatesForSave()
        {
            List<UniqueLocationState> states = new();

            foreach (var loc in Locations)
                states.Add(loc.State);

            return states;
        }

        public void LoadFromSave(List<UniqueLocationState> savedStates)
        {
            foreach (var state in savedStates)
            {
                if (lookup.TryGetValue(state.id, out var inst))
                {
                    inst.State.position = state.position;
                    inst.State.cleared = state.cleared;
                    inst.State.discovered = state.discovered;
                    inst.State.lastClearedTimestamp = state.lastClearedTimestamp;
                    inst.State.timesCleared = state.timesCleared;
                }
            }
        }
    }
}


