using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CloseButtonHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        CloseSaveWindow();
    }

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
        {
            menuManager.CloseSaveLoadScene();
        }
        else
        {
            SceneManager.UnloadSceneAsync("SaveLoadScene");
            Time.timeScale = 1;  // Возобновляем игру, если нет MainMenu
            
            // Включаем камеру обратно, если есть
            if (CameraMaster.Instance != null)
                CameraMaster.Instance.EnablePan();
        }
    }
}
