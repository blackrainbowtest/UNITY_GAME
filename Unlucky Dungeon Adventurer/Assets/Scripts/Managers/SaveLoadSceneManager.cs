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

        // убираем автосейв
        files = System.Array.FindAll(files, f => !f.Contains("auto"));

        foreach (string path in files)
        {
            var obj = Instantiate(slotPrefab, slotContainer);
            obj.GetComponent<SaveSlotUI>().Init(path);
        }

        // ----- EMPTY SLOTS (always show in SAVE mode for new saves) -----
        if (SaveLoadState.Mode == SaveLoadMode.Save)
        {
            int slotsToShow = 10;  // Show 10 slots total
            for (int i = files.Length; i < slotsToShow; i++)
            {
                string emptyPath = Path.Combine(Application.persistentDataPath, $"save_{i}.json");
                var obj = Instantiate(slotPrefab, slotContainer);
                obj.GetComponent<SaveSlotUI>().Init(emptyPath);
            }
        }
    }
}