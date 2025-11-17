using System.Collections.Generic;

[System.Serializable]
public class ClassProgressionData
{
    public Dictionary<string, ClassProgressionEntry> classes;
}

[System.Serializable]
public class ClassProgressionEntry
{
    public int hpPerLevel;
    public int mpPerLevel;
    public int staminaPerLevel;

    public int attackPerLevel;
    public int defensePerLevel;
    public int agilityPerLevel;
    public int lustPerLevel;
}
