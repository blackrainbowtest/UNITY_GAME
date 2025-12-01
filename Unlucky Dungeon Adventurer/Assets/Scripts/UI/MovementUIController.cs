using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovementUIController : MonoBehaviour
{
    [Header("Верхняя панель")]
    public TextMeshProUGUI pathInfoText;

    [Header("Кнопки слева")]
    public Button walkButton;
    public Button restButton;

    [Header("Ссылка на контроллер движения")]
    public PlayerMovementController movementController;

    private void OnEnable()
    {
        UIEvents.OnPathPreview += OnPathPreview;
        UIEvents.OnMovementStarted += OnMovementStarted;
        UIEvents.OnMovementEnded += OnMovementEnded;
        UIEvents.OnRestAvailable += OnRestAvailable;
    }

    private void OnDisable()
    {
        UIEvents.OnPathPreview -= OnPathPreview;
        UIEvents.OnMovementStarted -= OnMovementStarted;
        UIEvents.OnMovementEnded -= OnMovementEnded;
        UIEvents.OnRestAvailable -= OnRestAvailable;
    }

    private void Start()
    {
        walkButton.onClick.AddListener(OnWalkClicked);
        restButton.onClick.AddListener(OnRestClicked);

        walkButton.gameObject.SetActive(false);
        restButton.gameObject.SetActive(false);
        if (pathInfoText != null)
            pathInfoText.text = "";
    }

    private void OnPathPreview(int staminaCost, int timeMinutes, bool hasEnough)
    {
        if (pathInfoText != null)
        {
            int hours = timeMinutes / 60;
            int mins = timeMinutes % 60;
            pathInfoText.text = $"Путь: стамина {staminaCost}, время {hours:D2}:{mins:D2}";
        }

        walkButton.gameObject.SetActive(true);
        walkButton.interactable = hasEnough;
    }

    private void OnMovementStarted()
    {
        walkButton.gameObject.SetActive(false);
        // можно временно прятать restButton
    }

    private void OnMovementEnded()
    {
        // инфу можно оставить, а можно очистить
        // pathInfoText.text = "";
    }

    private void OnRestAvailable(bool canRest)
    {
        restButton.gameObject.SetActive(canRest);
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

        if (RestUIController.Instance != null)
            RestUIController.Instance.Open(env);
    }
}
