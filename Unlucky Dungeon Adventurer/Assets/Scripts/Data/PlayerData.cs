using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string playerClass;

    // Уровень и деньги
    public int level = 1;
    public int gold = 0;

    // 🔥 Максимальные характеристики
    public int maxHP;
    public int maxMP;
    public int maxStamina;

    // 🔥 Текущие характеристики
    public int currentHP;
    public int currentMP;
    public int currentStamina;

    // Основные боевые параметры
    public int attack;
    public int defense;
    public int agility;
    public int lust;
    public int isPregnant;

    // Позиция на карте
    public float mapPosX;
    public float mapPosY;

    public PlayerData(string name, string playerClass, ClassStats stats)
    {
        this.playerName = name;
        this.playerClass = playerClass;

        // Максимальные статы
        maxHP = stats.baseHP;
        maxMP = stats.baseMP;
        maxStamina = 100;  // пока фиксированно

        // Текущие статы при создании
        currentHP = maxHP;
        currentMP = maxMP;
        currentStamina = maxStamina;

        // Базовые параметры
        attack = stats.baseAttack;
        defense = stats.baseDefense;
        agility = stats.baseAgility;
        lust = stats.baseLust;
        isPregnant = stats.baseIsPregnant;

        // Позиция героя при создании
        mapPosX = 0f;
        mapPosY = 0f;
    }
}
