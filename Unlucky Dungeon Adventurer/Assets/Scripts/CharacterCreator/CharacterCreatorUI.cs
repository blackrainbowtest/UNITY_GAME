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
    // Class options configured in the inspector
    public ClassOption[] classOptions;

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

    // Populate dropdown with each option's displayName
        foreach (var opt in classOptions)
        {
            optionLabels.Add(opt.displayName);
        }

        dropdownBackground.AddOptions(optionLabels);

    // Ensure default selected index is 0 if options exist
        if (classOptions.Length > 0)
            dropdownBackground.value = 0;
    }

    public void OnCreateButton()
    {
        string name = inputName.text.Trim();

        if (string.IsNullOrWhiteSpace(name))
            name = "Airin";

        string roleId = classOptions[dropdownBackground.value].internalName;

        int seedToUse = Mathf.Max(10000, selectedSeed);

        // Create first save
        SaveService.CreateNewGame(name, roleId, seedToUse);

        // Load WorldMap
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
