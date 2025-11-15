using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI slotTitle;
    public TextMeshProUGUI slotInfo;
    public Image background;
    public Button deleteButton;  // 🗑 кнопка удаления

    private string filePath;
    private bool isAutoSave;

    // Режим работы меню (Save / Load)
    public static bool isSaveMode = false;

    public void Init(string path, bool autoSave = false)
    {
        filePath = path;
        isAutoSave = autoSave;

        // ----- Имя слота -----
        string name = autoSave ? LanguageManager.Get("auto_save")
                               : Path.GetFileNameWithoutExtension(path);
        slotTitle.text = name;

        bool exists = File.Exists(path);

        // ----- Текст под слотом -----
        if (exists)
        {
            string date = File.GetLastWriteTime(path).ToString("dd.MM.yyyy HH:mm");

            if (SaveLoadState.Mode == SaveLoadMode.Save)
                slotInfo.text = $"{LanguageManager.Get("button_save")}: {date}";
            else
                slotInfo.text = $"{LanguageManager.Get("button_load")}: {date}";

            background.color = Color.white;
        }
        else
        {
            slotInfo.text = LanguageManager.Get("empty_slot");
            background.color = new Color(1, 1, 1, 0.25f); // прозрачный для пустых
        }

        // ----- Клик по всему слоту -----
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnSlotClick);

        // ----- Кнопка удаления -----
        deleteButton.onClick.RemoveAllListeners();

        if (isAutoSave)
        {
            deleteButton.gameObject.SetActive(false); // автосейв нельзя удалить
        }
        else
        {
            deleteButton.gameObject.SetActive(true);
            deleteButton.onClick.AddListener(OnDelete);
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

        int index = GetSlotIndex();
        Debug.Log($"[SaveSlotUI] LOAD from slot {index}, path={filePath}");

        SaveData data = SaveManager.Load(index);
        GameManager.Instance.LoadGameData(data);
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
        string[] parts = fileName.Split('_');
        return int.Parse(parts[1]);
    }
}
