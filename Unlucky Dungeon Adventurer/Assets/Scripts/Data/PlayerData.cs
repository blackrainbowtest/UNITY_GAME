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

    public int experience;
    public int experienceToNext;

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
    
    // World seed for procedural generation (saved with player)
    public int worldSeed;

    // Свойства для удобства (используют финальные значения)
    public int maxHP => finalMaxHP;
    public int maxMP => finalMaxMP;
    public int maxStamina => finalMaxStamina;

    public PlayerData(string name, string playerClass, ClassStats stats)
    {
        this.playerName = name;
        this.playerClass = playerClass;
        
        // assign a world seed for this player
        worldSeed = Random.Range(0, 99999999);

        // базовые статы от класса
        ApplyBaseStatsFromClass(stats);

        // опыт
        experience = 0;
        experienceToNext = ExperienceTable.GetExpRequiredForLevel(1);

        // высчитываем финальные значения (пока == базовым)
        RecalculateFinalStats();

        // устанавливаем current HP/MP/STA = максимальным
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

    /// <summary>
    /// Добавляет опыт персонажу.
    /// </summary>
    public void GainExperience(int amount)
    {
        experience += amount;

        while (experience >= experienceToNext)
        {
            experience -= experienceToNext;
            LevelUp();
        }
    }

    /// <summary>
    /// Повышает уровень персонажа.
    /// </summary>
    private void LevelUp()
    {
        level++;

        // Получаем правила роста для текущего класса
        var prog = ClassProgressionManager.Data[playerClass];

        // растим базовые статы по данным JSON
        baseMaxHP      += prog.hpPerLevel;
        baseMaxMP      += prog.mpPerLevel;
        baseMaxStamina += prog.staminaPerLevel;

        baseAttack  += prog.attackPerLevel;
        baseDefense += prog.defensePerLevel;
        baseAgility += prog.agilityPerLevel;
        baseLust    += prog.lustPerLevel;

        // пересчитываем финальные
        RecalculateFinalStats();

        // новый порог опыта
        experienceToNext = ExperienceTable.GetExpRequiredForLevel(level);

        Debug.Log($"LEVEL UP! {playerName} теперь уровня {level}");
    }

}
