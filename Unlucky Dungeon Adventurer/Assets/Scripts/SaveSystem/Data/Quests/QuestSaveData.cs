/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/SaveSystem/Data/Quests/QuestSaveData.cs             */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 16:58:43 by UDA                                      */
/*   Updated: 2025/12/03 16:58:43 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using System;
using System.Collections.Generic;

/// <summary>
/// Quest progression block.
/// </summary>
[Serializable]
public class QuestSaveData
{
    public List<string> active    = new List<string>();
    public List<string> completed = new List<string>();
}
