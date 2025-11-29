using System;

public static class BiomeMaskUtils
{
    // Bit flags for 8-direction autotiling
    private const byte N  = 1;    // y+1
    private const byte S  = 2;    // y-1
    private const byte W  = 4;    // x-1
    private const byte E  = 8;    // x+1

    private const byte NW = 16;   // x-1, y+1
    private const byte NE = 32;   // x+1, y+1
    private const byte SW = 64;   // x-1, y-1
    private const byte SE = 128;  // x+1, y-1

    public static byte GetMask(Func<int,int,string> biomeGetter, int x, int y, string centerBiome)
    {
        byte mask = 0;

        bool Diff(int dx, int dy)
        {
            return biomeGetter(x + dx, y + dy) != centerBiome;
        }

        // Cardinal
        if (Diff(0, 1))  mask |= N;
        if (Diff(0, -1)) mask |= S;
        if (Diff(-1, 0)) mask |= W;
        if (Diff(1, 0))  mask |= E;

        // Diagonals
        if (Diff(-1, 1))  mask |= NW;
        if (Diff(1, 1))   mask |= NE;
        if (Diff(-1, -1)) mask |= SW;
        if (Diff(1, -1))  mask |= SE;

        return mask;
    }
}
