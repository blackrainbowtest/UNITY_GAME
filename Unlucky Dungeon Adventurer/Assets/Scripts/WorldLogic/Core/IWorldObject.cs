/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Core/IWorldObject.cs                     */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 13:56:25 by UDA                                      */
/*   Updated: 2025/12/02 13:56:25 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

namespace WorldLogic
{
	/// <summary>
	/// Basic interface for any object on the global map.
	/// Doesn't know about specific subtypes (city/den/caravan).
	/// </summary>
	public interface IWorldObject
	{
		string Id { get; }              // A unique ID in the world session (e.g. "city_001")
		WorldObjectKind Kind { get; }   // Type (city, village, den, etc.)
		WorldTilePos Position { get; }  // What tile is it in?
	}
}
