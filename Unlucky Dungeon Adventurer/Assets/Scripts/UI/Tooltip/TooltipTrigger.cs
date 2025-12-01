/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   TooltipTrigger.cs                                    /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 14:26:59 by UDA                                      */
/*   Updated: 2025/12/01 14:26:59 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerClickHandler
{
    private ItemInstance _item;

    // чтобы знать, открыт tooltip или нет
    private bool _isOpen = false;

	public void OnClick()
	{
		if (_item == null) return;

		ItemActionWindow.Instance.Open(_item, GetComponent<InventorySlotUI>());
	}

    public void SetItem(ItemInstance item)
    {
        _item = item;
        _isOpen = false;
    }

    public void Clear()
    {
        _item = null;
        _isOpen = false;
    }

	public void HideImmediate()
	{
		TooltipController.Instance.Hide();
		_isOpen = false;
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_item == null)
            return;

        if (_isOpen)
        {
            TooltipController.Instance.Hide();
            _isOpen = false;
            return;
        }

        TooltipController.Instance.Show(
            _item.Loc.name,
            _item.Loc.description,
            transform.position
        );

        _isOpen = true;
    }
}
