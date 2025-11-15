using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(LoadMainMenu());
	}

	private System.Collections.IEnumerator LoadMainMenu()
	{
		// Ждём кадр, чтобы гарантировать создание GameManager
		yield return null;

		// Загружаем главное меню
		SceneLoader.LoadScene("MainMenu");
	}
}
