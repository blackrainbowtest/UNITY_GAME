using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string playerName;
    public string playerClass;
    public int level;
    public int gold;
    public Vector2 mapPosition;

    // Можно добавить в будущем
    // public QuestData quests;
    // public EnemyStatsData kills;
}