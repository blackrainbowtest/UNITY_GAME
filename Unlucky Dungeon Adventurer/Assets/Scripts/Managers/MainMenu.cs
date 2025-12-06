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
        Time.timeScale = 1;  // Р’РѕР·РѕР±РЅРѕРІР»СЏРµРј РёРіСЂСѓ
        
        // Р’РєР»СЋС‡Р°РµРј РєР°РјРµСЂСѓ РѕР±СЂР°С‚РЅРѕ, РµСЃР»Рё РµСЃС‚СЊ
        if (CameraMaster.Instance != null)
            CameraMaster.Instance.EnablePan();
    }

    public void OpenSettings()
    {
        UDADebug.Log("Settings clicked пїЅ пїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ");
    }

    public void ExitGame()
    {
        UDADebug.Log("Exit clicked");
        Application.Quit();
    }
}

