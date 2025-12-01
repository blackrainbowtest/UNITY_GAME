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

public class InventorySlotUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI quantityText;

    private ItemInstance _item;
    private TooltipTrigger _tooltip;

    private bool _pressed;
    private float _pressTime;
    private const float LONG_PRESS_TIME = 0.35f;

    private void Awake()
    {
        _tooltip = GetComponent<TooltipTrigger>();
    }

    public ItemInstance Item => _item;

    public void SetItem(ItemInstance inst)
    {
        _item = inst;

        icon.enabled = inst != null;
        quantityText.text = inst != null && inst.quantity > 1 ? inst.quantity.ToString() : "";

        _tooltip.SetItem(inst);
        if (inst != null)
            icon.sprite = ItemIconDatabase.Get(inst.id);
    }

    public void Clear()
    {
        _item = null;
        icon.enabled = false;
        quantityText.text = "";
        _tooltip.Clear();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_item == null) return;

        _pressed = true;
        _pressTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
    }

    private void Update()
    {
        if (_pressed && !DragManager.Instance.IsDragging)
        {
            if (Time.time - _pressTime >= LONG_PRESS_TIME)
            {
                // старт drag
                DragManager.Instance.StartDrag(_item, this);
                _tooltip.HideImmediate();
                _pressed = false;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (DragManager.Instance.IsDragging)
        {
            DragManager.Instance.PlaceToSlot(this);
            return;
        }

        // обычный клик → tooltip
        if (_item != null)
            _tooltip.OnClick();
    }
}
