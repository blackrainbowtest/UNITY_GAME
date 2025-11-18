using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SaveLoadFade : MonoBehaviour
{
    private CanvasGroup group;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        group.alpha = 0;
        while (group.alpha < 1)
        {
            group.alpha += Time.deltaTime * 2f;
            yield return null;
        }
        group.alpha = 1;
    }

    public IEnumerator FadeOutAndClose()
    {
        while (group.alpha > 0)
        {
            group.alpha -= Time.deltaTime * 2f;
            yield return null;
        }
        group.alpha = 0;

        // 🧩 вместо вызова CloseSaveWindow() просто закрываем сцену напрямую
        var menu = FindFirstObjectByType<MainMenu>();
        if (menu != null)
            menu.CloseSaveLoadScene();
        else
            SceneManager.UnloadSceneAsync("SaveLoadScene");
    }
}
