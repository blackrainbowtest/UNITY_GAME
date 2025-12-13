/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Managers/WorldLogicDirector.cs           */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 13:38:52 by UDA                                      */
/*   Updated: 2025/12/02 13:38:52 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using System.Collections.Generic;
using WorldLogic.Cities;
using WorldLogic.Generators;

namespace WorldLogic
{
    public class WorldLogicDirector : MonoBehaviour
    {
        public int WorldSeed { get; private set; }
        public WorldGenerator Generator { get; private set; }

        private readonly List<IGenerator> generators = new();
        private readonly List<IWorldManager> managers = new();

        public void Initialize(int seed, WorldGenerator generator)
        {
            Debug.Log($"[WorldLogicDirector] ===== INITIALIZE CALLED ===== seed={seed}");
            
            WorldSeed = seed;
            Generator = generator;

            RegisterManagers();

            // Check if unique locations already exist in save
            var saveData = GameManager.Instance.GetCurrentGameData();
            var worldSave = saveData.world;

            // --------------------------
            // ЗАГРУЗКА из сейва
            // --------------------------
            bool hasUniqueLocations = worldSave.HasUniqueLocations();
            bool hasCities = worldSave.HasCities();

            Debug.Log($"[WorldLogicDirector] Save check: hasUniqueLocations={hasUniqueLocations}, hasCities={hasCities}");
            Debug.Log($"[WorldLogicDirector] cityStates count: {(worldSave.cityStates != null ? worldSave.cityStates.Count : 0)}");

            if (hasUniqueLocations || hasCities)
            {
                Debug.Log("[WorldLogicDirector] Loading world logic from save... (UL:" + hasUniqueLocations + ", Cities:" + hasCities + ")");

                // Load unique locations if they exist
                if (hasUniqueLocations)
                {
                    var ulMgr = FindFirstObjectByType<UniqueLocationManager>();
                    if (ulMgr != null)
                    {
                        var defs = new List<UniqueLocationDef>(
                            Resources.LoadAll<UniqueLocationDef>("WorldData/UniqueLocations")
                        );
                        ulMgr.LoadInitialFromSave(worldSave.uniqueLocationStates, defs);
                    }
                }

                // Always generate cities deterministically, then patch from save
                RegisterGenerators();
                RunGenerators();
                {
                    var loadedCityMgr = FindFirstObjectByType<CityManager>();
                    if (loadedCityMgr != null)
                    {
                        // Initialize from freshly generated deterministic states
                        loadedCityMgr.Initialize(CityGenerator.GeneratedStates, WorldSeed, Generator);

                        // Patch from save only discovered/changed states
                        if (hasCities && worldSave.cityStates != null && worldSave.cityStates.Count > 0)
                        {
                            Debug.Log($"[WorldLogicDirector] Patching {worldSave.cityStates.Count} saved city overrides...");
                            loadedCityMgr.ApplyOverrides(worldSave.cityStates);
                        }
                    }
                }

                // If some data missing, generate it (unique locations only)
                if (!hasUniqueLocations)
                {
                    Debug.Log("[WorldLogicDirector] Missing unique locations, generating...");
                    // unique locations generator may already be registered above; ensure save
                    var ulMgr = FindFirstObjectByType<UniqueLocationManager>();
                    if (ulMgr != null)
                    {
                        ulMgr.Initialize();
                        worldSave.uniqueLocationStates = ulMgr.GetStatesForSave();
                    }
                }

            InitializeManagers();

            // Render cities on world map
            var cityRenderer = FindFirstObjectByType<CityWorldRenderer>();
            var savedCityMgr = FindFirstObjectByType<CityManager>();
            if (cityRenderer != null && savedCityMgr != null)
                cityRenderer.Initialize(savedCityMgr, Generator, WorldSeed);

            return;
            }

            // --------------------------
            // ПЕРВОЕ СОЗДАНИЕ МИРА
            // --------------------------

            RegisterGenerators();
            RunGenerators();

            // Save unique locations
            {
                var ulMgr = FindFirstObjectByType<UniqueLocationManager>();
                if (ulMgr != null)
                {
                    ulMgr.Initialize();
                    worldSave.uniqueLocationStates = ulMgr.GetStatesForSave();
                }
            }

            // Save cities
            {
                var cityMgr = FindFirstObjectByType<CityManager>();
                if (cityMgr != null)
                {
                    cityMgr.Initialize(CityGenerator.GeneratedStates, WorldSeed, Generator);
                    worldSave.cityStates = cityMgr.GetStatesForSave();
                }
            }

            InitializeManagers();

            // Render cities on world map
            var newCityRenderer = FindFirstObjectByType<CityWorldRenderer>();
            var newCityMgr = FindFirstObjectByType<CityManager>();
            if (newCityRenderer != null && newCityMgr != null)
                newCityRenderer.Initialize(newCityMgr, Generator, WorldSeed);

            Debug.Log("[WorldLogicDirector] World logic initialized for new world.");
        }

        private void RegisterGenerators()
        {
            generators.Add(new UniqueLocationsGenerator());
            generators.Add(new CityGenerator());
            // В будущем:
            // generators.Add(new VillageGenerator());
            // generators.Add(new PointsOfInterestGenerator());
            // generators.Add(new EnemyCampsGenerator());
        }

        private void RegisterManagers()
        {
            var ulMgr = FindFirstObjectByType<UniqueLocationManager>();
            if (ulMgr != null)
                managers.Add(ulMgr);
            else
                Debug.LogWarning("[WorldLogic] UniqueLocationManager not found in scene!");

            var cityMgr = FindFirstObjectByType<CityManager>();
            if (cityMgr != null)
                managers.Add(cityMgr);
            else
                Debug.LogWarning("[WorldLogic] CityManager not found in scene!");
        }

        private void RunGenerators()
        {
            foreach (var gen in generators)
                gen.Generate(WorldSeed, Generator);
        }

        private void InitializeManagers()
        {
            foreach (var mgr in managers)
                mgr.Initialize();
        }
    }
}
