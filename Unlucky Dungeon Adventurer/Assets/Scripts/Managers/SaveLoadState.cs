/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/Managers/SaveLoadState.cs                           */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 09:25:08 by UDA                                      */
/*   Updated: 2025/12/03 09:25:08 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

public enum SaveLoadMode
{
    Save,
    Load
}

public static class SaveLoadState
{
    public static SaveLoadMode Mode = SaveLoadMode.Load;
}
