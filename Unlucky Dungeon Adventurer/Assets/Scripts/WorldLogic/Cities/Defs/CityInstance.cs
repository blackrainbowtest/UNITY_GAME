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
    /// Содержит ссылку на CityDef, CityState и реальные данные в памяти.
    /// </summary>
    public class CityInstance
    {
        /// <summary>Дефиниция города (неизменяемая).</summary>
        public CityDef def;

        /// <summary>Состояние города (изменяется, сохраняется в сейв).</summary>
        public CityState state;

        /// <summary>Позиция на карте (в тайлах).</summary>
        public WorldTilePos position => state.position;

        /// <summary>Display name из переводов.</summary>
        public string displayName;

        /// <summary>Description из переводов.</summary>
        public string description;

        /// <summary>Конструктор из CityDef и CityState.</summary>
        public CityInstance(CityDef cityDef, CityState cityState)
        {
            def = cityDef;
            state = cityState;
            
            // Загружаем локализованные строки
            LanguageManager.LoadLanguage("cities");
            displayName = LanguageManager.Get(cityDef.displayNameKey);
            description = LanguageManager.Get(cityDef.descriptionKey);
        }
    }
}
