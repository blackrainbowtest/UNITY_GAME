using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneLoader.LoadScene("CharacterCreation");
    }

    public void LoadGame()
    {
        Debug.Log("Load game clicked Ч здесь будет логика загрузки сохранений");
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
