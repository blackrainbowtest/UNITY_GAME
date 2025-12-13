/* ************************************************************************** */
/*                                                                            */
/*   File: CityManager.cs                                 /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/11                                                     */
/*   Updated: 2025/12/11                                                     */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using UnityEngine;

namespace WorldLogic.Cities
{
    /// <summary>
    /// Менеджер городов. Хранит список всех городов, предоставляет доступ по ID,
    /// управляет состоянием и интеграцией с сейвом.
    /// </summary>
    public class CityManager : MonoBehaviour, IWorldManager
    {
        public static CityManager Instance { get; private set; }

        /// <summary>Словарь городов (по ID из CityState).</summary>
        private Dictionary<string, CityInstance> cities = new Dictionary<string, CityInstance>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>Реализация IWorldManager.Initialize() — пусто, т.к. инициализация через Initialize(List&lt;CityState&gt;).</summary>
        void IWorldManager.Initialize()
        {
            // Реальная инициализация в Initialize(List<CityState>)
        }

        /// <summary>Инициализировать менеджер из списка CityState.</summary>
        public void Initialize(List<CityState> cityStates, int worldSeed, WorldGenerator worldGen)
        {
            Debug.Log($"[CityManager] ===== Initialize called with {(cityStates != null ? cityStates.Count : 0)} city states =====");
            
            cities.Clear();
            
            if (cityStates == null || cityStates.Count == 0)
            {
                Debug.LogWarning("[CityManager] No city states provided.");
                return;
            }

            // Загружаем все CityDef из Resources
            var cityDefs = Resources.LoadAll<CityDef>("WorldData/Cities");
            var defDict = new Dictionary<string, CityDef>();
            foreach (var def in cityDefs)
            {
                if (!string.IsNullOrEmpty(def.id))
                    defDict[def.id] = def;
            }

            // Используем первый доступный def (т.к. у городов теперь динамический id)
            CityDef defaultDef = cityDefs.Length > 0 ? cityDefs[0] : null;
            if (defaultDef == null)
            {
                Debug.LogError("[CityManager] No CityDef found in Resources!");
                return;
            }

            // Создаём CityInstance для каждого CityState
            foreach (var state in cityStates)
            {
                var instance = new CityInstance(defaultDef, state, worldSeed, worldGen);
                cities[state.id] = instance;
            }

            Debug.Log($"[CityManager] Initialized {cities.Count} cities.");
        }

        /// <summary>Получить город по ID.</summary>
        public CityInstance GetCity(string cityId)
        {
            if (cities.TryGetValue(cityId, out var city))
                return city;
            return null;
        }

        /// <summary>Получить всех города.</summary>
        public IEnumerable<CityInstance> GetAllCities()
        {
            return cities.Values;
        }

        /// <summary>Получить город по точной позиции тайла.</summary>
        public CityInstance GetCityAt(Vector2Int tilePos)
        {
            foreach (var city in cities.Values)
            {
                if (city.state.position.X == tilePos.x && city.state.position.Y == tilePos.y)
                    return city;
            }
            return null;
        }

        /// <summary>Найти город по позиции на карте с допуском.</summary>
        public CityInstance GetCityAtPosition(Vector2Int position, int maxDistance = 2)
        {
            foreach (var city in cities.Values)
            {
                int dist = System.Math.Abs(city.state.position.X - position.x) + 
                           System.Math.Abs(city.state.position.Y - position.y);
                if (dist <= maxDistance)
                    return city;
            }
            return null;
        }

        /// <summary>Пометить город как открытый.</summary>
        public void DiscoverCity(string cityId, int currentDay)
        {
            if (cities.TryGetValue(cityId, out var city))
            {
                city.state.discovered = true;
                city.state.lastVisitedDay = currentDay;
            }
        }

        /// <summary>Получить все состояния городов для сохранения.</summary>
        public List<CityState> GetStatesForSave()
        {
            var states = new List<CityState>();
            foreach (var city in cities.Values)
            {
                // Save only discovered or changed cities (simple heuristic)
                if (city.state.discovered || city.state.lastVisitedDay >= 0 || !string.IsNullOrEmpty(city.state.additionalData))
                    states.Add(city.state);
            }
            return states;
        }

        /// <summary>Загрузить города из сохранённого состояния (вызывается из WorldLogicDirector).</summary>
        public void LoadInitialFromSave(List<CityState> savedStates, List<CityDef> allDefs, int worldSeed, WorldGenerator worldGen)
        {
            cities.Clear();

            if (savedStates == null || savedStates.Count == 0)
            {
                Debug.LogWarning("[CityManager] No saved city states to load.");
                return;
            }

            // Используем первый доступный def (т.к. города теперь не привязаны к конкретному CityDef)
            CityDef defaultDef = allDefs != null && allDefs.Count > 0 ? allDefs[0] : null;
            if (defaultDef == null)
            {
                Debug.LogError("[CityManager] No CityDef available for loading cities!");
                return;
            }

            foreach (var state in savedStates)
            {
                var instance = new CityInstance(defaultDef, state, worldSeed, worldGen);
                cities[state.id] = instance;
            }

            Debug.Log($"[CityManager] Loaded {cities.Count} cities from save.");
        }

        /// <summary>
        /// Apply overrides from save onto existing generated cities.
        /// </summary>
        public void ApplyOverrides(List<CityState> overrides)
        {
            if (overrides == null || overrides.Count == 0) return;
            int applied = 0;
            foreach (var ov in overrides)
            {
                if (cities.TryGetValue(ov.id, out var inst))
                {
                    inst.state.discovered = ov.discovered;
                    inst.state.lastVisitedDay = ov.lastVisitedDay;
                    if (!string.IsNullOrEmpty(ov.factionId)) inst.state.factionId = ov.factionId;
                    if (ov.currentPopulation != 0) inst.state.currentPopulation = ov.currentPopulation; // optional
                    if (!string.IsNullOrEmpty(ov.additionalData)) inst.state.additionalData = ov.additionalData;
                    applied++;
                }
            }
            Debug.Log($"[CityManager] Applied {applied} city overrides from save.");
        }
    }
}
