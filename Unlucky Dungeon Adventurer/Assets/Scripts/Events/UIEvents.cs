using System;

public static class UIEvents
{
    /// <summary>
    /// Вызывается, если попытка построить маршрут превышает доступную стамину.
    /// </summary>
    public static Action OnNotEnoughStamina;
    // Вызывается, когда игра полностью инициализирована
    public static event Action OnGameInitialized;

    // Вызывается, когда игрок загружен из сохрана
    public static event Action OnPlayerLoaded;

    // Вызывается, когда меняются статы игрока (здоровье, голд, опыт и т.п.)
    public static event Action OnPlayerStatsChanged;

    /// <summary>
    /// Вызывается при выборе пути по клику.
    /// staminaCost - сколько стамины нужно.
    /// timeMinutes - сколько минут займёт путь.
    /// hasEnoughStamina - хватает ли сейчас стамины.
    /// </summary>
    public static Action<int, int, bool> OnPathPreview;

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

    /// <summary>
    /// Движение по пути началось.
    /// </summary>
    public static Action OnMovementStarted;

    /// <summary>
    /// Движение по пути закончено (успешно или прервано).
    /// </summary>
    public static Action OnMovementEnded;

    /// <summary>
    /// Можно ли сейчас показывать кнопку отдыха.
    /// </summary>
    public static Action<bool> OnRestAvailable;
}
