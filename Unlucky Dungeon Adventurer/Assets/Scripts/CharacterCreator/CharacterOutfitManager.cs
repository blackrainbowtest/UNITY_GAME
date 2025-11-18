using UnityEngine;
using UnityEngine.UI;

public class CharacterOutfitManager : MonoBehaviour
{
    [Header("Character Layers")]
    public Image legs;

    [Header("Leg Sprites")]
    public Sprite legsPaladin;
    public Sprite legsRogue;
    public Sprite legsSlave;
    public Sprite legsHermit;

    // Метод вызывается, когда игрок выбирает вариант из списка
    public void ApplySet(string setName)
    {
        switch (setName)
        {
            case "Paladin":
                legs.sprite = legsPaladin;
                break;
            case "Rogue":
                legs.sprite = legsRogue;
                break;
            case "Slave":
                legs.sprite = legsSlave;
                break;
            case "Hermit":
                legs.sprite = legsHermit;
                break;
            default:
                legs.sprite = null;
                break;
        }
    }
}
