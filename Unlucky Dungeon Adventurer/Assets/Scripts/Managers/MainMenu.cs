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
        SaveLoadState.Mode = SaveLoadMode.Load;
        SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
    }

    public void OpenSaveLoadScene_Save()
    {
        SaveLoadState.Mode = SaveLoadMode.Save;
        SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
    }

    public void CloseSaveLoadScene()
    {
        SceneManager.UnloadSceneAsync("SaveLoadScene");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings clicked � ����� ������� ��������� ����� ��������");
    }

    public void ExitGame()
    {
        Debug.Log("Exit clicked");
        Application.Quit();
    }
}
