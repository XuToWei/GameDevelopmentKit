using System.Text;

// base on https://github.com/SaladLab/Unity3D.ThaiFontAdjuster
public static class ThaiFontAdjuster
{
    public static bool IsThaiString(string s)
    {
        var length = s.Length;
        for (var i = 0; i < length; i++)
        {
            var c = s[i];
            if (c >= '\x0E00' && c <= '\x0E7F')
                return true;
        }
        return false;
    }

    // ========== EXTENDED CHARACTER TABLE ==========
    // F700:     uni0E10.descless    (base.descless)
    // F701~04:  uni0E34~37.left     (upper.left)
    // F705~09:  uni0E48~4C.lowleft  (top.lowleft)
    // F70A~0E:  uni0E48~4C.low      (top.low)
    // F70F:     uni0E0D.descless    (base.descless)
    // F710~12:  uni0E31,4D,47.left  (upper.left)
    // F713~17:  uni0E48~4C.left     (top.left)
    // F718~1A:  uni0E38~3A.low      (lower.low)
    // ==============================================

    public static (bool, string) Adjust(string s)
    {
        if (s == null) return (false, null);
        UnityEngine.Profiling.Profiler.BeginSample("ThaiFontAdjuster.Adjust");
        var isAdjusted = false;
        // http://www.bakoma-tex.com/doc/fonts/enc/c90/c90.pdf

        var length = s.Length;
        var sb = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            var c = s[i];

            // [base] ~ [top]
            if (IsTop(c) && i > 0)
            {
                // [base]             [top] -> [base]             [top.low]
                // [base]     [lower] [top] -> [base]     [lower] [top.low]
                // [base.asc]         [top] -> [base.asc]         [top.lowleft]
                // [base.asc] [lower] [top] -> [base.asc] [lower] [top.lowleft]
                var b = s[i - 1];
                if (IsLower(b) && i > 1)
                    b = s[i - 2];
                if (IsBase(b))
                {
                    var followingNikhahit = (i < length - 1 && (s[i + 1] == '\x0E33' || s[i + 1] == '\x0E4D'));
                    if (IsBaseAsc(b))
                    {
                        if (followingNikhahit)
                        {
                            // [base.asc] [top] [sara am] -> [base.asc] [nikhahit] [top.left] [sara aa]
                            isAdjusted = true;
                            c = (char)(c + ('\xF713' - '\x0E48'));
                            sb.Append('\xF711');
                            sb.Append(c);
                            if (s[i + 1] == '\x0E33')
                                sb.Append('\x0E32');
                            i += 1;
                            continue;
                        }
                        else
                        {
                            isAdjusted = true;
                            c = (char)(c + ('\xF705' - '\x0E48'));
                        }
                    }
                    else
                    {
                        if (followingNikhahit == false)
                        {
                            isAdjusted = true;
                            c = (char)(c + ('\xF70A' - '\x0E48'));
                        }
                    }
                }

                // [base.asc] [upper] [top] -> [base.asc] [upper] [top.left]
                if (i > 1 && IsUpper(s[i - 1]) && IsBaseAsc(s[i - 2]))
                {
                    isAdjusted = true;
                    c = (char)(c + ('\xF713' - '\x0E48'));
                }
            }
            // [base.asc] [upper] -> [base.asc] [upper-left]
            else if (IsUpper(c) && i > 0 && IsBaseAsc(s[i - 1]))
            {
                switch (c)
                {
                    case '\x0E31': { c = '\xF710'; isAdjusted = true; } break;
                    case '\x0E34': { c = '\xF701'; isAdjusted = true; } break;
                    case '\x0E35': { c = '\xF702'; isAdjusted = true; } break;
                    case '\x0E36': { c = '\xF703'; isAdjusted = true; } break;
                    case '\x0E37': { c = '\xF704'; isAdjusted = true; } break;
                    case '\x0E4D': { c = '\xF711'; isAdjusted = true; } break;
                    case '\x0E47': { c = '\xF712'; isAdjusted = true; } break;
                }
            }
            // [base.desc] [lower] -> [base.desc] [lower.low]
            else if (IsLower(c) && i > 0 && IsBaseDesc(s[i - 1]))
            {
                isAdjusted = true;
                c = (char)(c + ('\xF718' - '\x0E38'));
            }
            // [YO YING] [lower] -> [YO YING w/o lower] [lower]
            else if (c == '\x0E0D' && i < length - 1 && IsLower(s[i + 1]))
            {
                isAdjusted = true;
                c = '\xF70F';
            }
            // [THO THAN] [lower] -> [THO THAN w/o lower] [lower]
            else if (c == '\x0E10' && i < length - 1 && IsLower(s[i + 1]))
            {
                isAdjusted = true;
                c = '\xF700';
            }
            sb.Append(c);
        }
        UnityEngine.Profiling.Profiler.EndSample();
        return (isAdjusted, isAdjusted ? sb.ToString() : null);
    }

    private static bool IsBase(char c)
    {
        return (c >= '\x0E01' && c <= '\x0E2F') || c == '\x0E30' || c == '\x0E40' || c == '\x0E41';
    }

    private static bool IsBaseDesc(char c)
    {
        return c == '\x0E0E' || c == '\x0E0F';
    }

    private static bool IsBaseAsc(char c)
    {
        return c == '\x0E1B' || c == '\x0E1D' || c == '\x0E1F' || c == '\x0E2C';
    }

    private static bool IsTop(char c)
    {
        // Tone Mark, THANTHAKHAT
        return c >= '\x0E48' && c <= '\x0E4C';
    }

    private static bool IsLower(char c)
    {
        // SARA U, SARA UU, PHINTHU
        return c >= '\x0E38' && c <= '\x0E3A';
    }

    private static bool IsUpper(char c)
    {
        return c == '\x0E31' || c == '\x0E34' || c == '\x0E35' || c == '\x0E36' ||
            c == '\x0E37' || c == '\x0E47' || c == '\x0E4D';
    }
}
