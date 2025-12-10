using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovementUIController : MonoBehaviour
{
    private void OnEnable()
    {
        UIEvents.OnPathPreview += OnPathPreview;
        UIEvents.OnMovementStarted += OnMovementStarted;
        UIEvents.OnMovementEnded += OnMovementEnded;
        UIEvents.OnRestAvailable += OnRestAvailable;
        UIEvents.OnNotEnoughStamina += OnNotEnoughStamina;
    }

    private void OnDisable()
    {
        UIEvents.OnPathPreview -= OnPathPreview;
        UIEvents.OnMovementStarted -= OnMovementStarted;
        UIEvents.OnMovementEnded -= OnMovementEnded;
        UIEvents.OnRestAvailable -= OnRestAvailable;
        UIEvents.OnNotEnoughStamina -= OnNotEnoughStamina;
    }

    private void OnNotEnoughStamina()
    {
        if (pathInfoText != null)
            pathInfoText.text = LanguageManager.Get("not_enough_stamina_for_path");
    }

    [Header("Верхняя панель")]
    public TextMeshProUGUI pathInfoText;

    [Header("Кнопки слева")]
    public Button walkButton;
    public Button restButton;

    [Header("Ссылка на контроллер движения")]
    public PlayerMovementController movementController;

    private void Start()
    {
        walkButton.onClick.AddListener(OnWalkClicked);
        restButton.onClick.AddListener(OnRestClicked);

        // Make buttons visible but disabled by default
        walkButton.gameObject.SetActive(true);
        walkButton.interactable = false;
        
        // Rest button always visible, enabled based on stamina
        restButton.gameObject.SetActive(true);
        UpdateRestButtonState();
        
        if (pathInfoText != null)
            pathInfoText.text = "";

    }

    private void UpdateRestButtonState()
    {
        var player = GameData.CurrentPlayer;
        if (player == null)
        {
            restButton.interactable = false;
            return;
        }

        // Enable rest button only if stamina is not full
        restButton.interactable = player.currentStamina < player.finalMaxStamina;
    }

    private void OnPathPreview(int staminaCost, int timeMinutes, bool hasEnough)
    {
        // Загружаем UI-переводы перед использованием ключей
        LanguageManager.LoadLanguage("ui_save_load");

        if (pathInfoText != null)
        {
            int hours = timeMinutes / 60;
            int mins = timeMinutes % 60;
            pathInfoText.text = LanguageManager.GetFormat("path_info_text", staminaCost, hours, mins);
        }

        walkButton.gameObject.SetActive(true);
        walkButton.interactable = hasEnough;
    }

    private void OnMovementStarted()
    {
        // Keep walk button visible but disable interaction during movement
        walkButton.gameObject.SetActive(true);
        walkButton.interactable = false;
        // Optionally hide restButton while moving
    }

    private void OnMovementEnded()
    {
        // инфу можно оставить, а можно очистить
        // pathInfoText.text = "";
    }

    private void OnRestAvailable(bool canRest)
    {
        // Keep button visible, just update interactability
        restButton.gameObject.SetActive(true);
        restButton.interactable = canRest;
    }

    private void OnWalkClicked()
    {
        movementController.StartWalk();
    }

    private void OnRestClicked()
    {
        var player = GameData.CurrentPlayer;
        if (player == null) return;

        Vector2Int coords = new Vector2Int(
            Mathf.RoundToInt(player.mapPosX),
            Mathf.RoundToInt(player.mapPosY)
        );

        RestEnvironment env = RestEnvironmentDetector.GetEnvironment(coords);

        if (RestUIManager.Instance == null) return;

        RestUIManager.Instance.OpenRestMenu(env);
    }
}
