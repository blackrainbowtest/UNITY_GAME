using UnityEngine;

[CreateAssetMenu(fileName = "TileSet47", menuName = "World/Tileset 47", order = 1)]
public class TileSet47 : ScriptableObject
{
    [Header("Tiles sliced in Unity (0..46)")]
    public Sprite[] tiles;   // 47 спрайтов в точном порядке

    // Статистика встреченных паттернов (для анализа недостижимых комбинаций)
    private static readonly System.Collections.Generic.Dictionary<int,int> FourSideCutUsage = new System.Collections.Generic.Dictionary<int,int>();
    private static readonly System.Collections.Generic.HashSet<byte> ObservedMasks = new System.Collections.Generic.HashSet<byte>();
    private static int totalCalls;

    /// <summary>
    /// Индекс по 8-направленной маске (0..255)
    /// Маппинг для классической 47-tile autotile системы (RPG Maker style)
    /// ВАЖНО: тайлы показывают ВЫРЕЗЫ (где НЕТ союзников в углах)
    /// </summary>
    public int GetTileIndex(byte mask)
    {
        // Биты маски (из BiomeMaskUtils):
        // UP=1, RIGHT=2, DOWN=4, LEFT=8, UP_LEFT=16, UP_RIGHT=32, DOWN_LEFT=64, DOWN_RIGHT=128
        
        if (mask == 0) return 0; // Пустой (нет союзников вообще)

        bool u  = (mask & 1) != 0;    // UP
        bool r  = (mask & 2) != 0;    // RIGHT
        bool d  = (mask & 4) != 0;    // DOWN
        bool l  = (mask & 8) != 0;    // LEFT
        bool ul = (mask & 16) != 0;   // UP_LEFT
        bool ur = (mask & 32) != 0;   // UP_RIGHT
        bool dl = (mask & 64) != 0;   // DOWN_LEFT
        bool dr = (mask & 128) != 0;  // DOWN_RIGHT
        
        int result = 0;

        // ═══════════════════════════════════════════════════════════
        // РУЧНОЙ МАППИНГ - МЕНЯЙТЕ ЧИСЛА ПОД СВОЙ ТАЙЛСЕТ
        // ═══════════════════════════════════════════════════════════

        // [ ]
        if (!ul && !u && !ur && !r && !dr && !d && !dl && !l) { result = 0; return result; }

        // [UL]
        if (ul && !u && !ur && !r && !dr && !d && !dl && !l) { result = 1; return result; }

        // [UR]
        if (!ul && !u && ur && !r && !dr && !d && !dl && !l) { result = 2; return result; }

        // [UL UR]
        if (ul && !u && ur && !r && !dr && !d && !dl && !l) { result = 3; return result; }

        // [DR]
        if (!ul && !u && !ur && !r && dr && !d && !dl && !l) { result = 4; return result; }

        // [UL DR]
        if (ul && !u && !ur && !r && dr && !d && !dl && !l) { result = 5; return result; }

        // [UR DR]
        if (!ul && !u && ur && !r && dr && !d && !dl && !l) { result = 6; return result; }

        // [UL UR DR]
        if (ul && !u && ur && !r && dr && !d && !dl && !l) { result = 7; return result; }

        // [DL]
        if (!ul && !u && !ur && !r && !dr && !d && dl && !l) { result = 8; return result; }

        // [UL DL]
        if (ul && !u && !ur && !r && !dr && !d && dl && !l) { result = 9; return result; }

        // [UR DL]
        if (!ul && !u && ur && !r && !dr && !d && dl && !l) { result = 10; return result; }

        // [UL UR DL]
        if (ul && !u && ur && !r && !dr && !d && dl && !l) { result = 11; return result; }

        // [DL DR]
        if (!ul && !u && !ur && !r && dr && !d && dl && !l) { result = 12; return result; }

        // [UL DL DR]
        if (ul && !u && !ur && !r && dr && !d && dl && !l) { result = 13; return result; }

        // [UR DL DR]
        if (!ul && !u && ur && !r && dr && !d && dl && !l) { result = 14; return result; }

        // [UL UR DL DR]
        if (ul && !u && ur && !r && dr && !d && dl && !l) { result = 15; return result; }

        // [L] (UL DL)
        if (!u && !ur && !r && !dr && !d && l) { result = 16; return result; }

        // [UR L] (UL DL)
        if (!u && ur && !r && !dr && !d && l) { result = 17; return result; }

        // [DR L] (UL DL)
        if (!u && !ur && !r && dr && !d && l) { result = 18; return result; }

        // [UR DR L] (UL DL)
        if (!u && ur && !r && dr && !d && l) { result = 19; return result; }

        // [U] (UL UR)
        if (u && !r && !dr && !d && !dl && !l) { result = 20; return result; }

        // [U DR] (UL UR)
        if (u && dr && !r && !d && !dl && !l) { result = 21; return result; }

        // [U DL] (UL UR)
        if (u && dl && !r && !dr && !d && !l) { result = 22; return result; }

        // [U DL DR] (UL UR)
        if (u && dl && dr && !r && !d && !l) { result = 23; return result; }

        // [R] (UR DR)
        if (!u && !ul && r && !d && !dl && !l) { result = 24; return result; }
        if (!ul && !u && r && dl && !d && !l) { result = 25; return result; }
        if (ul && !u && r && !dl && !d && !l) { result = 26; return result; }
        if (ul && !u && r && dl && !d && !l) { result = 27; return result; }
        if (!ul && !u && !ur && !r && d && !l) { result = 28; return result; }
        if (ul && !u && !ur && !r && d && !l) { result = 29; return result; }
        if (ur && d && !r && !u && !l && !ul) { result = 30; return result; }
        if (ul && ur && d && !r && !u && !l) { result = 31; return result; }
        if (!u && r && !d && !l) { result = 32; return result; }
        if (u && d && !r && !l) { result = 33; return result; }
        if (ul && u && l && !r && !d && !dr) { result = 34; return result; }
        if (ul && u && dr && l && !r && !d) { result = 35; return result; }
        if (u && ur && r && !d && !l && !dl) { result = 36; return result; }
        if (u && ur && r && dl && !d && !l) { result = 37; return result; }
        if (r && dr && d && !u && !l && !ul) { result = 38; return result; }
        if (ul && r && dr && d && !u && !l) { result = 39; return result; }
        if (d && dl && l && !u && !ur && !r && !ur) { result = 40; return result; }
        if (ur && d && dl && l && !u && !r) { result = 41; return result; }
        if (ul && u && ur && r && l && !d) { result = 42; return result; }
        if (ul && u && d && dl && l && !r) { result = 43; return result; }
        if (r && dr && d && dl && l && !u) { result = 44; return result; }
        if (u && ur && r && dr && d && !l) { result = 45; return result; }
        if (ul && u && ur && r && dr && d && dl && l) { result = 46; return result; }

        // Fallback
        ObservedMasks.Add(mask);
        totalCalls++;
        return 0;
    }

    [ContextMenu("Dump Observed TileSet47 Patterns")]
    public void DumpObservedTilePatterns() {
        Debug.Log($"[TileSet47] ==== Patterns Dump (total calls={totalCalls}) ====");
        Debug.Log("4-sides cuts usage:");
        for (int i = 0; i <= 15; i++) {
            int count = FourSideCutUsage.ContainsKey(i) ? FourSideCutUsage[i] : 0;
            Debug.Log($" cuts={i:D2} -> {count} occurrences");
        }
        Debug.Log("All observed raw masks:");
        foreach (var m in ObservedMasks) {
            Debug.Log($" mask={m}");
        }
        Debug.Log("====================================================");
    }
}
