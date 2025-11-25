using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class SaveSlotUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI Elements")]
    public TextMeshProUGUI slotTitle;
    public TextMeshProUGUI slotInfo;
    public TextMeshProUGUI playerNameText;     // Имя персонажа
    public TextMeshProUGUI levelText;          // Уровень
    public Image background;
    public GameObject deletePrompt;            // Контейнер для "удалить?" при удержании
    public TextMeshProUGUI deletePromptText;   // Текст "удалить?"

    private string filePath;
    private bool isAutoSave;
    private float holdTime = 0f;
    private const float holdDuration = 0.5f;   // 0.5 сек удержания
    private bool isDeletionPromptShown = false;

    // Режим работы меню (Save / Load)
    public static bool isSaveMode = false;

    public void Init(string path, bool autoSave = false)
    {
        filePath = path;
        isAutoSave = autoSave;
        isDeletionPromptShown = false;

        if (deletePrompt != null)
            deletePrompt.SetActive(false);

        // Diagnostic: ensure UI references are assigned to avoid NullReferenceException
        if (slotTitle == null) Debug.LogError("[SaveSlotUI] slotTitle is not assigned in the inspector.");
        if (slotInfo == null) Debug.LogError("[SaveSlotUI] slotInfo is not assigned in the inspector.");
        if (playerNameText == null) Debug.LogError("[SaveSlotUI] playerNameText is not assigned in the inspector.");
        if (levelText == null) Debug.LogError("[SaveSlotUI] levelText is not assigned in the inspector.");
        if (background == null) Debug.LogError("[SaveSlotUI] background Image is not assigned in the inspector.");

        bool exists = File.Exists(path);

        // ---------- Заголовок ----------
        if (slotTitle != null)
        {
            slotTitle.text = autoSave
                ? LanguageManager.Get("auto_save")
                : Path.GetFileNameWithoutExtension(path);
        }

        // ---------- Имя персонажа + уровень ----------
        if (exists)
        {
            try
            {
                SaveData data = SaveManager.Load(GetSlotIndex());

                if (data != null)
                {
                    if (playerNameText != null)
                    {
                        string autosaveLabel = isAutoSave ? LanguageManager.Get("auto_save") + " " : "";
                        playerNameText.text = autosaveLabel + data.player.name;
                    }
                    if (levelText != null) levelText.text = $"Lvl {data.player.level} | Gold: {data.player.gold}";
                }
                else
                {
                    if (playerNameText != null) playerNameText.text = "---";
                    if (levelText != null) levelText.text = "";
                }
            }
            catch
            {
                playerNameText.text = "---";
                levelText.text = "";
            }
        }
        else
        {
            if (playerNameText != null) playerNameText.text = "";
            if (levelText != null) levelText.text = "";
        }

        // ---------- Подпись под слотом ----------
        if (exists)
        {
            string date = File.GetLastWriteTime(path).ToString("dd.MM.yyyy HH:mm");

            if (slotInfo != null)
            {
                string mainLabel = SaveLoadState.Mode == SaveLoadMode.Save
                    ? $"{LanguageManager.Get("button_save")}: {date}"
                    : $"{LanguageManager.Get("button_load")}: {date}";

                // Add location and gold info if available
                SaveData data = SaveManager.Load(GetSlotIndex());
                string location = data != null ? LanguageManager.Get("location") + ": " + data.meta.sceneName : "";
                string gold = data != null ? LanguageManager.Get("gold") + ": " + data.player.gold : "";

                slotInfo.text = mainLabel
                    + (string.IsNullOrEmpty(location) ? "" : "\n" + location)
                    + (string.IsNullOrEmpty(gold) ? "" : ", " + gold);
            }

            if (background != null)
                background.color = Color.white;
        }
        else
        {
            if (slotInfo != null)
                slotInfo.text = LanguageManager.Get("empty_slot");

            if (background != null)
                background.color = new Color(1, 1, 1, 0.25f);
        }

        // ---------- Удаление ----------
        if (isAutoSave || !exists)
            deletePrompt?.SetActive(false);

        // ---------- Клик ----------
        Button btn = GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogError("[SaveSlotUI] No Button component found on the SaveSlot prefab. Attach a Button or update the prefab.");
        }
        else
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnSlotClick);
        }
    }

    private void OnSlotClick()
    {
        bool exists = File.Exists(filePath);
        Debug.Log($"[SaveSlotUI] Click on slot '{filePath}', mode={SaveLoadState.Mode}, exists={exists}");

        if (SaveLoadState.Mode == SaveLoadMode.Save)
            Save();
        else
            Load();
    }

    private void Save()
    {
        int index = GetSlotIndex();              // -1 для автосейва, 0..N для обычных
        Debug.Log($"[SaveSlotUI] SAVE to slot {index}, path={filePath}");

        SaveData data = GameManager.Instance.GetCurrentGameData();
        SaveManager.Save(data, index);          // ВНУТРИ SaveManager.Save должен создать файл

        // После сохранения обновляем отображение даты и прозрачность
        Init(filePath);
    }

    private void Load()
    {
        if (!File.Exists(filePath))
        {
            Debug.Log("Слот пуст — загрузить нечего");
            return;
        }

        SaveData data = SaveManager.Load(GetSlotIndex());

        if (data == null)
        {
            Debug.LogError("Ошибка загрузки сейва!");
            return;
        }

        // передаём сейв
        TempSaveCache.pendingSave = data;

        // Загружаем сцену, в которой был игрок
        SceneLoader.LoadScene(data.meta.sceneName);
    }

    private void OnDelete()
    {
        if (isAutoSave)
        {
            Debug.Log("Автосейв нельзя удалить.");
            return;
        }

        if (File.Exists(filePath))
            File.Delete(filePath);

        Init(filePath);
    }

    private int GetSlotIndex()
    {
        if (isAutoSave)
            return -1;

        string fileName = Path.GetFileNameWithoutExtension(filePath);

        // Ожидаем формат save_0, save_1, ...
        if (!fileName.StartsWith("save_"))
            return -1;  // чтобы не падало

        string num = fileName.Substring(5); // всё после "save_"

        if (int.TryParse(num, out int index))
            return index;

        Debug.LogError($"[SaveSlotUI] Неверный формат имени файла: {fileName}");
        return -1;
    }

    // ===== HOLD-TO-DELETE IMPLEMENTATION =====
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isAutoSave || File.Exists(filePath) == false)
            return;

        holdTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDeletionPromptShown = false;
        if (deletePrompt != null)
            deletePrompt.SetActive(false);
    }

    private void Update()
    {
        if (isAutoSave || File.Exists(filePath) == false)
            return;

        // Проверяем, нажата ли кнопка мыши на этом объекте
        if (EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0))
        {
            holdTime += Time.deltaTime;

            if (holdTime >= holdDuration && !isDeletionPromptShown)
            {
                isDeletionPromptShown = true;
                if (deletePrompt != null)
                {
                    deletePrompt.SetActive(true);
                    if (deletePromptText != null)
                        deletePromptText.text = LanguageManager.Get("delete_save");
                }
            }

            // Показываем прогресс (опционально - может быть визуальная шкала)
        }
        else
        {
            holdTime = 0f;
        }
    }
}
