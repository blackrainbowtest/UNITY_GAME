using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseButtonHandler : MonoBehaviour
{
    public void CloseSaveWindow()
    {
        var menuManager = Object.FindFirstObjectByType<MainMenu>();

        if (menuManager != null)
            menuManager.CloseSaveLoadScene();
        else
            SceneManager.UnloadSceneAsync("SaveLoadScene");
    }
}
