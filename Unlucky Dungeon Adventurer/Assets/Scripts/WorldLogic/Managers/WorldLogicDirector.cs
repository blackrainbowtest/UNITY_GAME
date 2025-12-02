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

            RegisterGenerators();
            RegisterManagers();

            RunGenerators();
            InitializeManagers();
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
			var mgr = FindObjectOfType<UniqueLocationManager>();
			if (mgr != null)
				managers.Add(mgr);
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

/**
Шаг 2. Подключаем Дирижёра к твоему миру

Сейчас твой мир рендерится, тайлы генерируются — но никто не “ведёт” объекты, события, уникальные места.

Делаем так:

В сцене WorldMap создаём пустой GameObject:
WorldLogicDirector

Навешиваем на него компонент WorldLogicDirector.

WorldLogicDirector (GameObject)
   - WorldLogicDirector.cs (компонент)

*/