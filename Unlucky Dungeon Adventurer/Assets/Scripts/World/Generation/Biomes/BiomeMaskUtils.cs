public static class BiomeMaskUtils
{
    // Битовые значения по 8 направлениям
    private const byte TOP = 2;
    private const byte BOTTOM = 8;
    private const byte LEFT = 16;
    private const byte RIGHT = 32;
    private const byte TOP_LEFT = 1;
    private const byte TOP_RIGHT = 4;
    private const byte BOT_LEFT = 64;
    private const byte BOT_RIGHT = 128;

    public static byte GetMask(Func<int, int, string> biomeGetter, int x, int y, string centerBiome)
    {
        byte mask = 0;

        void Check(int dx, int dy, byte value)
        {
            string b = biomeGetter(x + dx, y + dy);
            if (b != centerBiome && b != null) mask |= value;
        }

        Check(0, 1, TOP);
        Check(0, -1, BOTTOM);
        Check(-1, 0, LEFT);
        Check(1, 0, RIGHT);

        Check(-1, 1, TOP_LEFT);
        Check(1, 1, TOP_RIGHT);
        Check(-1, -1, BOT_LEFT);
        Check(1, -1, BOT_RIGHT);

        return mask;
    }
}
