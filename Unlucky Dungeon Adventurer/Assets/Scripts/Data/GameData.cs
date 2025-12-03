/* ************************************************************************** */
/*                                                                            */
/*   File: Assets/Scripts/Data/GameData.cs                                    */
/*                                                        /\_/\               */
/*                                                       ( •.• )              */
/*   By: unluckydungeonadventure.gmail.com                > ^ <               */
/*                                                                            */
/*   Created: 2025/12/03 09:21:39 by UDA                                      */
/*   Updated: 2025/12/03 09:21:39 by UDA                                      */
/*                                                                            */
/* ************************************************************************** */

using UnityEngine;
using System.Collections.Generic;

public static class GameData
{
    public static PlayerData CurrentPlayer;

    // 📜 Таблица всех классов
    public static Dictionary<string, ClassStats> classDatabase = new Dictionary<string, ClassStats>()
    {
        { "Paladin", new ClassStats
            {
                className = "Paladin",
                baseHP = 150,
                baseMP = 40,
                baseAttack = 20,
                baseDefense = 15,
                baseAgility = 5,
                baseLust = 10,
                baseIsPregnant = 0
            }
        },
        { "Rogue", new ClassStats
            {
                className = "Rogue",
                baseHP = 100,
                baseMP = 30,
                baseAttack = 25,
                baseDefense = 8,
                baseAgility = 15,
                baseLust = 20,
                baseIsPregnant = 0
            }
        },
        { "Slave", new ClassStats
            {
                className = "Slave",
                baseHP = 80,
                baseMP = 20,
                baseAttack = 15,
                baseDefense = 5,
                baseAgility = 10,
                baseLust = 30,
                baseIsPregnant = 0
            }
        },
        { "Hermit", new ClassStats
            {
                className = "Hermit",
                baseHP = 120,
                baseMP = 60,
                baseAttack = 10,
                baseDefense = 10,
                baseAgility = 8,
                baseLust = 5,
                baseIsPregnant = 0
            }
        },
    };

    // 💾 Сохраняем игрока
    // If 'seed' is provided, use it when creating the player; otherwise create as before.
    public static void SavePlayer(string name, string role, int? seed = null)
    {
        Debug.Log($"[SavePlayer] Создан игрок: {name}, класс={role}");
        Debug.Log($"[SavePlayer] CurrentPlayer теперь: {CurrentPlayer != null}");

        if (!classDatabase.ContainsKey(role))
        {
            Debug.LogWarning($"Неизвестный класс: {role}");
            return;
        }

        ClassStats stats = classDatabase[role];

        // создаём нового игрока на основе класса
        if (CurrentPlayer == null)
        {
            if (seed.HasValue)
                CurrentPlayer = new PlayerData(name, role, stats, seed.Value);
            else
                CurrentPlayer = new PlayerData(name, role, stats);
        }
        else
        {
            // If a CurrentPlayer already exists (rare), just update name/class and always set seed if provided
            CurrentPlayer.playerName = name;
            CurrentPlayer.playerClass = role;
            CurrentPlayer.ApplyBaseStatsFromClass(stats);
            CurrentPlayer.RecalculateFinalStats();
            if (seed.HasValue)
                CurrentPlayer.worldSeed = seed.Value;
        }

        // тут в будущем можно задать стартовый уровень/золото
        CurrentPlayer.level = 1;
        CurrentPlayer.gold = 500;

        // на всякий случай пересчитываем (чисто паранойя)
        CurrentPlayer.RecalculateFinalStats();
        CurrentPlayer.ResetToFull();

        // Сохраняем минимальные вещи в PlayerPrefs (для быстрого доступа)
        PlayerPrefs.SetString("playerName", name);
        PlayerPrefs.SetString("playerClass", role);
        // Сохраняем worldSeed чтобы при загрузке из PlayerPrefs мир был тем же
        PlayerPrefs.SetInt("worldSeed", CurrentPlayer.worldSeed);
        PlayerPrefs.Save();
    }


    // 📂 Загружаем игрока
    public static void LoadPlayer()
    {
        string name = PlayerPrefs.GetString("playerName", "Unknown");
        string role = PlayerPrefs.GetString("playerClass", "None");

        if (classDatabase.ContainsKey(role))
            CurrentPlayer = new PlayerData(name, role, classDatabase[role]);
        else
            CurrentPlayer = new PlayerData(name, role, new ClassStats());

        // Если в PlayerPrefs есть сохранённый seed — восстановим его (иначе остаётся случайный)
        int storedSeed = PlayerPrefs.GetInt("worldSeed", -1);
        if (storedSeed >= 0)
        {
            CurrentPlayer.worldSeed = storedSeed;
            Debug.Log($"[GameData] Loaded worldSeed from PlayerPrefs: {storedSeed}");
        }
    }
}
