using System.IO;
using UnityEngine;
using TMPro;

public class SaveLoadSceneManager : MonoBehaviour
{
    public Transform slotContainer;     // Content из ScrollView
    public GameObject slotPrefab;       // Prefab SaveSlotUI
    public TextMeshProUGUI titleText;

    private void Awake()
    {
        LanguageManager.LoadLanguage("ui_save_load");
        titleText.text = LanguageManager.Get("title");

        GenerateSlots();
    }

    private void GenerateSlots()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        // 1. автосейв
        string autoPath = Path.Combine(Application.persistentDataPath, "save_auto.json");
        var autoObj = Instantiate(slotPrefab, slotContainer);
        autoObj.GetComponent<SaveSlotUI>().Init(autoPath, true);

        // 2. обычные сохранения
        string[] files = Directory.GetFiles(Application.persistentDataPath, "save_*.json");
        Debug.Log($"[SaveLoadScene] Found {files.Length} save files");

        int index = 0;
        foreach (string path in files)
        {
            var obj = Instantiate(slotPrefab, slotContainer);
            obj.GetComponent<SaveSlotUI>().Init(path);
            index++;
        }

        // 3. если файлов нет — создаём пустой слот
        if (files.Length == 0)
        {
            Debug.Log("[SaveLoadScene] Creating empty slot");
            string emptyPath = Path.Combine(Application.persistentDataPath, "save_0.json");
            var obj = Instantiate(slotPrefab, slotContainer);
            obj.GetComponent<SaveSlotUI>().Init(emptyPath);
        }
    }
}