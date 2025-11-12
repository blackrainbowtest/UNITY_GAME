using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseButtonHandler : MonoBehaviour
{
    public void CloseSaveWindow()
    {
        var fade = FindFirstObjectByType<SaveLoadFade>();
        if (fade != null)
        {
            fade.StartCoroutine(fade.FadeOutAndClose());
            return;
        }

        var menuManager = FindFirstObjectByType<MainMenu>();
        if (menuManager != null)
            menuManager.CloseSaveLoadScene();
        else
            SceneManager.UnloadSceneAsync("SaveLoadScene");
    }
}
