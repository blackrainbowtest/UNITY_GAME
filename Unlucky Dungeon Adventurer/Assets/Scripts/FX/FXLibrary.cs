/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/FX/FXLibrary.cs                                     */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 12:16:35 by UDA                                      */
/*   Updated: 2025/12/02 12:16:35 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public class FXLibrary : MonoBehaviour
{
	public static FXLibrary Instance;

	public FXProfile[] profiles;

	private void Awake()
	{
		Instance = this;
	}

	public static FXProfile Get(string biomeId)
	{
		if (Instance == null || Instance.profiles == null)
			return null;

		foreach (var p in Instance.profiles)
			if (p != null && p.biomeId == biomeId)
				return p;

		return null;
	}
}
