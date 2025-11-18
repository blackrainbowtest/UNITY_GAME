using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneLoader.LoadScene("CharacterCreation");
    }

    public void OpenSaveLoadScene_Load()
    {
        SaveSlotUI.isSaveMode = false;
        SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
    }

    public void OpenSaveLoadScene_Save()
    {
        SaveSlotUI.isSaveMode = true;
        SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
    }

    public void CloseSaveLoadScene()
    {
        SceneManager.UnloadSceneAsync("SaveLoadScene");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings clicked Ч можно открыть отдельную сцену настроек");
    }

    public void ExitGame()
    {
        Debug.Log("Exit clicked");
        Application.Quit();
    }
}
