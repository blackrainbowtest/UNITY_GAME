using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string playerClass;

    // 🔹 Прогресс
    public int level = 1;
    public int gold = 0;

    // 🔹 БАЗОВЫЕ статы (то, что задаёт класс + уровень)
    public int baseMaxHP;
    public int baseMaxMP;
    public int baseMaxStamina;

    public int baseAttack;
    public int baseDefense;
    public int baseAgility;
    public int baseLust;

    // 🔹 ФИНАЛЬНЫЕ статы (база + шмот + бафы)
    // Пока = базе. Потом добавим экипировку.
    public int finalMaxHP;
    public int finalMaxMP;
    public int finalMaxStamina;

    public int finalAttack;
    public int finalDefense;
    public int finalAgility;
    public int finalLust;

    // 🔹 ТЕКУЩИЕ значения (то, что реально на полосках)
    public int currentHP;
    public int currentMP;
    public int currentStamina;

    // Прочее
    public int isPregnant;

    // Позиция на карте (для сейва)
    public float mapPosX;
    public float mapPosY;

    public PlayerData(string name, string playerClass, ClassStats stats)
    {
        this.playerName = name;
        this.playerClass = playerClass;

        ApplyBaseStatsFromClass(stats);
        RecalculateFinalStats();
        ResetToFull();
    }

    /// <summary>
    /// Берём базовые статы из шаблона класса (classDatabase)
    /// </summary>
    public void ApplyBaseStatsFromClass(ClassStats stats)
    {
        baseMaxHP = stats.baseHP;
        baseMaxMP = stats.baseMP;
        baseMaxStamina = 100; // временно фиксированное значение

        baseAttack = stats.baseAttack;
        baseDefense = stats.baseDefense;
        baseAgility = stats.baseAgility;
        baseLust = stats.baseLust;

        isPregnant = stats.baseIsPregnant;
    }

    /// <summary>
    /// Пересчёт ФИНАЛЬНЫХ статов.
    /// Сейчас: финальные = базовые.
    /// Потом сюда добавим шмот, бафы, дебафы.
    /// </summary>
    public void RecalculateFinalStats()
    {
        finalMaxHP = baseMaxHP;
        finalMaxMP = baseMaxMP;
        finalMaxStamina = baseMaxStamina;

        finalAttack = baseAttack;
        finalDefense = baseDefense;
        finalAgility = baseAgility;
        finalLust = baseLust;

        // Если текущее HP/MP/STA больше нового максимума — режем
        if (currentHP > finalMaxHP) currentHP = finalMaxHP;
        if (currentMP > finalMaxMP) currentMP = finalMaxMP;
        if (currentStamina > finalMaxStamina) currentStamina = finalMaxStamina;
    }

    /// <summary>
    /// Полное восстановление (используем при создании или отдыхе)
    /// </summary>
    public void ResetToFull()
    {
        currentHP = finalMaxHP;
        currentMP = finalMaxMP;
        currentStamina = finalMaxStamina;
    }
}
