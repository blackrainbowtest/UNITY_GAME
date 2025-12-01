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
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI quantityText;

    private ItemInstance _item;
    public ItemInstance Item => _item;

    private TooltipTrigger _tooltip;

    private void Awake()
    {
        _tooltip = GetComponent<TooltipTrigger>();
    }

    public void SetItem(ItemInstance inst)
    {
        _item = inst;

        icon.enabled = true;
        icon.sprite = ItemIconDatabase.Get(inst.id);

        quantityText.text = inst.quantity > 1 ? inst.quantity.ToString() : "";

        _tooltip.SetItem(inst);
    }

    public void Clear()
    {
        _item = null;

        icon.enabled = false;
        quantityText.text = "";

        _tooltip.Clear();
    }
}