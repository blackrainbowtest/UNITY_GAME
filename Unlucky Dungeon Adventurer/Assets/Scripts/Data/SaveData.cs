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

    // В будущем:
    // public KillStatsSaveData killStats = new KillStatsSaveData();
    // public AchievementsSaveData achievements = new AchievementsSaveData();
}

// -------------------- Игрок --------------------
[Serializable]
public class PlayerSaveData
{
    public string name;
    public string playerClass;
    public int level;
    public int gold;

    public int hp;
    public int mp;
    public int attack;
    public int defense;
    public int agility;
    public int lust;
    public int isPregnant;

    // позиция на карте мира (пока плоско)
    public float mapPosX;
    public float mapPosY;
}

// -------------------- Мир --------------------
[Serializable]
public class WorldSaveData
{
    public int worldSeed;
    public int currentDay;
    public float timeOfDay;

    // В будущем: открытые подземелья, города и т.п.
    // public List<Vector2Int> discoveredDungeons = new List<Vector2Int>();
    // public List<Vector2Int> discoveredCities = new List<Vector2Int>();
}

// -------------------- Инвентарь --------------------
[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items = new List<ItemSaveData>();
}

// Пока заглушка, потом привяжем к твоей системе предметов
[Serializable]
public class ItemSaveData
{
    public string itemId;   // например "sword_wooden_001"
    public int quantity;
}

// -------------------- Квесты --------------------
[Serializable]
public class QuestSaveData
{
    public List<string> active = new List<string>();
    public List<string> completed = new List<string>();

    // В будущем сюда можно добавлять прогресс, таймеры и т.п.
}

// -------------------- Мета-инфа про сейв --------------------
[Serializable]
public class MetaSaveData
{
    public int slotIndex;         // -1 = автосейв, 0..N = обычные слоты
    public string sceneName;      // "WorldMap", "Dungeon_01" и т.п.
    public string saveTime;       // строка даты/времени для отображения
    public string saveVersion;    // версия игры/схемы сейва
}
