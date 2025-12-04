/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/FX/BiomeFXController.cs                             */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/02 12:17:06 by UDA                                      */
/*   Updated: 2025/12/02 12:17:06 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public class BiomeFXController : MonoBehaviour
{
	public static BiomeFXController Instance;

	public FXRenderer fxRenderer;

	private string currentBiome = "";

	private void Awake()
	{
		Instance = this;
	}

	public void OnBiomeChanged(string biomeId)
	{
		if (biomeId == currentBiome) return;

		currentBiome = biomeId;

		FXProfile profile = FXLibrary.Get(biomeId);
		fxRenderer.ApplyProfile(profile);
	}
}
