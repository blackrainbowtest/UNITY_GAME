/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Core/WorldTilePos.cs                     */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 13:53:49 by UDA                                      */
/*   Updated: 2025/12/02 13:53:49 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System;
using UnityEngine;

namespace WorldLogic
{
	[Serializable]
	public struct WorldTilePos
	{
		public int X;
		public int Y;

		public WorldTilePos(int x, int y)
		{
			X = x;
			Y = y;
		}

		public static WorldTilePos FromVector2Int(Vector2Int v)
		{
			return new WorldTilePos(v.x, v.y);
		}

		public Vector2Int ToVector2Int()
		{
			return new Vector2Int(X, Y);
		}

		public override string ToString()
		{
			return $"({X}, {Y})";
		}

		public static WorldTilePos Zero => new WorldTilePos(0, 0);
	}
}
