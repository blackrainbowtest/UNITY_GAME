/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   RestCalculator.cs                                    /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 12:44:39 by UDA                                      */
/*   Updated: 2025/12/01 12:44:39 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public static class RestCalculator
{
    public static float GetEnvironmentMultiplier(RestEnvironment env)
    {
        return env switch
        {
            RestEnvironment.Field => 0.50f,
            RestEnvironment.Tent => 1.00f,
            RestEnvironment.Village => 1.50f,
            RestEnvironment.City => 2.50f,
            _ => 1f
        };
    }

    public static int GetRestMinutes(RestType type)
    {
        return type switch
        {
            RestType.ShortRest => 30,
            RestType.Meditation => 60,
            RestType.LongSleep => 240,
            _ => 0
        };
    }

    public static void ApplyRest(PlayerSaveData player, RestType type, RestEnvironment env)
    {
        float mult = GetEnvironmentMultiplier(env);

        switch (type)
        {
            case RestType.ShortRest:
                PlayerStatsController.Instance.ModifyStamina(
                    Mathf.RoundToInt(player.baseMaxStamina * 0.20f * mult)
                );
                break;

            case RestType.Meditation:
                PlayerStatsController.Instance.ModifyMP(
                    Mathf.RoundToInt(player.baseMaxMP * 0.50f * mult)
                );
                PlayerStatsController.Instance.ModifyStamina(
                    Mathf.RoundToInt(player.baseMaxStamina * 0.10f * mult)
                );
                break;

            case RestType.LongSleep:
                PlayerStatsController.Instance.ModifyHP(
                    Mathf.RoundToInt(player.baseMaxHP * 1.00f * mult)
                );
                PlayerStatsController.Instance.ModifyMP(
                    Mathf.RoundToInt(player.baseMaxMP * 1.00f * mult)
                );
                PlayerStatsController.Instance.ModifyStamina(
                    Mathf.RoundToInt(player.baseMaxStamina * 1.00f * mult)
                );
                break;
        }
    }
}
