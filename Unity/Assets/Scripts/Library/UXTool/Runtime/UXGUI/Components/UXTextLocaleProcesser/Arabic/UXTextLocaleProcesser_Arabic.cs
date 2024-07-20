using ArabicSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ThunderFireUnityEx;

namespace UnityEngine.UI
{
    public class UXTextLocaleProcesser_Arabic : UXTextLocaleProcesser
    {
        [SerializeField] private string m_ReversedFixedText;

        private string m_Reversed_NoRichSymbol_FixedText;

        [SerializeField] private bool m_UseTashkeel = false;
        public bool UseTashkeel
        {
            get => m_UseTashkeel;
            set
            {
                if (m_UseTashkeel != value)
                {
                    m_UseTashkeel = value;
                }
            }
        }

        [SerializeField] private bool m_UseHinduNumber = false;
        public bool UseHinduNumber
        {
            get => m_UseHinduNumber;
            set
            {
                if (m_UseHinduNumber != value)
                {
                    m_UseHinduNumber = value;
                }
            }
        }

        public UXTextLocaleProcesser_Arabic(UXText text) : base(text)
        {
            LocalizationType = LocalizationHelper.LanguageType.Arabic;
        }

        public override void ModifyLocaleTextSettings()
        {
            switch (LocaleText.alignment)
            {
                case TextAnchor.MiddleLeft:
                    LocaleText.alignment = TextAnchor.MiddleRight;
                    break;
                case TextAnchor.UpperLeft:
                    LocaleText.alignment = TextAnchor.UpperRight;
                    break;
                case TextAnchor.LowerLeft:
                    LocaleText.alignment = TextAnchor.LowerRight;
                    break;
            }
        }

        //处理 阿拉伯语的字母、语序、富文本标签等问题
        //因为句子语序会被按行分割重排，所以阿拉伯语的富文本标签不能跨行(标签之间不能有\n换行符, 文本框Rect导致的自动换行是OK的)
        //原文本:                        على</color>{0}<color=#FFAD2E> واحدة
        //SplitAndFixText后(ArabicFix):  ةﺪﺣاو >/color<{0}>color=#FFAD2E<ﯽﻠﻋ
        //FixRichTextSymbol后:           ةﺪﺣاو <color=#FFAD2E>{0}</color>ﯽﻠﻋ
        public override string GenLocaleRenderedString(string text)
        {
            OriginString = text;
            if (!ContainsArabicLetters(text))
            {
                return text;
            }

            //var tmp = text.ToArray().ToList();
            UnityEngine.Profiling.Profiler.BeginSample("UXText.UXTextLocaleProcesser_Arabic GenLocaleRenderedString SplitAndFixText");
            //替换阿拉伯语字母 按行倒转字符串
            SplitAndFixText();
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("UXText.UXTextLocaleProcesser_Arabic GenLocaleRenderedString FixNoArabicBracket");
            //把非Arabic的() 和 < > 反向回正确状态
            FixNoArabicBracket();
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("UXText.UXTextLocaleProcesser_Arabic GenLocaleRenderedString FixRichTextEndSymbol");
            FixRichTextEndSymbol();
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("UXText.UXTextLocaleProcesser_Arabic GenLocaleRenderedString SwapRichTextSymbol");
            //对调richText的开闭标签
            SwapRichTextSymbol();
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("UXText.UXTextLocaleProcesser_Arabic GenLocaleRenderedString GenRemoveRichTextSymbolText");
            //生成一个不带富文本标签的字符串
            GenRemoveRichTextSymbolText();
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("UXText.UXTextLocaleProcesser_Arabic GenLocaleRenderedString FillStringInfos");
            //计算并缓存每个word和char的顺序和起止信息等
            //用来在后面的手动插入 \n 换行时使用
            FillStringInfos();
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("UXText.UXTextLocaleProcesser_Arabic GenLocaleRenderedString GenFitWidthRenderedText");
            //根据wordMeshInfo和verts信息计算每行长度，如果超行手动插入换行符换行
            m_Reversed_NoRichSymbol_FixedText = GenFitWidthRenderedText();
            UnityEngine.Profiling.Profiler.EndSample();

            //把换好行的字符串重新加回richText标签
            UnityEngine.Profiling.Profiler.BeginSample("UXText.UXTextLocaleProcesser_Arabic GenLocaleRenderedString MergeNewLineAndRichTextSymbol");
            MergeNewLineAndRichTextSymbol();
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("UXText.UXTextLocaleProcesser_Arabic GenLocaleRenderedString FillRichTextColorInfos");
            //计算并缓存每个char的富文本颜色信息
            //用来在后面修改顶点颜色时使用
            FillRichTextColorInfos();
            UnityEngine.Profiling.Profiler.EndSample();

            List<string> splitLines = new List<string>(m_Reversed_NoRichSymbol_FixedText.Split('\n'));
            splitLines.Reverse();
            m_Reversed_NoRichSymbol_FixedText = string.Join("\n", splitLines);

            return m_Reversed_NoRichSymbol_FixedText;
        }

