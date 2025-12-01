/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   InventoryUIController.cs                             /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:33:13 by UDA                                      */
/*   Updated: 2025/12/01 13:33:13 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance;

    public GameObject panel;
    public Transform slotsParent;
    public InventorySlotUI slotPrefab;
    public TextMeshProUGUI goldText;

    private void Awake()
    {
        Instance = this;
    }

    public void Open()
    {
        panel.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    public void Refresh()
    {
        foreach (Transform child in slotsParent)
            Destroy(child.gameObject);

        var items = InventoryController.Instance.Items;
        int capacity = InventoryController.Instance.GetCapacity();

        for (int i = 0; i < capacity; i++)
        {
            var slot = Instantiate(slotPrefab, slotsParent);

            if (i < items.Count)
                slot.SetItem(items[i]);
            else
                slot.Clear();
        }

        goldText.text = GameManager.Instance.GetCurrentGameData().player.gold.ToString();
    }
}
