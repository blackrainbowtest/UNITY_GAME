/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Data/UniqueLocations/                    */
/*   UniqueLocationState.cs                               /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 16:24:10 by UDA                                      */
/*   Updated: 2025/12/02 16:24:10 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System;

namespace WorldLogic
{
    /// <summary>
    /// Runtime-состояние уникальной локации для сейвов.
    /// Привязано к конкретному миру и прогрессу игрока.
    /// </summary>
    [Serializable]
    public class UniqueLocationState
    {
        // ID должен совпадать с UniqueLocationDef.id
        public string id;

        // Позиция локации на глобальной карте
        public WorldTilePos position;

        // Игрок хотя бы раз обнаружил эту локацию на карте?
        public bool discovered;

        // Локация полностью зачищена (главная цель выполнена)?
        public bool cleared;

        // Сколько раз игрок её зачищал (если позволишь переигрывать)
        public int timesCleared;

        // Внутриигровое время последней зачистки (если есть система времени)
        public long lastClearedTimestamp;

        // Удобное вычисляемое свойство
        public bool IsActive => !cleared;

        public UniqueLocationState(string id, WorldTilePos position)
        {
            this.id = id;
            this.position = position;
            discovered = false;
            cleared = false;
            timesCleared = 0;
            lastClearedTimestamp = 0;
        }
    }
}
