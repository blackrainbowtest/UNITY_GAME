using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(LoadMainMenu());
	}

	// Load persistent game data early in the lifecycle.
	// Awake runs before any Start methods, so it's a good place for registry initialization.
	private void Awake()
	{
		DataRegistry.EnsureLoaded();
		BiomeDB.EnsureLoaded();
	}

	// Defer scene loading by one frame to ensure any required singletons (e.g. GameManager)
	// created in Awake/Start are ready before switching scenes.
	private System.Collections.IEnumerator LoadMainMenu()
	{
		yield return null;
		SceneLoader.LoadScene("MainMenu");
	}
}