        //从m_OrigialText对句子拆分，单行修正阿拉伯语，句子顺序被翻转
        protected void SplitAndFixText()
        {
            if (OriginString.Contains(Environment.NewLine))
                OriginString = OriginString.Replace(Environment.NewLine, "\n");

            List<string> lines = new List<string>(OriginString.Split('\n'));

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = ArabicFixer.Fix(lines[i], m_UseTashkeel, false, m_UseHinduNumber);
            }
            lines.Reverse();

            m_ReversedFixedText = string.Join("\n", lines);
        }

        // 使用正则表达式匹配阿拉伯语字母
        private static Regex ArabicRegex = new Regex(@"\p{IsArabic}");
        private bool ContainsArabicLetters(string text)
        {
            return ArabicRegex.IsMatch(text);
        }

        private static Regex BracketRegex = new Regex(@"(>[^<]+<)|(\)[^(]+\()");

        //把非Arabic字符两边的括号换回正确的顺序 )UTC( -> (UTC)
        //richText所用的<> 和 </>标签中 尖括号也会被反向，这个方法里也会再反向回到正确状态
        //不在ArabicSupport中处理的原因是，原文中有可能会有使用尖括号但是并不是富文本的情况
        //保持ArabicSupport中只处理阿拉伯字母的问题，不做富文本处理这种额外逻辑
        private void FixNoArabicBracket()
        {
            // 使用正则表达式匹配被<>或()包着的子字符串
            string result = BracketRegex.Replace(m_ReversedFixedText, m =>
            {
                string match = m.Value;

                //if (!ContainsArabicLetters(match))
                //{
                if (match.StartsWith(")") && match.EndsWith("("))
                {
                    return "(" + match.Substring(1, match.Length - 2) + ")";
                }
                if (match.StartsWith(">") && match.EndsWith("<"))
                {
                    return "<" + match.Substring(1, match.Length - 2) + ">";
                }
                //}
                return match;
            });
            m_ReversedFixedText = result;
        }

        #region RichText
        //Rich Text symbol:
        //>b< >/b<
        //>i< >/i<
        //>size< >/size<
        //>color=#ff0000ff< >/color<
        //>material=2< >/material<
        //<quad ******>
        private static int symbolCompareLength = 6;
        private static List<string> richTextSymbols = new List<string>
        { "</b>", "<b>" , "</i>", "<i>" , "</size", "<size=" , "</colo", "<color", "</mate", "<mater"};
        private static Dictionary<string, string> richTextSymbolsMatch = new Dictionary<string, string>
        {
            { "<b>", "</b>"},
            { "<i>", "</i>"},
            { "<size=", "</size>"},
            { "<color", "</color>"},
            { "<mater", "</material>"},
        };
        private static Regex richTextSymbolRegex = new Regex(@"(<\/?(?:b|i|size|color=#\w{8}|color=#\w{6}|color|material=\d+|material|quad\s+\*+)>)");
        private static Regex richTextSymbolAndBlankRegex = new Regex(@"(\s|<\/?(?:b|i|size|color=#\w{8}|color=#\w{6}|color|material=\d+|material|quad\s+\*+)>)");

