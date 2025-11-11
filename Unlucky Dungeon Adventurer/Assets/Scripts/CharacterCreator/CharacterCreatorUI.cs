using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCreatorUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField inputName;
    public TMP_Dropdown dropdownBackground;
    public Image legsLayer;
    public Button createButton; // ← новая ссылка на кнопку

    [Header("Legs Sprites")]
    public Sprite legsPaladin;
    public Sprite legsRogue;
    public Sprite legsSlave;
    public Sprite legsHermit;

    private void Start()
    {
        // сразу блокируем кнопку
        createButton.interactable = false;

        // следим за вводом имени
        inputName.onValueChanged.AddListener(OnNameChanged);

        // следим за сменой класса
        dropdownBackground.onValueChanged.AddListener(OnBackgroundChanged);
    }

    private void OnNameChanged(string newText)
    {
        // активируем кнопку только если имя не пустое
        createButton.interactable = !string.IsNullOrWhiteSpace(newText);
    }

    public void OnBackgroundChanged(int index)
    {
        if (legsLayer == null)
        {
            Debug.LogWarning("Поле legsLayer не назначено в инспекторе!");
            return;
        }

        switch (index)
        {
            case 0:
                legsLayer.sprite = legsPaladin;
                break;
            case 1:
                legsLayer.sprite = legsRogue;
                break;
            case 2:
                legsLayer.sprite = legsSlave;
                break;
            case 3:
                legsLayer.sprite = legsHermit;
                break;
            default:
                legsLayer.sprite = null;
                Debug.LogWarning("Неизвестный индекс варианта: " + index);
                break;
        }
    }

    public void OnCreateButton()
    {
        string role = dropdownBackground.options[dropdownBackground.value].text;
        string name = inputName.text;
        Debug.Log($"[CREATE BUTTON CLICKED] Создан герой: {name}, класс: {role}");

        GameData.SavePlayer(name, role);
        SceneLoader.LoadScene("WorldMap");
    }
}
