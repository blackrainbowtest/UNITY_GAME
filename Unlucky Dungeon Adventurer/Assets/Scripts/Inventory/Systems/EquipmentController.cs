/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   EquipmentController.cs                               /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:32:43 by UDA                                      */
/*   Updated: 2025/12/01 13:32:43 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    public static EquipmentController Instance;

    public ItemInstance head;
    public ItemInstance body;
    public ItemInstance ring1;
    public ItemInstance ring2;
    public ItemInstance amulet;
    public ItemInstance bag;

    private void Awake()
    {
        Instance = this;
    }

    public bool Equip(ItemInstance inst)
    {
        switch (inst.Def.type)
        {
            case "head":
                head = inst; return true;

            case "body":
                body = inst; return true;

            case "ring":
                if (ring1 == null) { ring1 = inst; return true; }
                if (ring2 == null) { ring2 = inst; return true; }
                return false;

            case "amulet":
                amulet = inst; return true;

            case "bag":
                bag = inst; return true;
        }

        return false;
    }

    public void Unequip(ItemInstance inst)
    {
        if (head == inst) head = null;
        else if (body == inst) body = null;
        else if (ring1 == inst) ring1 = null;
        else if (ring2 == inst) ring2 = null;
        else if (amulet == inst) amulet = null;
        else if (bag == inst) bag = null;
    }
}
