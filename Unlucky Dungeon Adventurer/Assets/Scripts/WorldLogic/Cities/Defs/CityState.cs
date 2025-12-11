/* ************************************************************************** */
/*                                                                            */
/*   File: CityState.cs                                   /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/11                                                     */
/*   Updated: 2025/12/11                                                     */
/*                                                                            */
/* ************************************************************************** */

namespace WorldLogic.Cities
{
    /// <summary>
    /// Состояние города в сейве. Сохраняется в worldData.cityStates.
    /// Содержит координаты, текущее население, факт открытия и прочее.
    /// </summary>
    [System.Serializable]
    public class CityState
    {
        /// <summary>Ссылка на CityDef.id</summary>
        public string id;

        /// <summary>Позиция города на карте</summary>
        public WorldTilePos position;

        /// <summary>Текущее население города</summary>
        public int currentPopulation;

        /// <summary>ID фракции, контролирующей город</summary>
        public string factionId;

        /// <summary>Открыта ли локация игроком</summary>
        public bool discovered;

        /// <summary>День последнего посещения (или -1, если не посещали)</summary>
        public int lastVisitedDay = -1;

        /// <summary>Дополнительное состояние для будущего расширения (экономика, события и т.п.)</summary>
        public string additionalData = "";
    }
}
