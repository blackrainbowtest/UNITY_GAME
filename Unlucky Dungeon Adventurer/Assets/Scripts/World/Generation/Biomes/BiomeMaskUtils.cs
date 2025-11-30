using System;

public static class BiomeMaskUtils
{
    private const byte UP    = 1;
    private const byte RIGHT = 2;
    private const byte DOWN  = 4;
    private const byte LEFT  = 8;

    private const byte UP_LEFT    = 16;
    private const byte UP_RIGHT   = 32;
    private const byte DOWN_LEFT  = 64;
    private const byte DOWN_RIGHT = 128;

    /// <summary>
    /// Вычисляет маску для autotile системы.
    /// Бит = 1 означает что в этом направлении такой же биом (союзник).
    /// Бит = 0 означает что там другой биом (враг).
    /// 
    /// targetBiome - биом, для которого рисуем tileset (обычно edgeBiome/dominant)
    /// </summary>
    public static byte GetMask(Func<int,int,string> biomeGetter, int x, int y, string targetBiome)
    {
        byte mask = 0;

        // Same = true если в этом направлении такой же биом как targetBiome
        bool Same(int dx, int dy) =>
            biomeGetter(x + dx, y + dy) == targetBiome;

        // Устанавливаем биты ТАМ ГДЕ СОЮЗНИКИ (одинаковый биом)
        if (Same(0, 1))   mask |= UP;          // Сверху такой же биом
        if (Same(1, 0))   mask |= RIGHT;       // Справа такой же биом
        if (Same(0, -1))  mask |= DOWN;        // Снизу такой же биом
        if (Same(-1, 0))  mask |= LEFT;        // Слева такой же биом

        if (Same(-1, 1))  mask |= UP_LEFT;     // Сверху-слева такой же биом
        if (Same(1, 1))   mask |= UP_RIGHT;    // Сверху-справа такой же биом
        if (Same(-1, -1)) mask |= DOWN_LEFT;   // Снизу-слева такой же биом
        if (Same(1, -1))  mask |= DOWN_RIGHT;  // Снизу-справа такой же биом

        return mask;
    }
}
