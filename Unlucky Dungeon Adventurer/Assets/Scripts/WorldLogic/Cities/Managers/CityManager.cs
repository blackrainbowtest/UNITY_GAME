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
        public void Initialize(List<CityState> cityStates)
    {
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

        // Создаём CityInstance для каждого CityState
        foreach (var state in cityStates)
        {
            if (defDict.TryGetValue(state.id, out var def))
            {
                var instance = new CityInstance(def, state);
                cities[state.id] = instance;
            }
            else
            {
                Debug.LogWarning($"[CityManager] CityDef not found for id: {state.id}");
            }
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

        /// <summary>Найти город по позиции на карте.</summary>
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
                states.Add(city.state);
            }
            return states;
        }

        /// <summary>Загрузить города из сохранённого состояния (вызывается из WorldLogicDirector).</summary>
        public void LoadInitialFromSave(List<CityState> savedStates, List<CityDef> allDefs)
        {
            cities.Clear();

            if (savedStates == null || savedStates.Count == 0)
            {
                Debug.LogWarning("[CityManager] No saved city states to load.");
                return;
            }

            var defDict = new Dictionary<string, CityDef>();
            foreach (var def in allDefs)
            {
                if (!string.IsNullOrEmpty(def.id))
                    defDict[def.id] = def;
            }

            foreach (var state in savedStates)
            {
                if (defDict.TryGetValue(state.id, out var def))
                {
                    var instance = new CityInstance(def, state);
                    cities[state.id] = instance;
                }
                else
                {
                    Debug.LogWarning($"[CityManager] CityDef not found for saved city: {state.id}");
                }
            }

            Debug.Log($"[CityManager] Loaded {cities.Count} cities from save.");
        }
    }
}
