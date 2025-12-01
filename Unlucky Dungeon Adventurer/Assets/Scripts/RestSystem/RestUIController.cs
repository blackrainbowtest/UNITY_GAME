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
        // Кнопки привязаны один раз
        shortRestButton.onClick.AddListener(() => Choose(RestType.ShortRest));
        meditationButton.onClick.AddListener(() => Choose(RestType.Meditation));
        longSleepButton.onClick.AddListener(() => Choose(RestType.LongSleep));

        // Окно скрыто по умолчанию
        panel.SetActive(false);
    }

    /// <summary>
    /// Открывает окно отдыха и устанавливает доступные значения.
    /// </summary>
    public void Open(RestEnvironment env)
    {
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
    }

    /// <summary>
    /// Выбор типа отдыха.
    /// </summary>
    private void Choose(RestType type)
    {
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

