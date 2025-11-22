using System;

public static class UIEvents
{
    // Вызывается, когда игра полностью инициализирована
    public static event Action OnGameInitialized;

    // Вызывается, когда игрок загружен из сохрана
    public static event Action OnPlayerLoaded;

    // Вызывается, когда меняются статы игрока (здоровье, голд, опыт и т.п.)
    public static event Action OnPlayerStatsChanged;

    // Методы для вызова событий
    public static void InvokeGameInitialized()
    {
        OnGameInitialized?.Invoke();
    }

    public static void InvokePlayerLoaded()
    {
        OnPlayerLoaded?.Invoke();
    }

    public static void InvokePlayerStatsChanged()
    {
        OnPlayerStatsChanged?.Invoke();
    }
}
