using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCreatorUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField inputName;
    public TMP_Dropdown dropdownBackground;
    public Image outfitLayer;
    public Image accessoryLayer;

    [Header("Sprites")]
    public Sprite rogueOutfit;
    public Sprite slaveOutfit;
    public Sprite paladinOutfit;
    public Sprite desertOutfit;
    public Sprite backpackAccessory;

    private void Start()
    {
        dropdownBackground.onValueChanged.AddListener(OnBackgroundChanged);
    }

    public void OnBackgroundChanged(int index)
    {
        switch (index)
        {
            case 0: // Плут
                outfitLayer.sprite = rogueOutfit;
                accessoryLayer.sprite = null;
                break;
            case 1: // Бывший раб
                outfitLayer.sprite = slaveOutfit;
                accessoryLayer.sprite = null;
                break;
            case 2: // Ученик паладина
                outfitLayer.sprite = paladinOutfit;
                accessoryLayer.sprite = null;
                break;
            case 3: // Дитя пустыни
                outfitLayer.sprite = desertOutfit;
                accessoryLayer.sprite = backpackAccessory;
                break;
        }
    }

    public void OnCreateButton()
    {
        Debug.Log($"Создан герой: {inputName.text}, роль: {dropdownBackground.options[dropdownBackground.value].text}");
        // потом добавим переход на карту
    }
}
