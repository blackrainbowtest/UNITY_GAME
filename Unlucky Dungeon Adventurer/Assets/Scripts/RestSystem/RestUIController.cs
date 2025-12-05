/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   RestUIController.cs                                  /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 12:46:17 by UDA                                      */
/*   Updated: 2025/12/01 12:46:17 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestUIController : MonoBehaviour
{
    public static RestUIController Instance;

    [Header("Основные UI элементы")]
    public GameObject panel;               // Корневой объект окна отдыха
    public TextMeshProUGUI titleText;      // Заголовок (динамический)
    
    [Header("Кнопки выбора")]
    public Button shortRestButton;
    public Button meditationButton;
    public Button longSleepButton;

    private RestEnvironment _currentEnvironment;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Check references before use
        if (shortRestButton != null && meditationButton != null && longSleepButton != null)
        {
            shortRestButton.onClick.AddListener(() => Choose(RestType.ShortRest));
            meditationButton.onClick.AddListener(() => Choose(RestType.Meditation));
            longSleepButton.onClick.AddListener(() => Choose(RestType.LongSleep));
        }

        // Hide panel by default if assigned
        if (panel != null)
        {
            panel.SetActive(false);

            // Allow closing by clicking on the background
            var bgImage = panel.GetComponent<UnityEngine.UI.Image>();
            var bgButton = panel.GetComponent<UnityEngine.UI.Button>();
            if (bgButton == null)
            {
                bgButton = panel.AddComponent<UnityEngine.UI.Button>();
                bgButton.transition = UnityEngine.UI.Selectable.Transition.None;
                bgButton.targetGraphic = bgImage; // reuse background image if exists
            }

            bgButton.onClick.RemoveAllListeners();
            bgButton.onClick.AddListener(Close);
        }
    }

    /// <summary>
    /// Открывает окно отдыха и устанавливает доступные значения.
    /// </summary>
    public void Open(RestEnvironment env)
    {
        if (panel == null || titleText == null)
        {
            Debug.LogError("[RestUI] panel/titleText not assigned on RestUIController");
            return;
        }

        _currentEnvironment = env;

        // Заголовок в зависимости от того, где игрок отдыхает
        titleText.text = env switch
        {
            RestEnvironment.Field   => "Отдых на открытой местности",
            RestEnvironment.Tent    => "Отдых в палатке",
            RestEnvironment.Village => "Отдых в деревне",
            RestEnvironment.City    => "Отдых в гостинице",
            _ => "Отдых"
        };

        panel.SetActive(true);
        // Bring on top
        panel.transform.SetAsLastSibling();

        // Ensure any CanvasGroup (if added later) is visible
        var cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        Debug.Log("[RestUI] Open called, panel activated");
    }

    /// <summary>
    /// Выбор типа отдыха.
    /// </summary>
    private void Choose(RestType type)
    {
        if (RestController.Instance == null)
        {
            Debug.LogError("[RestUI] RestController.Instance is null!");
            return;
        }

        RestController.Instance.StartRest(type, _currentEnvironment);
        Close();
    }

    /// <summary>
    /// Закрывает окно.
    /// </summary>
    public void Close()
    {
        panel.SetActive(false);
    }
}

