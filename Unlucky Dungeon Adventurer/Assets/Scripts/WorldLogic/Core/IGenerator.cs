/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Core/IGenerator.cs                       */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 13:47:24 by UDA                                      */
/*   Updated: 2025/12/02 13:47:24 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

namespace WorldLogic
{
	/// <summary>
	/// World data generator.
	/// Runs once when a world is created/loaded,
	/// based on a seed and WorldGenerator.
	/// </summary>
	public interface IGenerator
	{
		void Generate(int seed, WorldGenerator worldGen);
	}
}