        //Arabic中因为需要按行翻转语言，所以richText标记遇到换行之后会失效
        //本方法中记录下原文本中需要修改颜色的字符信息，用来直接改顶点色
        //同时去掉richText标签 <color>生效、<b><i><size><material>会失效
        public override void FixVerts(IList<UIVertex> verts)
        {
            if (!ContainsArabicLetters(OriginString))
            {
                return;
            }

            UIVertex m_TempVerts1;
            UIVertex m_TempVerts2;
            UIVertex m_TempVerts3;
            UIVertex m_TempVerts4;
            for (int meshIdx = 0; meshIdx * 4 < verts.Count; meshIdx++)
            {
                if (meshIdx >= m_CharColorInfos.Count) break;

                var charInfo = m_CharColorInfos[meshIdx];
                if (charInfo.hasColor)
                {
                    m_TempVerts1 = verts[meshIdx * 4];
                    m_TempVerts2 = verts[meshIdx * 4 + 1];
                    m_TempVerts3 = verts[meshIdx * 4 + 2];
                    m_TempVerts4 = verts[meshIdx * 4 + 3];
                    m_TempVerts1.color = charInfo.charColor;
                    m_TempVerts2.color = charInfo.charColor;
                    m_TempVerts3.color = charInfo.charColor;
                    m_TempVerts4.color = charInfo.charColor;
                    verts[meshIdx * 4] = m_TempVerts1;
                    verts[meshIdx * 4 + 1] = m_TempVerts2;
                    verts[meshIdx * 4 + 2] = m_TempVerts3;
                    verts[meshIdx * 4 + 3] = m_TempVerts4;
                }
            }
        }

        private bool IsRichTextSymbol(string word, out string newWord)
        {
            newWord = word;
            if (word.StartsWith("<") && word.EndsWith(">"))
            {
                if (word.Length > symbolCompareLength)
                {
                    newWord = word.Substring(0, symbolCompareLength);
                }
                if (richTextSymbols.Contains(newWord))
                {
                    return true;
                }
            }
            return false;
        }

        private void FixRichTextEndSymbol()
        {
            m_ReversedFixedText = m_ReversedFixedText.Replace("<color/>", "</color>");
        }


        //对调richText的开闭标签顺序
        private void SwapRichTextSymbol()
        {
            //string t = "ab >/b<>/i<>/size<>/color<>/material< cde>hi< >material=2<>color=#ff0000ff<>size=25<>i<>b<";
            string[] words = richTextSymbolRegex.Split(m_ReversedFixedText);
            m_ReversedFixedText = ReverseRichTextSymbol(words);
        }
        private string ReverseRichTextSymbol(string[] words)
        {
            Stack<int> richTextSymbolIndexStack = new Stack<int>();
            string[] newWords = new string[words.Length];
            for (int i = 0; i < words.Length; i++)
            {
                if (IsRichTextSymbol(words[i], out string newWord))
                {
                    //ReverseRichTextSymbolBracket(ref words[i]);
                    //richText标签，检查和栈中标签是否匹配，匹配上就交换位置
                    if (richTextSymbolIndexStack.Count > 0)
                    {
                        if (richTextSymbolsMatch.ContainsKey(newWord))
                        {
                            int stackWordIndex = richTextSymbolIndexStack.Peek();
                            if (richTextSymbolsMatch[newWord].Equals(newWords[stackWordIndex]))
                            {
                                //richText标签匹配上了,交换一下开闭标签位置, Stack中Top的richText标签出栈
                                newWords[i] = newWords[richTextSymbolIndexStack.Peek()];
                                newWords[richTextSymbolIndexStack.Peek()] = words[i];
                                richTextSymbolIndexStack.Pop();
                            }
                            else
                            {
                                //richText标签没匹配上，当前标签入栈
                                newWords[i] = words[i];
                                richTextSymbolIndexStack.Push(i);
                            }
                        }
                        else
                        {
                            //richtext标签不是开始标签,直接入栈
                            newWords[i] = words[i];
                            richTextSymbolIndexStack.Push(i);
                        }
                    }
                    else
                    {
                        //richText标签栈为空，直接入栈
                        newWords[i] = words[i];
                        richTextSymbolIndexStack.Push(i);
                    }
                }
                else
                {
                    //不是richText标签，直接加到新字符串数组中
                    newWords[i] = words[i];
                }
            }
            return string.Join("", newWords);
        }

