/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   RestController.cs                                    /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:04:14 by UDA                                      */
/*   Updated: 2025/12/01 13:04:14 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public class RestController : MonoBehaviour
{
    public static RestController Instance;

    private SaveData Save => GameManager.Instance.GetCurrentGameData();
    private PlayerSaveData Player => Save.player;
    private WorldSaveData World => Save.world;

    private void Awake()
    {
        Instance = this;
    }

    public void RestSimple(int minutes, int staminaRecover)
    {
        MovementTimeController.ApplyTime(World, minutes);
        PlayerStatsController.Instance.ModifyStamina(staminaRecover);

        UIEvents.OnPlayerStatsChanged?.Invoke();
        UIEvents.OnRestAvailable?.Invoke(false);

        Debug.Log($"[Rest] +{minutes} минут, восстановлено {staminaRecover} стамины");
    }
}
