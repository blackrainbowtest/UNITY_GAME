/* ************************************************************************** */
/*                                                                            */
/*                                                                            */
/*   MovementTimeController.cs                            /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/01 13:03:47 by UDA                                      */
/*   Updated: 2025/12/01 13:03:47 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

public static class MovementTimeController
{
    public static void ApplyTime(WorldSaveData world, int minutes)
    {
        world.AddMinutes(minutes);
    }
}