        //生成不带richText标签的字符串
        public void GenRemoveRichTextSymbolText()
        {
            string result = richTextSymbolRegex.Replace(m_ReversedFixedText, match =>
            {
                return string.Empty;
            });
            m_Reversed_NoRichSymbol_FixedText = result;
        }

        //反向 <> 符号
        //把 >b< 改成 <b>
        private void ReverseRichTextSymbolBracket(ref string word)
        {
            char[] charArray = word.ToCharArray();
            charArray[0] = '<';
            charArray[charArray.Length - 1] = '>';
            word = new string(charArray);
        }

        public struct CharColorInfo
        {
            //char在verts中的idx 不算空格
            public int vertCharIdx;
            public bool hasColor;
            public Color charColor;
        }
        protected List<CharColorInfo> m_CharColorInfos = new List<CharColorInfo>();

        private static List<char> richTextColorEnd = new List<char>() { '<', '/', 'c', 'o', 'l', 'o', 'r', '>' };
        private static List<char> richTextColor = new List<char>();
        //把计算过宽度重新插入换行符\n之后的m_Reversed_NoRichSymbol_FixedText和m_ReversedFixedText字符串合并
        //新字符串中同时保留计算好的换行符和RchTextSymbol
        private void MergeNewLineAndRichTextSymbol()
        {
            List<char> reversedFixedTextChars = m_ReversedFixedText.ToCharArray().ToList();
            List<char> chars = m_Reversed_NoRichSymbol_FixedText.ToCharArray().ToList();

            List<char> newChars = new List<char>();

            int reversedFixedTextCharIndex = 0;
            bool inRichTextColor = false;
            for (int charIdx = 0; charIdx < chars.Count; charIdx++)
            {
                char curChar = chars[charIdx];
                //先处理换行后Text中的\n
                if (curChar == '\n')
                {
                    if (inRichTextColor)
                    {
                        //换行时如果在RichTextColor中,给这行加上一个</color>, 给下一行开头加上一个<color=#>
                        newChars.AddRange(richTextColorEnd);
                        newChars.Add(curChar);
                        newChars.AddRange(richTextColor);
                    }
                    else
                    {
                        //不在RichTextColor标签中正常换行即可
                        newChars.Add(curChar);
                    }

                    if (reversedFixedTextCharIndex >= reversedFixedTextChars.Count)
                    {
                        //已经是最后一位，超了没影响
                        if (charIdx == chars.Count - 1)
                        {
                            continue;
                        }
                        else
                        {
                            Debug.LogWarning("UXText LocaleProcesser_Arabic MergeNewLineAndRichTextSymbol /n Error: " + LocaleText.transform.PathFromRoot());
                            continue;
                        }
                    }
                    //如果原字符串也是换行符, 原字符串index前进一位
                    char curReversedFixedTextChar = reversedFixedTextChars[reversedFixedTextCharIndex];
                    if (curReversedFixedTextChar == '\n')
                    {
                        reversedFixedTextCharIndex++;
                    }
                }
                else
                {
                    if (reversedFixedTextCharIndex >= reversedFixedTextChars.Count)
                    {
                        Debug.LogWarning("UXText LocaleProcesser_Arabic MergeNewLineAndRichTextSymbol Error: " + LocaleText.transform.PathFromRoot());
                        continue;
                    }
                    char curReversedFixedTextChar = reversedFixedTextChars[reversedFixedTextCharIndex];
                    //处理原Text中的RichTextColor信息
                    if (curReversedFixedTextChar == '<')
                    {
                        List<char> richTextSymbolChar = reversedFixedTextChars.GetRange(reversedFixedTextCharIndex, 8);

                        if (string.Join("", richTextSymbolChar) == "<color=#")
                        {
                            inRichTextColor = true;
                            richTextColor = reversedFixedTextChars.GetRange(reversedFixedTextCharIndex, 15);

                            newChars.AddRange(richTextColor);
                            reversedFixedTextCharIndex = reversedFixedTextCharIndex + 15 + 1;
                        }
                        if (string.Join("", richTextSymbolChar) == "</color>")
                        {
                            newChars.AddRange(reversedFixedTextChars.GetRange(reversedFixedTextCharIndex, 8));
                            reversedFixedTextCharIndex = reversedFixedTextCharIndex + 8 + 1;
                            inRichTextColor = false;
                        }
                    }
                    else
                    {
                        reversedFixedTextCharIndex++;
                    }
                    newChars.Add(curChar);
                }
            }

            m_ReversedFixedText = new string(newChars.ToArray());
        }

