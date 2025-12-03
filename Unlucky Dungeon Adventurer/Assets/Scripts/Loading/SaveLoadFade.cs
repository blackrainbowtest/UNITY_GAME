/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/Loading/SaveLoadFade.cs                             */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 09:23:31 by UDA                                      */
/*   Updated: 2025/12/03 09:23:31 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

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
