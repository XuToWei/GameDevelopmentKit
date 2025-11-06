namespace Game
{
    public static class StringUtility
    {
        public static string ToLocalization(this string key)
        {
            return GameEntry.Localization.GetString(key);
        }

        public static string ToLocalization<T>(this string key, T arg)
        {
            return GameEntry.Localization.GetString(key, arg);
        }

        public static string ToLocalization<T1, T2>(this string key, T1 arg1, T2 arg2)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2);
        }

        public static string ToLocalization<T1, T2, T3>(this string key, T1 arg1, T2 arg2, T3 arg3)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3);
        }

        public static string ToLocalization<T1, T2, T3, T4>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4);
        }

        public static string ToLocalization<T1, T2, T3, T4, T5>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4, arg5);
        }

        public static string ToLocalization<T1, T2, T3, T4, T5, T6>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static string ToLocalization<T1, T2, T3, T4, T5, T6, T7>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static string ToLocalization<T1, T2, T3, T4, T5, T6, T7, T8>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static string ToLocalization<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static string ToLocalization<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static string ToLocalization<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static string ToLocalization<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static string ToLocalization<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static string ToLocalization<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static string ToLocalization<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static string ToLocalization<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return GameEntry.Localization.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        }
    }
}
