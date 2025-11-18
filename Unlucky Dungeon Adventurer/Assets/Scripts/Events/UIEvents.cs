using System;

public static class UIEvents
{
    /// <summary>
    /// Срабатывает когда статы игрока изменились
    /// </summary>
    public static Action OnPlayerStatsChanged;

    /// <summary>
    /// Срабатывает когда игра полностью инициализирована и готова к работе
    /// </summary>
    public static Action OnGameInitialized;
}
