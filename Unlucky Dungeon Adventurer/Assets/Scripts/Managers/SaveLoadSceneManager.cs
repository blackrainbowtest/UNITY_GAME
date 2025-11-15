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

        if (SaveLoadState.Mode == SaveLoadMode.Save)
            titleText.text = LanguageManager.Get("title_save");
        else
            titleText.text = LanguageManager.Get("title_load");

        GenerateSlots();
    }

    private void GenerateSlots()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        // ----- AUTOSAVE -----
        if (SaveLoadState.Mode == SaveLoadMode.Load)
        {
            string autoPath = Path.Combine(Application.persistentDataPath, "save_auto.json");
            var autoObj = Instantiate(slotPrefab, slotContainer);
            autoObj.GetComponent<SaveSlotUI>().Init(autoPath, true);
        }

        // ----- REGULAR SAVES -----
        string[] files = Directory.GetFiles(Application.persistentDataPath, "save_*.json");

        foreach (string path in files)
        {
            var obj = Instantiate(slotPrefab, slotContainer);
            obj.GetComponent<SaveSlotUI>().Init(path);
        }

        // ----- EMPTY SLOT (only in SAVE mode when no saves) -----
        if (SaveLoadState.Mode == SaveLoadMode.Save && files.Length == 0)
        {
            string emptyPath = Path.Combine(Application.persistentDataPath, "save_0.json");
            var obj = Instantiate(slotPrefab, slotContainer);
            obj.GetComponent<SaveSlotUI>().Init(emptyPath);
        }
    }
}