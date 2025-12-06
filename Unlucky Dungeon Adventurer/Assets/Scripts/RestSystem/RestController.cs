/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   RestController.cs                                    /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 12:45:58 by UDA                                      */
/*   Updated: 2025/12/01 12:45:58 by UDA                                      */
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

    public void StartRest(RestType type, RestEnvironment env)
    {
        int minutes = RestCalculator.GetRestMinutes(type);

        // Определить событие
        RestEvent e = RestEventResolver.RollEvent(type, env);

           if (e.type == RestEventType.None)
           {
               // нормальный отдых
               World.AddMinutes(minutes);
               RestCalculator.ApplyRest(Player, type, env);
 
               UIEvents.InvokePlayerStatsChanged();
               return;
           }

           if (e.type == RestEventType.Noise)
           {
               int penalizedMinutes = Mathf.RoundToInt(minutes * e.penaltyMultiplier);
               World.AddMinutes(penalizedMinutes);
               RestCalculator.ApplyRest(Player, type, env);
 
               UIEvents.InvokePlayerStatsChanged();
               return;
           }

           if (e.type == RestEventType.MinorAmbush)
           {
               // UI-взаимодействие
               return;
           }

           if (e.type == RestEventType.MajorAmbush)
           {
               // SceneManager.LoadScene("BattleScene");
               return;
           }
    }
}
