/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Core/DeterministicHash.cs                */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 17:48:35 by UDA                                      */
/*   Updated: 2025/12/02 17:48:35 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

namespace WorldLogic
{
	public static class DeterministicHash
	{
		public static int Hash(int seed, int salt)
		{
			unchecked
			{
				int v = seed ^ (salt * 374761393);
				v ^= (v << 13);
				v ^= (v >> 17);
				v ^= (v << 5);
				return v & 0x7fffffff;
			}
		}
	}
}
