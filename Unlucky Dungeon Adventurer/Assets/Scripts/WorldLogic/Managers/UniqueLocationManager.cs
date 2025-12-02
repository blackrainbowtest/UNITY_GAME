/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Managers/UniqueLocationManager.cs        */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
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
    /// Главный менеджер уникальных локаций.
    /// Управляет всеми 50 инстансами после генерации.
    /// Сохраняет/загружает state и предоставляет API для игры.
    /// </summary>
    public class UniqueLocationManager : MonoBehaviour, IWorldManager
    {
        public static UniqueLocationManager Instance { get; private set; }

        // Все 50 уникальных локаций (после генерации)
        public List<UniqueLocationInstance> Locations { get; private set; }

        // Быстрый доступ: ID → Instance
        private Dictionary<string, UniqueLocationInstance> lookup;

        public void Awake()
        {
            Instance = this;
        }

        // Вызывается из WorldLogicDirector
        public void Initialize()
        {
            LoadGeneratedLocations();

            BuildLookup();

            Debug.Log($"[UniqueLocationManager] Loaded {Locations.Count} unique locations.");
        }

        // ============================================================
        // Загрузка сгенерированных данных после WorldLogicDirector.RunGenerators
        // ============================================================

        private void LoadGeneratedLocations()
        {
            // Берётся статический список из генератора
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
        //             ДОСТУП К ЛОКАЦИЯМ
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
        //         ОБНОВЛЕНИЕ СОСТОЯНИЯ ЛОКАЦИЙ
        // ============================================================

        public void Discover(string id)
        {
            if (lookup.TryGetValue(id, out var inst))
            {
                inst.MarkDiscovered();
                Debug.Log($"[UniqueLocationManager] Discovered {id}");
            }
        }

        public void Clear(string id, long timestamp)
        {
            if (lookup.TryGetValue(id, out var inst))
            {
                inst.MarkCleared(timestamp);
                Debug.Log($"[UniqueLocationManager] Cleared {id}");
            }
        }

        // ============================================================
        //      ПОЛЕЗНЫЕ ФУНКЦИИ ДЛЯ UI/ГЕЙМПЛЕЯ
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
        //         Сохранение / загрузка состояния игрока
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

