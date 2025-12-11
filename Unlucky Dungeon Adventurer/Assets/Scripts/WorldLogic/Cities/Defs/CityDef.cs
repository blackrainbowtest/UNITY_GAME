/* ************************************************************************** */
/*                                                                            */
/*   File: CityDef.cs                                     /\_/\               */
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
    /// ScriptableObject с базовыми характеристиками города.
    /// Текстовые поля (имя/описание) берутся из переводов по ключу.
    /// </summary>
    [CreateAssetMenu(
        fileName = "CityDef",
        menuName = "World/Cities/City Definition",
        order = 1)]
    public class CityDef : ScriptableObject
    {
        [Header("Unique ID (internal, never changes)")]
        public string id;

        [Header("Localization keys")]
        public string displayNameKey;  
        public string descriptionKey;

        [Header("Icon or minimap marker for city")]
        public Sprite icon;

        [Header("Population range for generation")]
        public int minPopulation = 300;
        public int maxPopulation = 3000;

        [Header("Faction or alignment")]
        public string factionId;

        [Header("Allowed biomes for city placement")]
        public string[] allowedBiomes = new string[] { "plains", "forest" };

        [Header("How large the city is (visual/logic usage)")]
        public int sizeCategory = 1;  // 1 = village, 2 = town, 3 = city, etc.
    }
}
