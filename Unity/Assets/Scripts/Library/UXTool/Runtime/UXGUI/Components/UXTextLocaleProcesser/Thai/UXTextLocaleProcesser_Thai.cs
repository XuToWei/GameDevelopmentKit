using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.UXText;

namespace UnityEngine.UI
{
    public class UXTextLocaleProcesser_Thai : UXTextLocaleProcesser
    {
        public UXTextLocaleProcesser_Thai(UXText text) : base(text)
        {
            LocalizationType = LocalizationHelper.LanguageType.Thai;
        }

        public override string GenLocaleRenderedString(string text)
        {
            ClearWordBreak();

            OriginString = text;
            UnityEngine.Profiling.Profiler.BeginSample("UXText.CustomRefreshRenderedText.Thai");

            string adjustStr = ThaiFontAdjust(OriginString);
            string breakLineStr = HandleThaiLineBreak(adjustStr);

            UnityEngine.Profiling.Profiler.EndSample();
            return breakLineStr;
        }

        private string ThaiFontAdjust(string text)
        {
            UnityEngine.Profiling.Profiler.BeginSample("UXText.ThaiFontAdjust");
            var (isAdjusted, adjustedStr) = ThaiFontAdjuster.Adjust(text);
            UnityEngine.Profiling.Profiler.EndSample();

            return isAdjusted ? adjustedStr : text;
        }

        private static List<int> _thaiWordLengths = null;
        private static HashSet<string> _thaiWordDict = null;
        public static bool InThaiWordDict(string word)
        {
            return _thaiWordDict.Contains(word);
        }
        public static List<int> GetThaiWordDictLengths()
        {
            return _thaiWordLengths;
        }
        public static bool IsThaiDictInit()
        {
            return _thaiWordDict != null;
        }
        public static void ReadThaiDict()
        {
            UnityEngine.Profiling.Profiler.BeginSample("UXText.ReadThaiDict");
            var t_thaiWordLengths = new HashSet<int>();
            _thaiWordLengths = new List<int>();
            _thaiWordDict = new HashSet<string>();

            var txt = ResourceManager.Load<TextAsset>(UXGUIConfig.ThaiWordDictPath);
            if (txt != null)
            {
                var lines = txt.text.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Length <= 0) continue;
                    _thaiWordDict.Add(line);
                    t_thaiWordLengths.Add(line.Length);
                }
                _thaiWordLengths = new List<int>(t_thaiWordLengths);
                // 从大到小
                _thaiWordLengths.Sort();
                _thaiWordLengths.Reverse();
            }
            else
            {
                Debug.Log($"泰语字典损坏，初始化失败");
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public static int MaxThaiLineBreakHandleTime = 100;
        private string HandleThaiLineBreak(string text)
        {
            if (!IsThaiDictInit()) ReadThaiDict();

            UnityEngine.Profiling.Profiler.BeginSample("UXText.HandleThaiLineBreak");
            var addLineBreakCount = 0;
            var lastHandledLine = 0;
            for (int i = 0; i < MaxThaiLineBreakHandleTime; i++)
            {
                if (TryThaiLineBreak(lastHandledLine, addLineBreakCount, out var insertPos, out lastHandledLine))
                {
                    text = text.Insert(insertPos, "\n");
                    var setting = LocaleText.GetGenerationSettings(LocaleText.rectTransform.rect.size);

                    LocaleText.cachedTextGenerator.PopulateWithErrors(text, setting, LocaleText.gameObject);
                    ++addLineBreakCount;
                }
                else
                {
                    break;
                }
                if (i == MaxThaiLineBreakHandleTime - 1)
                {
                    Debug.Log($"泰语分词处理了{MaxThaiLineBreakHandleTime}还没处理完");
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return text;

        }
        private List<(int, int)> _words = new List<(int, int)>();
        private bool _wordBroken = false;
        private void ClearWordBreak()
        {
            _wordBroken = false;
            _words.Clear();
        }
        private void HandleWordBreak()
        {
            if (_wordBroken) return;
            _wordBroken = true;

            if (!IsThaiDictInit()) ReadThaiDict();
            if (OriginString == null) return;
            UnityEngine.Profiling.Profiler.BeginSample("UXText.HandleWordBreak");
            for (int idx = 0; idx < OriginString.Length; idx++)
            {
                foreach (var wordLength in _thaiWordLengths)
                {
                    var endIdx = wordLength + idx;
                    if (endIdx > OriginString.Length) continue;

                    var word = OriginString.Substring(idx, wordLength);
                    if (_thaiWordDict.Contains(word))
                    {
                        _words.Add((idx, endIdx));
                        // 词尽量长
                        idx = endIdx - 1;
                        break;
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        private bool TryThaiLineBreak(int lastHandleLine, int breakCount, out int insertPos, out int handledLine)
        {
            insertPos = 0;
            handledLine = lastHandleLine;
            var lines = LocaleText.cachedTextGenerator.lines;
            var lastLineStartIdx = 0;
            for (int idx = 0; idx < lines.Count; idx++)
            {
                if (idx > lastHandleLine)
                {
                    this.HandleWordBreak();
                    var lastLineStartPos = lines[idx - 1].startCharIdx - breakCount;
                    var lastLineEndPos = lines[idx].startCharIdx - breakCount;
                    var left = 0;
                    var right = this._words.Count;
                    while (left < right)
                    {
                        var center = (left + right) / 2;
                        if (this._words[center].Item1 >= lastLineEndPos) right = center - 1;
                        else if (this._words[center].Item2 < lastLineEndPos) left = center + 1;
                        else left = right = center;
                    }

                    if (left < 0 || left >= this._words.Count) continue;
                    var wordExp = this._words[left];
                    if (wordExp.Item1 < lastLineEndPos && lastLineEndPos <= wordExp.Item2)
                    {
                        insertPos = wordExp.Item1 + breakCount;
                        handledLine = idx;
                        return true;
                    }
                }
                lastLineStartIdx = lines[idx].startCharIdx;
            }
            return false;
        }
    }
}
