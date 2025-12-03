/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Transitions/SceneLoader.cs               */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 10:17:46 by UDA                                      */
/*   Updated: 2025/12/03 10:17:46 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
	public static string sceneToLoad;

	public static void LoadScene(string sceneName)
	{
		sceneToLoad = sceneName;
		SceneManager.LoadScene("LoadingScene");
	}
}
