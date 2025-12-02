/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Data/UniqueLocations/                    */
/*   UniqueLocationInstance.cs                            /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 14:33:03 by UDA                                      */
/*   Updated: 2025/12/02 14:33:03 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

namespace WorldLogic
{
    /// <summary>
    /// Runtime-объект уникальной локации.
    /// Содержит статические данные (Def) и динамические данные (State).
    /// </summary>
    public class UniqueLocationInstance : IWorldObject
    {
        public UniqueLocationDef Def { get; private set; }
        public UniqueLocationState State { get; private set; }

        public string Id => State.id;
        public WorldObjectKind Kind => WorldObjectKind.UniqueLocation;
        public WorldTilePos Position => State.position;

        public bool IsDiscovered => State.discovered;
        public bool IsCleared => State.cleared;
        public int DangerLevel => Def.dangerLevel;

        public UniqueLocationInstance(UniqueLocationDef def, UniqueLocationState state)
        {
            Def = def;
            State = state;
        }

        /// <summary>
        /// Пометить локацию как обнаруженную игроком.
        /// </summary>
        public void MarkDiscovered()
        {
            State.discovered = true;
        }

        /// <summary>
        /// Пометить как зачищенную.
        /// Увеличивает счетчик и записывает время.
        /// </summary>
        public void MarkCleared(long timestamp)
        {
            State.cleared = true;
            State.timesCleared++;
            State.lastClearedTimestamp = timestamp;
        }

        /// <summary>
        /// Удобно для будущих UI систем (иконки на карте).
        /// </summary>
        public bool ShouldShowOnMap()
        {
            return State.discovered || State.cleared;
        }
    }
}
