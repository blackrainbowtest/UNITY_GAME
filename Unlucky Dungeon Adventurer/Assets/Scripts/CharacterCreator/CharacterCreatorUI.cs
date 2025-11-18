using UnityEngine;
using TMPro;

public class CharacterCreatorUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField inputName;
    public TMP_Dropdown dropdownBackground;

    [Header("Class Options")]
    public ClassOption[] classOptions; // настраиваем в инспекторе

    private void Awake()
    {
        SetupDropdown();
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
        SceneLoader.LoadScene("WorldMap");
    }
}
