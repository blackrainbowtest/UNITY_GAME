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