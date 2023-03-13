using QFSW.QC.Containers;
using QFSW.QC.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QFSW.QC
{
    public static class TextProcessing
    {
        public static readonly char[] DefaultLeftScopers = { '<', '[', '(', '{', '"' };
        public static readonly char[] DefaultRightScopers = { '>', ']', ')', '}', '"' };

        private static readonly ConcurrentStringBuilderPool _stringBuilderPool = new ConcurrentStringBuilderPool();

        public static string ReduceScope(this string input, int maxReduction = -1)
        {
            return input.ReduceScope(DefaultLeftScopers, DefaultRightScopers, maxReduction);
        }

        public static string ReduceScope(this string input, char leftScoper, char rightScoper, int maxReduction = -1)
        {
            return input.ReduceScope(leftScoper.AsArraySingle(), rightScoper.AsArraySingle(), maxReduction);
        }

        public static string ReduceScope<T>(this string input, T leftScopers, T rightScopers, int maxReduction = -1)
            where T : IReadOnlyList<char>
        {
            if (leftScopers.Count != rightScopers.Count)
            {
                throw new ArgumentException("There must be an equal number of corresponding left and right scopers");
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            string trimmedInput = input.Trim();

            if (maxReduction == 0)
            {
                return trimmedInput;
            }

            for (int i = 0; i < leftScopers.Count; i++)
            {
                char leftScoper = leftScopers[i];
                char rightScoper = rightScopers[i];

                if (leftScoper == rightScoper)
                {
                    string descopedInput = trimmedInput;
                    bool descoped = false;
                    while (descopedInput[0] == leftScoper && descopedInput[descopedInput.Length - 1] == rightScoper)
                    {
                        descoped = true;
                        descopedInput = descopedInput.Substring(1, descopedInput.Length - 2).Trim();
                        if (descopedInput.Length == 0) { break; }
                    }

                    for (int j = 0; j < descopedInput.Length; j++)
                    {
                        if (descopedInput[j] == leftScoper && (j == 0 || descopedInput[j - 1] != '\\'))
                        {
                            descoped = false;
                            break;
                        }
                    }
                    if (descoped) { return descopedInput.ReduceScope(leftScopers, rightScopers, maxReduction - 1); }
                }
                else
                {
                    if (trimmedInput[0] == leftScoper && trimmedInput[trimmedInput.Length - 1] == rightScoper)
                    {
                        int scope = 1;
                        for (int j = 1; j < trimmedInput.Length - 1; j++)
                        {
                            if (trimmedInput[j] == leftScoper) { scope++; }
                            else if (trimmedInput[j] == rightScoper) { scope--; }

                            if (scope == 0)
                            {
                                return trimmedInput;
                            }
                        }

                        return trimmedInput.Substring(1, trimmedInput.Length - 2).ReduceScope(leftScopers, rightScopers, maxReduction - 1);
                    }
                }
            }

            return input;
        }

        public static string[] SplitScoped(this string input, char splitChar, bool autoReduceScope = false)
        {
            return input.SplitScoped(splitChar, -1, autoReduceScope);
        }

        public static string[] SplitScoped(this string input, char splitChar, int maxCount, bool autoReduceScope = false)
        {
            return input.SplitScoped(splitChar, DefaultLeftScopers, DefaultRightScopers, maxCount, autoReduceScope);
        }

        public static string[] SplitScoped(this string input, char splitChar, char leftScoper, char rightScoper, bool autoReduceScope = false)
        {
            return input.SplitScoped(splitChar, leftScoper, rightScoper, -1, autoReduceScope);
        }

        public static string[] SplitScoped(this string input, char splitChar, char leftScoper, char rightScoper, int maxCount, bool autoReduceScope = false)
        {
            return input.SplitScoped(splitChar, leftScoper.AsArraySingle(), rightScoper.AsArraySingle(), maxCount, autoReduceScope);
        }

        public static string[] SplitScoped<T>(this string input, char splitChar, T leftScopers, T rightScopers, bool autoReduceScope = false)
            where T : IReadOnlyList<char>
        {
            return input.SplitScoped(splitChar, leftScopers, rightScopers, -1, autoReduceScope);
        }

        public static string[] SplitScoped<T>(this string input, char splitChar, T leftScopers, T rightScopers, int maxCount, bool autoReduceScope = false)
            where T : IReadOnlyList<char>
        {
            if (autoReduceScope) { input = input.ReduceScope(leftScopers, rightScopers); }
            if (string.IsNullOrWhiteSpace(input)) { return Array.Empty<string>(); }

            IEnumerable<int> rawSplitIndices = GetScopedSplitPoints(input, splitChar, leftScopers, rightScopers);
            int[] splitIndices;
            if (maxCount > 0)
            {
                splitIndices = rawSplitIndices.Take(maxCount - 1).ToArray();
            }
            else
            {
                splitIndices = rawSplitIndices.ToArray();
            }

            // Return single array when no splits occurred
            if (splitIndices.Length == 0)
            {
                return new[] { input };
            }

            string[] splitString = new string[splitIndices.Length + 1];
            int lastSplitIndex = 0;
            for (int i = 0; i < splitIndices.Length; i++)
            {
                splitString[i] = input.Substring(lastSplitIndex, splitIndices[i] - lastSplitIndex).Trim();
                lastSplitIndex = splitIndices[i] + 1;
            }

            splitString[splitIndices.Length] = input.Substring(lastSplitIndex).Trim();
            return splitString;
        }

        public static IEnumerable<int> GetScopedSplitPoints<T>(string input, char splitChar, T leftScopers, T rightScopers)
            where T : IReadOnlyList<char>
        {
            if (leftScopers.Count != rightScopers.Count)
            {
                throw new ArgumentException("There must be an equal number of corresponding left and right scopers");
            }

            int[] scopes = new int[leftScopers.Count];
            for (int i = 0; i < input.Length; i++)
            {
                if (i == 0 || input[i - 1] != '\\')
                {
                    for (int j = 0; j < leftScopers.Count; j++)
                    {
                        char leftScoper = leftScopers[j];
                        char rightScoper = rightScopers[j];

                        if (input[i] == leftScoper && leftScoper == rightScoper) { scopes[j] = 1 - scopes[j]; }
                        else if (input[i] == leftScoper) { scopes[j]++; }
                        else if (input[i] == rightScoper) { scopes[j]--; }
                    }
                }

                if (input[i] == splitChar && scopes.All(x => x == 0))
                {
                    yield return i;
                }
            }
        }

        public static bool CanSplitScoped(this string input, char splitChar)
        {
            return input.CanSplitScoped(splitChar, DefaultLeftScopers, DefaultRightScopers);
        }

        public static bool CanSplitScoped(this string input, char splitChar, char leftScoper, char rightScoper)
        {
            return input.CanSplitScoped(splitChar, leftScoper.AsArraySingle(), rightScoper.AsArraySingle());
        }

        public static bool CanSplitScoped<T>(this string input, char splitChar, T leftScopers, T rightScopers)
            where T : IReadOnlyList<char>
        {
            return GetScopedSplitPoints(input, splitChar, leftScopers, rightScopers).Any();
        }

        public static string SplitFirst(this string input, char splitChar)
        {
            return input.SplitScopedFirst(splitChar, Array.Empty<char>(), Array.Empty<char>());
        }

        public static string SplitScopedFirst(this string input, char splitChar)
        {
            return input.SplitScopedFirst(splitChar, DefaultLeftScopers, DefaultRightScopers);
        }

        public static string SplitScopedFirst(this string input, char splitChar, char leftScoper, char rightScoper)
        {
            return input.SplitScopedFirst(splitChar, leftScoper.AsArraySingle(), rightScoper.AsArraySingle());
        }

        public static string SplitScopedFirst<T>(this string input, char splitChar, T leftScopers, T rightScopers)
            where T : IReadOnlyList<char>
        {
            IEnumerable<int> splitPoints = GetScopedSplitPoints(input, splitChar, leftScopers, rightScopers);
            foreach (int splitPoint in splitPoints)
            {
                return input.Substring(0, splitPoint).Trim();
            }

            return input;
        }

        public static string UnescapeText(this string input, char escapeChar) { return input.UnescapeText(escapeChar.AsArraySingle()); }
        public static string UnescapeText<T>(this string input, T escapeChars)
            where T : IReadOnlyCollection<char>
        {
            foreach (char escapeChar in escapeChars)
            {
                input = input.Replace($"\\{escapeChar}", escapeChar.ToString());
            }

            return input;
        }

        public static string ReverseItems(this string input, char splitChar)
        {
            int lastSplit = input.Length;
            StringBuilder buffer = _stringBuilderPool.GetStringBuilder(input.Length);

            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (input[i] == splitChar)
                {
                    int substringIndex = i + 1;
                    if (substringIndex < input.Length)
                    {
                        buffer.Append(input, substringIndex, lastSplit - substringIndex);
                    }

                    buffer.Append(splitChar);
                    lastSplit = i;
                }
                else if (i == 0)
                {
                    buffer.Append(input, 0, lastSplit);
                }
            }

            return _stringBuilderPool.ReleaseAndToString(buffer);
        }
    }
}
