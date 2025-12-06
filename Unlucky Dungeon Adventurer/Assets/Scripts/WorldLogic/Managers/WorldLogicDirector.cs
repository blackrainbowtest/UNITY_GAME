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
            WorldSeed = seed;
            Generator = generator;

            RegisterManagers();

            // Check if unique locations already exist in save
            var saveData = GameManager.Instance.GetCurrentGameData();
            var worldSave = saveData.world;

            if (worldSave.HasUniqueLocations())
            {
                // Load existing locations from save
                var mgr = FindFirstObjectByType<UniqueLocationManager>();
                if (mgr != null)
                {
                    var defs = new List<UniqueLocationDef>(
                        Resources.LoadAll<UniqueLocationDef>("WorldData/UniqueLocations")
                    );
                    mgr.LoadInitialFromSave(worldSave.uniqueLocationStates, defs);
                    mgr.Initialize(); // Initialize to print coordinates
                }

                // Initialize all managers after loading from save
                InitializeManagers();
            }
            else
            {
                // Generate new locations for first-time world
                RegisterGenerators();
                RunGenerators();

                // Save generated locations to save data
                var mgr = FindFirstObjectByType<UniqueLocationManager>();
                if (mgr != null)
                {
                    mgr.Initialize();
                    worldSave.uniqueLocationStates = mgr.GetStatesForSave();
                }
                else
                {
                    Debug.LogError("[WorldLogicDirector] UniqueLocationManager not found in scene!");
                }

                // Initialize any remaining managers after generation
                InitializeManagers();
            }
        }

        private void RegisterGenerators()
        {
			generators.Add(new UniqueLocationsGenerator());
            // В будущем:
            // generators.Add(new CityGenerator());
            // generators.Add(new EventGenerator());
        }

        private void RegisterManagers()
        {
			var uniqueLocMgr = FindFirstObjectByType<UniqueLocationManager>();
			if (uniqueLocMgr != null)
				managers.Add(uniqueLocMgr);
			else
				Debug.LogWarning("[WorldLogic] UniqueLocationManager not found in scene!");
			// Сюда мы добавим ссылки на акторов, события, фракции
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
