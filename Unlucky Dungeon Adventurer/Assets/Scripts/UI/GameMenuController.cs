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

        Time.timeScale = 0; // ставим игру на паузу
    }

    public void Close()
    {
        isOpen = false;

        StartCoroutine(Fade(menuGroup, 1f, 0f, 0.25f));
        menuGroup.interactable = false;
        menuGroup.blocksRaycasts = false;

        Time.timeScale = 1; // снимаем паузу
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

    // кнопки
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
        // режим сохранения
        SaveLoadState.Mode = SaveLoadMode.Save;

        // закрываем меню
        Close();

        // включаем время (окно SaveLoad должно работать)
        Time.timeScale = 1;

        // открываем сцену поверх
        SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
    }

    public void OnLoadClicked()
    {
        // режим загрузки
        SaveLoadState.Mode = SaveLoadMode.Load;

        Close();
        Time.timeScale = 1;

        SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
    }

}
