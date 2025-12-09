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
    [SerializeField] private Image backgroundPanel;     // Темный фон (полупрозрачный черный)
    [SerializeField] private Image runnerImage;         // Анимированная иконка бегуна 512x512
    [SerializeField] private Animator runnerAnimator;   // Animator для проигрывания runner_run

    [Header("Appearance Settings")]
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.8f); // Темный полупрозрачный фон
    [SerializeField] [Range(0.1f, 2f)] private float animationSpeed = 0.5f;    // Скорость анимации (0.5 = в 2 раза медленнее)
    [SerializeField] private float minimumDisplayTime = 1f;                     // Минимальное время показа (секунды)

    private float showTimestamp = 0f; // Время когда показали overlay

    private void Awake()
    {
        // Настроить фон
        if (backgroundPanel != null)
        {
            backgroundPanel.color = backgroundColor;
            backgroundPanel.raycastTarget = true; // Блокировать клики под overlay
        }

        // Убедиться что Animator установлен и настроен
        if (runnerAnimator == null && runnerImage != null)
        {
            runnerAnimator = runnerImage.GetComponent<Animator>();
        }

        if (runnerAnimator != null)
        {
            // Анимация работает даже при Time.timeScale = 0
            runnerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        if (runnerImage != null)
        {
            runnerImage.raycastTarget = false; // Не блокировать клики (только фон блокирует)
        }

        // По умолчанию скрыт
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Показать оверлей и запустить анимацию
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        showTimestamp = Time.time; // Запомнить время показа
        
        if (runnerAnimator != null)
        {
            runnerAnimator.enabled = true;
            runnerAnimator.speed = animationSpeed; // Установить скорость анимации
            // Анимация запустится автоматически через controller
        }
    }

    /// <summary>
    /// Скрыть оверлей и остановить анимацию
    /// Асинхронно: гарантирует минимальное время показа
    /// </summary>
    public System.Collections.IEnumerator HideAsync()
    {
        // Подождать минимальное время показа
        float elapsedTime = Time.time - showTimestamp;
        if (elapsedTime < minimumDisplayTime)
        {
            yield return new WaitForSeconds(minimumDisplayTime - elapsedTime);
        }

        // Скрыть overlay
        if (runnerAnimator != null)
        {
            runnerAnimator.enabled = false;
        }
        
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Скрыть оверлей мгновенно (без минимального времени)
    /// </summary>
    public void HideImmediate()
    {
        if (runnerAnimator != null)
        {
            runnerAnimator.enabled = false;
        }
        
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Проверка: активен ли оверлей
    /// </summary>
    public bool IsVisible => gameObject.activeSelf;
}
