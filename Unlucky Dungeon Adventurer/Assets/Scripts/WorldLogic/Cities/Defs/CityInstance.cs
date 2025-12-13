/* ************************************************************************** */
/*                                                                            */
/*   File: CityInstance.cs                                /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/11                                                     */
/*   Updated: 2025/12/11                                                     */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

namespace WorldLogic.Cities
{
    /// <summary>
    /// Runtime представление города на карте.
    /// Содержит ссылку на CityDef, CityState и детерминированно сгенерированное имя.
    /// </summary>
    public class CityInstance
    {
        /// <summary>Дефиниция города (неизменяемая).</summary>
        public CityDef def;

        /// <summary>Состояние города (изменяется, сохраняется в сейв).</summary>
        public CityState state;

        /// <summary>Позиция на карте (в тайлах).</summary>
        public WorldTilePos position => state.position;

        /// <summary>Детерминированное имя города (генерируется из биома + seed + index).</summary>
        public string displayName;

        /// <summary>Описание, детерминированно выбранное для биома.</summary>
        public string description;

        /// <summary>Конструктор из CityDef, CityState и WorldGenerator для получения биома.</summary>
        public CityInstance(CityDef cityDef, CityState cityState, int worldSeed, WorldGenerator worldGen)
        {
            def = cityDef;
            state = cityState;
            
            // Получаем биом из позиции города
            var tile = worldGen.GetTile(cityState.position.X, cityState.position.Y);
            string biomeId = tile?.biomeId ?? "unknown";

            // Генерируем детерминированные имя и описание
            displayName = CityNameDatabase.GetName(biomeId, worldSeed, cityState.generationIndex);
            // Fallback, если в базе нет названий
            if (string.IsNullOrEmpty(displayName) || displayName == "Unknown City")
                displayName = $"city_{cityState.position.X}_{cityState.position.Y}";
            description = CityNameDatabase.GetDescription(biomeId, worldSeed, cityState.generationIndex);
        }
    }
}
