/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   RestEventResolver.cs                                 /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 12:45:39 by UDA                                      */
/*   Updated: 2025/12/01 12:45:39 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;

public static class RestEventResolver
{
    public static RestEvent RollEvent(RestType restType, RestEnvironment env)
    {
        // Чем хуже условия, тем выше шанс событий
        float baseChance = env switch
        {
            RestEnvironment.Field => 0.25f,
            RestEnvironment.Tent => 0.15f,
            RestEnvironment.Village => 0.05f,
            RestEnvironment.City => 0.01f,
            _ => 0.10f
        };

        float roll = Random.value;

        if (roll > baseChance)
        {
            return new RestEvent { type = RestEventType.None };
        }

        // Если событие прошло → проверяем тип
        float typeRoll = Random.value;

        if (typeRoll < 0.30f)
        {
            return new RestEvent { type = RestEventType.Noise, penaltyMultiplier = 0.8f };
        }
        if (typeRoll < 0.60f)
        {
            return new RestEvent { type = RestEventType.MinorAmbush };
        }

        return new RestEvent { type = RestEventType.MajorAmbush };
    }
}