        private void FillRichTextColorInfos()
        {
            m_CharColorInfos.Clear();
            CharColorInfo curCharInfo = new CharColorInfo();

            int totalCharIdx = 0;
            if (m_ReversedFixedText == null)
                return;
            string[] reversedFixedLines = m_ReversedFixedText.Split('\n');
            reversedFixedLines = reversedFixedLines.Reverse().ToArray();
            for (int lineIdx = 0; lineIdx < reversedFixedLines.Length; lineIdx++)
            {
                string[] words = richTextSymbolAndBlankRegex.Split(reversedFixedLines[lineIdx]);

                bool hasColor = false;
                Color wordColor = Color.white;

                string tmp;
                for (int wordIdx = 0; wordIdx < words.Length; wordIdx++)
                {
                    string word = words[wordIdx];
                    if (string.IsNullOrWhiteSpace(word))
                    {
                        //空格跳过 因为生成的verts中没有空格的顶点
                        continue;
                    }

                    if (IsRichTextSymbol(word, out tmp))
                    {
                        if (word.StartsWith("<color=#") && word.EndsWith(">"))
                        {
                            hasColor = true;
                            string colorStr = word.Substring(7, word.Length - 8);
                            ColorUtility.TryParseHtmlString(colorStr, out wordColor);
                        }

                        if (word.Equals("</color>"))
                        {
                            hasColor = false;
                        }
                        continue;
                    }
                    //缓存每个字符的颜色信息 
                    for (int charIndex = 0; charIndex < word.Length; charIndex++)
                    {
                        curCharInfo.vertCharIdx = totalCharIdx;
                        if (hasColor)
                        {
                            curCharInfo.hasColor = true;
                            curCharInfo.charColor = wordColor;
                        }
                        else
                        {
                            curCharInfo.hasColor = false;
                        }
                        m_CharColorInfos.Add(curCharInfo);
                        totalCharIdx++;
                    }
                }
            }
        }
        #endregion


