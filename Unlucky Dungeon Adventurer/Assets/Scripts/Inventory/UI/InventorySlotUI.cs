/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   InventorySlotUI.cs                                   /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:33:32 by UDA                                      */
/*   Updated: 2025/12/01 13:33:32 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI quantityText;

    private ItemInstance _item;
    public ItemInstance Item => _item;

    public void SetItem(ItemInstance inst)
    {
        _item = inst;

        icon.enabled = true;
        icon.sprite = ItemIconDatabase.Get(inst.id); // мы добавим позже

        // локализация работает:
        GetComponent<TooltipTrigger>().SetText(
            inst.Loc.name,
            inst.Loc.description
        );

        quantityText.text = inst.quantity > 1 ? inst.quantity.ToString() : "";
    }

    public void Clear()
    {
        _item = null;
        icon.enabled = false;
        quantityText.text = "";

        GetComponent<TooltipTrigger>().Clear();
    }
}
