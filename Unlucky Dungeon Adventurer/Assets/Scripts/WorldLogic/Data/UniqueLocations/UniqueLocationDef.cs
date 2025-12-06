/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/WorldLogic/Data/UniqueLocations/UniqueLocationDef.cs*/
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 14:30:56 by UDA                                      */
/*   Updated: 2025/12/02 14:30:56 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

namespace WorldLogic
{
	[CreateAssetMenu(
		fileName = "UniqueLocation",
		menuName = "World/UniqueLocation",
		order = 1)]
	public class UniqueLocationDef : ScriptableObject
	{
		[Header("ID (must be unique!)")]
		public string id;

		[Header("Icon shown on the world map")]
		public Sprite icon;

		[Header("Required biome")]
		public string requiredBiome; // e.g. "mountain", "swamp"

		[Header("Spawn weight among all unique locations")]
		public int rarity = 1;

		[Header("Difficulty tier of the location")]
		public int dangerLevel = 1;

		[Header("Spawn rules")]
		public bool nearMountains;
		public bool nearWater;
		public bool farFromCities;

		[Header("Spawn distribution settings")]
		public int spawnRadius = 3;
		public int spawnAttempts = 500;

		[Header("Localization keys")]
		public string nameKey;        // e.g. "dragon_lair.name"
		public string descriptionKey; // e.g. "dragon_lair.description"
	}
}