        #region 手动换行 Info members of text and mesh
        //Arabic换行逻辑
        //原文 : gfedcba
        //          kjih
        //换行后:  edcba
        //         jihgf
        //             k
        protected string GenFitWidthRenderedText()
        {
            //保存原有HWrap状态
            bool m_HorizontalWrap = LocaleText.customGenerationSettings.horizontalOverflow == HorizontalWrapMode.Wrap;

            //根据顶点插入换行符，重新生成网格
            if (m_HorizontalWrap)
            {
                LocaleText.customGenerationSettings.horizontalOverflow = HorizontalWrapMode.Overflow;

                float scaleFactorCache = LocaleText.customGenerationSettings.scaleFactor;
                LocaleText.customGenerationSettings.scaleFactor = 1;
                LocaleText.cachedTextGenerator.PopulateWithErrors(m_Reversed_NoRichSymbol_FixedText, LocaleText.customGenerationSettings, LocaleText.gameObject);

                IList<UIVertex> verts = LocaleText.cachedTextGenerator.verts;
                string wrappedText = ManuallyRTLWrapFromMesh(verts);

                LocaleText.customGenerationSettings.scaleFactor = scaleFactorCache;
                return wrappedText;
            }
            else
            {
                return m_Reversed_NoRichSymbol_FixedText;
            }
        }

        public struct CharMeshInfo
        {
            //char在字符串中的Idx 计算时包括空格
            public int charIdx;
            public float xMin, xMax;
        }

        public struct WordMeshInfo
        {
            //text字符串中的单词信息
            public int startCharIdx;
            public int numOfChars;
            public int endCharIdx { get => startCharIdx + numOfChars - 1; }

            //mesh中的单词信息
            public int startMeshIdx;
            public int endMeshIdx { get => startMeshIdx + numOfChars - 1; }

            public float xMin, xMax;
        }

        public struct LineInfo
        {
            public int startWordIdx;
            public int numOfWords;
            public int startCharIdx;
            public int numofChars;
            public int endWordIdx { get => startWordIdx + numOfWords - 1; }
            public int endCharIdx { get => startCharIdx + numofChars - 1; }
        }

        protected List<CharMeshInfo> m_CharMeshInfos = new List<CharMeshInfo>();
        protected List<WordMeshInfo> m_WordMeshInfos = new List<WordMeshInfo>();
        protected List<LineInfo> m_LineInfos = new List<LineInfo>();

        public int NaturalLineCount { get => m_LineInfos.Count; }
        public int TotalWordCount { get => m_WordMeshInfos.Count; }

        /// <summary>
        /// 拆分文本字符信息 处理每行 每个单词 的起止字符、字符个数信息
        /// </summary>
        protected void FillStringInfos()
        {
            m_WordMeshInfos.Clear();
            m_LineInfos.Clear();

            LineInfo curLine = new LineInfo();
            WordMeshInfo curWordInfo = new WordMeshInfo();

            int totalCharIdx = 0;
            int curWordIdx = 0;
            if (m_Reversed_NoRichSymbol_FixedText == null)
                return;
            string[] reversedFixedLines = m_Reversed_NoRichSymbol_FixedText.Split('\n');
            for (int lineIdx = 0; lineIdx < reversedFixedLines.Length; lineIdx++)
            {
                curLine.startCharIdx = totalCharIdx;
                curLine.startWordIdx = curWordIdx;
                string[] words = reversedFixedLines[lineIdx].Split(' ');

                curLine.numOfWords = words.Length;
                int localCharIdx = 0;

                for (int wordIdx = 0; wordIdx < words.Length; wordIdx++)
                {
                    string word = words[wordIdx];
                    curWordInfo.startCharIdx = curLine.startCharIdx + localCharIdx;
                    curWordInfo.numOfChars = word.Length;
                    m_WordMeshInfos.Add(curWordInfo);
                    localCharIdx += word.Length + 1;
                }

                curLine.numofChars = localCharIdx - 1;
                m_LineInfos.Add(curLine);

                curWordIdx += words.Length;
                totalCharIdx += localCharIdx;
            }
        }

