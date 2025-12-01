/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   ItemActionWindow.cs                                  /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 16:38:35 by UDA                                      */
/*   Updated: 2025/12/01 16:38:35 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemActionWindow : MonoBehaviour
{
    public static ItemActionWindow Instance;

    [Header("UI")]
    public GameObject panel;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;

    [Header("Кнопки")]
    public Button useButton;
    public Button equipButton;
    public Button dropButton;
    public Button closeButton;

    private ItemInstance _item;
    private InventorySlotUI _slot;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);

        closeButton.onClick.AddListener(() => Close());
        dropButton.onClick.AddListener(() => DropItem());
        useButton.onClick.AddListener(() => UseItem());
        equipButton.onClick.AddListener(() => EquipItem());
    }

    public void Open(ItemInstance item, InventorySlotUI slot)
    {
        _item = item;
        _slot = slot;

        if (item == null)
        {
            Close();
            return;
        }

        panel.SetActive(true);

        icon.sprite = ItemIconDatabase.Get(item.id);
        nameText.text = item.Loc.name;
        descText.text = item.Loc.description;

        // Логика кнопок
        SetupButtons();
    }

    private void SetupButtons()
    {
        string type = _item.Def.type;

        useButton.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);

        if (type == "consumable")
            useButton.gameObject.SetActive(true);

        if (type == "weapon" || type == "armor" || type == "bag")
            equipButton.gameObject.SetActive(true);

        // drop всегда доступен
        dropButton.gameObject.SetActive(true);
    }

    private void UseItem()
    {
        InventoryController.Instance.UseItem(_item);
        Close();
    }

    private void EquipItem()
    {
        EquipmentController.Instance.Equip(_item);
        Close();
    }

    private void DropItem()
    {
        // Удаляем один предмет из стака (или весь, если нестакаемый)
        int amountToRemove = _item.IsStackable ? 1 : _item.quantity;
        InventoryController.Instance.RemoveItem(_item, amountToRemove);
        Close();
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}
