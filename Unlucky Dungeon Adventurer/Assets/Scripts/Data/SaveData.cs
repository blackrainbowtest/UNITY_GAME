using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public PlayerSaveData player = new PlayerSaveData();
    public WorldSaveData world = new WorldSaveData();
    public InventorySaveData inventory = new InventorySaveData();
    public QuestSaveData quests = new QuestSaveData();
    public MetaSaveData meta = new MetaSaveData();

    // � �������:
    // public KillStatsSaveData killStats = new KillStatsSaveData();
    // public AchievementsSaveData achievements = new AchievementsSaveData();
}

// -------------------- ����� --------------------
[Serializable]
public class PlayerSaveData
{
    public string name;
    public string playerClass;

    public int level;
    public int gold;
    public int worldSeed;

    public int experience;       // 🔥
    public int experienceToNext; // 🔥

    // 🔹 БАЗОВЫЕ статы (то, что задаёт класс/уровень)
    public int baseMaxHP;
    public int baseMaxMP;
    public int baseMaxStamina;

    public int baseAttack;
    public int baseDefense;
    public int baseAgility;
    public int baseLust;

    // 🔹 ТЕКУЩИЕ значения (то, что реально на полосках)
    public int currentHP;
    public int currentMP;
    public int currentStamina;

    public int isPregnant;

    // позиция игрока на карте
    public float mapPosX;
    public float mapPosY;
}

// -------------------- ��� --------------------
[Serializable]
public class WorldSaveData
{
    public int worldSeed;
    public int currentDay;
    public float timeOfDay;

    // � �������: �������� ����������, ������ � �.�.
    // public List<Vector2Int> discoveredDungeons = new List<Vector2Int>();
    // public List<Vector2Int> discoveredCities = new List<Vector2Int>();
}

// -------------------- ��������� --------------------
[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items = new List<ItemSaveData>();
}

// ���� ��������, ����� �������� � ����� ������� ���������
[Serializable]
public class ItemSaveData
{
    public string itemId;   // �������� "sword_wooden_001"
    public int quantity;
}

// -------------------- ������ --------------------
[Serializable]
public class QuestSaveData
{
    public List<string> active = new List<string>();
    public List<string> completed = new List<string>();

    // � ������� ���� ����� ��������� ��������, ������� � �.�.
}

// -------------------- ����-���� ��� ���� --------------------
[Serializable]
public class MetaSaveData
{
    public int slotIndex;         // -1 = ��������, 0..N = ������� �����
    public string sceneName;      // "WorldMap", "Dungeon_01" � �.�.
    public string saveTime;       // ������ ����/������� ��� �����������
    public string saveVersion;    // ������ ����/����� �����
    public string currentBiome;   // Биом, на котором стоит персонаж
}
