/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/UI/LoadingOverlayController.cs                      */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/10 by UDA                                               */
/*   Updated: 2025/12/10 by UDA                                               */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер для overlay-загрузочного экрана с анимацией бегуна
/// Показывается при телепорте камеры на дальние расстояния
/// </summary>
public class LoadingOverlayController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image backgroundPanel;
    [SerializeField] private Image runnerImage;
    [SerializeField] private Animator runnerAnimator;
    [SerializeField] private RuntimeAnimatorController runnerController;

    [Header("Settings")]
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.8f);
    [SerializeField] [Range(0.1f, 2f)] private float animationSpeed = 0.5f;
    [SerializeField] private float minimumDisplayTime = 1f;

    private float showTimestamp = 0f;

    private void Awake()
    {
        // Настройка компонентов
        if (backgroundPanel != null)
        {
            backgroundPanel.color = backgroundColor;
            backgroundPanel.raycastTarget = true;
        }

        if (runnerImage != null)
        {
            runnerImage.raycastTarget = false;
            
            // Получить Animator если не назначен
            if (runnerAnimator == null)
            {
                runnerAnimator = runnerImage.GetComponent<Animator>();
                if (runnerAnimator == null)
                {
                    runnerAnimator = runnerImage.gameObject.AddComponent<Animator>();
                }
            }
        }

        // Настройка Animator
        if (runnerAnimator != null && runnerController != null)
        {
            runnerAnimator.runtimeAnimatorController = runnerController;
            runnerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            runnerAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        showTimestamp = Time.unscaledTime;
        
        if (runnerAnimator != null && runnerController != null)
        {
            runnerAnimator.runtimeAnimatorController = runnerController;
            runnerAnimator.enabled = true;
            runnerAnimator.speed = animationSpeed;
            runnerAnimator.Play("runner_run", -1, 0f);
        }
    }

    public System.Collections.IEnumerator HideAsync()
    {
        float elapsed = Time.unscaledTime - showTimestamp;
        if (elapsed < minimumDisplayTime)
        {
            yield return new WaitForSecondsRealtime(minimumDisplayTime - elapsed);
        }

        if (runnerAnimator != null)
        {
            runnerAnimator.enabled = false;
        }
        
        gameObject.SetActive(false);
    }

    public void HideImmediate()
    {
        if (runnerAnimator != null)
        {
            runnerAnimator.enabled = false;
        }
        gameObject.SetActive(false);
    }

    public bool IsVisible => gameObject.activeSelf;
}
