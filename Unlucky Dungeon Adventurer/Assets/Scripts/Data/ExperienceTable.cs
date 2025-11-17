using UnityEngine;

public static class ExperienceTable
{
    // Формула опыта: классическая RPG формула
    // Можно настроить попозже
    public static int GetExpRequiredForLevel(int level)
    {
        // примерная формула как в JRPG:
        // EXP = 100 * level^1.5

        return Mathf.RoundToInt(100f * Mathf.Pow(level, 1.5f));
    }
}
