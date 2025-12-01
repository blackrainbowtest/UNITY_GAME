/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   DragManager.cs                                       /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 14:41:55 by UDA                                      */
/*   Updated: 2025/12/01 14:41:55 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance;

    [Header("UI")]
    public Image floatingIcon;
    public Canvas canvas;

    private ItemInstance _dragItem;
    private InventorySlotUI _sourceSlot;

    private void Awake()
    {
        Instance = this;
        floatingIcon.gameObject.SetActive(false);
    }

    public bool IsDragging => _dragItem != null;

    public void StartDrag(ItemInstance item, InventorySlotUI sourceSlot)
    {
        _dragItem = item;
        _sourceSlot = sourceSlot;

        floatingIcon.sprite = ItemIconDatabase.Get(item.id);
        floatingIcon.gameObject.SetActive(true);
    }

    public void StopDrag()
    {
        _dragItem = null;
        _sourceSlot = null;
        floatingIcon.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!IsDragging) return;

        // перемещение floatingIcon под палец
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out pos
        );
        floatingIcon.rectTransform.anchoredPosition = pos;
    }

    public void PlaceToSlot(InventorySlotUI targetSlot)
    {
        if (_dragItem == null || _sourceSlot == null)
            return;

        // если источник = цель → отмена
        if (targetSlot == _sourceSlot)
        {
            StopDrag();
            return;
        }

        var targetItem = targetSlot.Item;

        // 1) Стак одинаковых предметов
        if (targetItem != null && targetItem.id == _dragItem.id && targetItem.IsStackable)
        {
            int freeSpace = targetItem.Def.maxStack - targetItem.quantity;
            if (freeSpace > 0)
            {
                int moveAmount = Mathf.Min(freeSpace, _dragItem.quantity);
                targetItem.quantity += moveAmount;
                _dragItem.quantity -= moveAmount;

                if (_dragItem.quantity == 0)
                    InventoryController.Instance.Items.Remove(_dragItem);
            }
        }
        else
        {
            // 2) Поменять местами
            _sourceSlot.SetItem(targetItem);
            targetSlot.SetItem(_dragItem);

            // обновляем данные в Save
            var list = InventoryController.Instance.Items;
            int a = list.IndexOf(_dragItem);
            int b = list.IndexOf(targetItem);

            if (a >= 0 && b >= 0)
            {
                list[a] = targetItem;
                list[b] = _dragItem;
            }
        }

        InventoryUIController.Instance.Refresh();
        StopDrag();
    }
}
