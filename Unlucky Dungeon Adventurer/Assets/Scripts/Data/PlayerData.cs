using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string playerClass;
    public int level = 1;
    public int gold = 0;

    // 🧩 Основные характеристики
    public int hp;
    public int mp;
    public int attack;
    public int defense;
    public int agility;
    public int lust;
    public int isPregnant;

    // 🏗 Конструктор, который принимает класс со статами
    public PlayerData(string name, string playerClass, ClassStats stats)
    {
        this.playerName = name;
        this.playerClass = playerClass;

        // начальные характеристики из шаблона класса
        hp = stats.baseHP;
        mp = stats.baseMP;
        attack = stats.baseAttack;
        defense = stats.baseDefense;
        agility = stats.baseAgility;
        lust = stats.baseLust;
        isPregnant = stats.baseIsPregnant;
    }
}
