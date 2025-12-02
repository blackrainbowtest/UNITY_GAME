/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   InventoryController.cs                               /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:32:26 by UDA                                      */
/*   Updated: 2025/12/01 13:32:26 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    private SaveData Save => GameManager.Instance.GetCurrentGameData();
    private PlayerSaveData Player => Save.player;

    public int baseCapacity = 16;

    private void Awake()
    {
        Instance = this;
        if (Player.inventoryItems == null)
            Player.inventoryItems = new List<ItemInstance>();
    }

    public List<ItemInstance> Items => Player.inventoryItems;

    public int GetCapacity()
    {
        int bonus = 0;

        foreach (var it in Items)
        {
            var def = it.Def;
            if (def.type == "bag")
                bonus += def.capacityBonus;
        }

        return baseCapacity + bonus;
    }

    public bool AddItem(string id, int amount = 1)
    {
        var def = ItemDatabase.Instance.Get(id);

        // 1 — Добавление в существующий стак
        if (def.maxStack > 1)
        {
            foreach (var inst in Items)
            {
                if (inst.id == id && inst.quantity < def.maxStack)
                {
                    int canAdd = def.maxStack - inst.quantity;
                    int add = Mathf.Min(canAdd, amount);

                    inst.quantity += add;
                    amount -= add;

                    if (amount <= 0)
                        return true;
                }
            }
        }

        // 2 — Добавление новых стаков
        while (amount > 0)
        {
            if (Items.Count >= GetCapacity())
                return false;

            int stackSize = Mathf.Min(def.maxStack, amount);
            Items.Add(new ItemInstance { id = id, quantity = stackSize });
            amount -= stackSize;
        }

        return true;
    }

    public void SortInventory(InventorySort.SortMode mode)
    {
        var sorted = InventorySort.Sort(Player.inventoryItems, mode);

        Player.inventoryItems = sorted;

        InventoryUIController.Instance.Refresh();
    }

    public void RemoveItem(ItemInstance inst, int amount)
    {
        inst.quantity -= amount;
        if (inst.quantity <= 0)
            Items.Remove(inst);
    }

    public bool UseItem(ItemInstance inst)
    {
        if (inst == null) return false;
        var def = inst.Def;
        if (def == null) return false;
        if (def.type != "consumable")
        {
            Debug.LogWarning($"[Inventory] Попытка использовать не consumable: {def.id}");
            return false;
        }

        // Применяем эффекты
        var effects = def.effects;
        if (effects != null)
        {
            if (effects.hp != 0) PlayerStatsController.Instance.ModifyHP(effects.hp);
            if (effects.stamina != 0) PlayerStatsController.Instance.ModifyStamina(effects.stamina);
            if (effects.mana != 0) PlayerStatsController.Instance.ModifyMP(effects.mana);
        }

        // Уменьшаем количество
        RemoveItem(inst, 1);

        // Обновляем UI инвентаря если открыт
        if (InventoryUIController.Instance != null)
            InventoryUIController.Instance.Refresh();

        return true;
    }
}
