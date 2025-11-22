using UnityEngine;
using TMPro;

public class CharacterCreatorUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField inputName;
    public TMP_Dropdown dropdownBackground;
    public TMP_InputField inputSeed;

    private int selectedSeed = 10000;

    [Header("Class Options")]
    public ClassOption[] classOptions; // настраиваем в инспекторе

    private void Awake()
    {
        SetupDropdown();
        InitializeSeed();
    }

    private void InitializeSeed()
    {
        // Generate a default seed (ensure >= 10000)
        selectedSeed = Random.Range(10000, 99999999);
        if (inputSeed != null)
            inputSeed.text = selectedSeed.ToString();
    }

    private void SetupDropdown()
    {
        dropdownBackground.ClearOptions();

        var optionLabels = new System.Collections.Generic.List<string>();

        // Заполняем дропдаун displayName-ами
        foreach (var opt in classOptions)
        {
            optionLabels.Add(opt.displayName);
        }

        dropdownBackground.AddOptions(optionLabels);

        // На всякий случай выставляем выбор на 0
        if (classOptions.Length > 0)
            dropdownBackground.value = 0;
    }

    public void OnCreateButton()
    {
        string name = inputName.text;

        int index = dropdownBackground.value;

        if (index < 0 || index >= classOptions.Length)
        {
            Debug.LogError($"[CREATE] Некорректный индекс класса: {index}");
            return;
        }

        // 👇 вот ЭТО критичный момент:
        string roleInternal = classOptions[index].internalName;
        string roleDisplay  = classOptions[index].displayName;

        Debug.Log($"[CREATE BUTTON CLICKED] Создан герой: {name}, класс: {roleDisplay} (ID: {roleInternal})");

        GameData.SavePlayer(name, roleInternal);

        // Apply the selected seed to the newly created player so world generation is deterministic
        if (GameData.CurrentPlayer != null)
        {
            GameData.CurrentPlayer.worldSeed = Mathf.Max(10000, selectedSeed);
            Debug.Log($"[CREATE] Assigned worldSeed = {GameData.CurrentPlayer.worldSeed}");
        }
        SceneLoader.LoadScene("WorldMap");
    }

    // Called by the seed input field on end edit / value change
    public void OnSeedInputChanged(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            selectedSeed = 10000;
            if (inputSeed != null) inputSeed.text = selectedSeed.ToString();
            return;
        }

        if (int.TryParse(text, out var val))
        {
            if (val < 10000) val = 10000;
            selectedSeed = val;
            if (inputSeed != null) inputSeed.text = selectedSeed.ToString();
        }
        else
        {
            // not a valid int -> reset to minimum
            selectedSeed = 10000;
            if (inputSeed != null) inputSeed.text = selectedSeed.ToString();
        }
    }
}