        /// <summary>
        /// 从顶点数据获取Mesh与Char的映射关系，每4个顶点对应一个CharMesh，获取每个CharMesh的最大和最小的X坐标
        /// </summary>
        /// <param name="verts"></param>
        protected void FillMeshInfos(IList<UIVertex> verts)
        {
            if (verts.Count == 0) return;
            m_CharMeshInfos.Clear();
            if (m_WordMeshInfos.Count == 0) FillStringInfos();
            if (m_WordMeshInfos.Count == 0) return;

            int charIdx = 0, wordIdx = 0;
            while (m_WordMeshInfos[wordIdx].numOfChars == 0) wordIdx++; //跳过开头空格
            charIdx = wordIdx; //起始的charIdx 和 wordIdx是相同的,因为之前跳过的每个空格占一个char
            WordMeshInfo curWordInfo = m_WordMeshInfos[wordIdx];
            for (int meshIdx = 0; meshIdx * 4 < verts.Count; meshIdx++)
            {
                CharMeshInfo curCharMesh;
                curCharMesh.charIdx = charIdx;
                curCharMesh.xMin = verts[meshIdx * 4].position.x;
                curCharMesh.xMax = verts[meshIdx * 4 + 1].position.x;
                m_CharMeshInfos.Add(curCharMesh);

                //在每个词首存入单词信息
                if (charIdx == 0 || m_Reversed_NoRichSymbol_FixedText[charIdx - 1] == ' ' || m_Reversed_NoRichSymbol_FixedText[charIdx - 1] == '\n')
                {
                    curWordInfo = m_WordMeshInfos[wordIdx];
                    curWordInfo.startMeshIdx = meshIdx;
                    curWordInfo.xMin = curCharMesh.xMin;
                }

                charIdx++;

                //在词尾空格处将单词信息存入列表
                if (charIdx >= m_Reversed_NoRichSymbol_FixedText.Length || m_Reversed_NoRichSymbol_FixedText[charIdx] == ' ' || m_Reversed_NoRichSymbol_FixedText[charIdx] == '\n')
                {
                    curWordInfo.xMax = curCharMesh.xMax;
                    m_WordMeshInfos[wordIdx] = curWordInfo;
                    charIdx++;
                    wordIdx++;
                    while (wordIdx < m_WordMeshInfos.Count && m_WordMeshInfos[wordIdx].numOfChars == 0)
                        wordIdx++; //跳过空词
                }
            }
        }
        #endregion Info members of text and mesh


        /// <summary>
        /// 手动计算下换行
        /// 根据Rect的宽度计算单行能否显示得下，如果不能，找到换行处，插入回车符，生成新的字符串
        /// </summary>
        /// <param name="verts"></param>
        /// <returns></returns>
        string ManuallyRTLWrapFromMesh(IList<UIVertex> verts)
        {
            //获取框的总宽度
            float widthLimit = LocaleText.gameObject.GetComponent<RectTransform>().rect.width;

            FillMeshInfos(verts);

            List<int> charIndicesToInsert = new List<int>();
            foreach (LineInfo line in m_LineInfos)
            {
                float lineXMax = m_WordMeshInfos[line.endWordIdx].xMax;
                for (int wordIdx = line.endWordIdx; wordIdx >= line.startWordIdx; wordIdx--)
                {
                    WordMeshInfo curWordMesh = m_WordMeshInfos[wordIdx];
                    if (lineXMax - curWordMesh.xMin > widthLimit)
                    {
                        charIndicesToInsert.Add(curWordMesh.endCharIdx + 1);
                        lineXMax = curWordMesh.xMax;
                    }
                }
            }

            charIndicesToInsert.Sort();
            if (m_Reversed_NoRichSymbol_FixedText == null)
                return "";
            List<char> fixedTextBuffer = new List<char>(m_Reversed_NoRichSymbol_FixedText);
            int insertedCount = 0;
            foreach (int insertIdx in charIndicesToInsert)
            {
                fixedTextBuffer.Insert(insertIdx + insertedCount, '\n');
                insertedCount++;
            }
            char[] fixedChars = fixedTextBuffer.ToArray();
            string fixedText = new string(fixedChars);
            return fixedText;
        }
    }
}
