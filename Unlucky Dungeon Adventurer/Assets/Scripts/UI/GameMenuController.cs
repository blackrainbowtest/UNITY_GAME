using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameMenuController : MonoBehaviour
{
    public static GameMenuController Instance;

    [Header("UI")]
    public CanvasGroup menuGroup;

    private bool isOpen = false;

    private void Awake()
    {
        Instance = this;
        HideInstant();
    }

    public void ToggleMenu()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    public void Open()
    {
        isOpen = true;
        StartCoroutine(Fade(menuGroup, 0f, 1f, 0.25f));
        menuGroup.interactable = true;
        menuGroup.blocksRaycasts = true;
        Time.timeScale = 0;

        // Disable camera controls while menu is open
        if (CameraMaster.Instance != null)
            CameraMaster.Instance.DisablePan();
    }

    public void Close()
    {
        isOpen = false;
        StartCoroutine(Fade(menuGroup, 1f, 0f, 0.25f));
        menuGroup.interactable = false;
        menuGroup.blocksRaycasts = false;
        Time.timeScale = 1;

        // Re-enable camera controls when menu closes
        if (CameraMaster.Instance != null)
            CameraMaster.Instance.EnablePan();
    }

    private void HideInstant()
    {
        menuGroup.alpha = 0;
        menuGroup.interactable = false;
        menuGroup.blocksRaycasts = false;
    }

    private System.Collections.IEnumerator Fade(CanvasGroup g, float a, float b, float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            g.alpha = Mathf.Lerp(a, b, t / time);
            yield return null;
        }

        g.alpha = b;
    }

    public void OnResumeClicked()
    {
        Close();
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void OnSaveClicked()
    {
        SaveLoadState.Mode = SaveLoadMode.Save;
        Close();
        Time.timeScale = 0;  // Оставляем на паузе
        SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
    }

    public void OnLoadClicked()
    {
        SaveLoadState.Mode = SaveLoadMode.Load;
        Close();
        Time.timeScale = 0;  // Оставляем на паузе
        SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
    }

}
