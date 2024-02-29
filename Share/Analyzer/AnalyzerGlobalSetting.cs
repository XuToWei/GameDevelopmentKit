using System.Collections.Generic;

namespace ET.Analyzer
{
    public static class AnalyzerGlobalSetting
    {
        /// <summary>
        /// 是否开启项目的所有分析器
        /// </summary>
        public static bool EnableAnalyzer = true;

        /// <summary>
        /// EnableClass特性忽略的目录
        /// </summary>
        public static HashSet<string> EnableClassIgnoreDirNames = new HashSet<string>()
        {
            "Model/Generate", "Model/Client/Generate"
        };
    }
}