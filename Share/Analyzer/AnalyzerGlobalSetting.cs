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
        /// 忽略所有分析器的目录名
        /// </summary>
        public static HashSet<string> IgnorePathNames = new HashSet<string>()
        {
            "Model/Generate", "Model/Client/Generate", "Hotfix/Server/Admin", "Hotfix/Server/Agent"
        };
    }
}