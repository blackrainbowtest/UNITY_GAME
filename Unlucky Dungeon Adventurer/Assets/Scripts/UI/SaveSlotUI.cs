using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI slotTitle;
    public TextMeshProUGUI slotInfo;
    public Button loadButton;
    public Button deleteButton;
    public Image background;

    private string filePath;
    private bool isAutoSave;

    public void Init(string path, bool autoSave = false)
    {
        filePath = path;
        isAutoSave = autoSave;

        string name = autoSave ? LanguageManager.Get("auto_save") : Path.GetFileNameWithoutExtension(path);
        slotTitle.text = name;

        if (File.Exists(path))
        {
            string date = File.GetLastWriteTime(path).ToString("dd.MM.yyyy HH:mm");
            slotInfo.text = $"{LanguageManager.Get("button_load")}: {date}";
            background.color = new Color(1, 1, 1, 1);
        }
        else
        {
            slotInfo.text = LanguageManager.Get("empty_slot");
            background.color = new Color(1, 1, 1, 0.25f); // прозрачный для пустых
        }

        loadButton.GetComponentInChildren<TextMeshProUGUI>().text = LanguageManager.Get("button_load");
        deleteButton.GetComponentInChildren<TextMeshProUGUI>().text = LanguageManager.Get("button_delete");

        loadButton.onClick.AddListener(OnLoad);
        deleteButton.onClick.AddListener(OnDelete);
    }

    private void OnLoad()
    {
        if (!File.Exists(filePath))
        {
            SaveData data = GameManager.Instance.GetCurrentGameData();
            SaveManager.Save(data, GetSlotIndex());
            Init(filePath);
        }
        else
        {
            SaveData data = SaveManager.Load(GetSlotIndex());
            GameManager.Instance.LoadGameData(data);
        }
    }

    private void OnDelete()
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
        Init(filePath);
    }

    private int GetSlotIndex()
    {
        if (isAutoSave)
            return -1; // особый случай
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string[] parts = fileName.Split('_');
        return int.Parse(parts[1]);
    }
}
