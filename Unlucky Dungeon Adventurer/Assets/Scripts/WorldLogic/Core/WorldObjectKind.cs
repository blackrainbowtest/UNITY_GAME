/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Core/WorldObjectKind.cs                  */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 13:55:10 by UDA                                      */
/*   Updated: 2025/12/02 13:55:10 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

namespace WorldLogic
{
	public enum WorldObjectKind
	{
		Unknown = 0,

		// Static points
		City,
		Village,
		DungeonEntrance,
		Cave,
		Ruins,
		Shrine,
		UniqueLocation,    // lair of a dragon, lich, etc.

		// Dynamic things
		Caravan,
		MonsterPack,
		Patrol,
		Anomaly,

		// It can be expanded further
	}
}
