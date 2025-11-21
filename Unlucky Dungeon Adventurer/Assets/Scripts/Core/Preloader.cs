using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(LoadMainMenu());
	}

	/**
	 * Automatically load this data when the game starts.
	 */
	private void Awake()
	{
		DataRegistry.LoadAll();
	}

	/**
	 * Wait for a frame to ensure GameManager is created
	 */
	private System.Collections.IEnumerator LoadMainMenu()
	{
		yield return null;
		SceneLoader.LoadScene("MainMenu");
	}
}
