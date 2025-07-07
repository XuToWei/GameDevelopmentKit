using Microsoft.CodeAnalysis;

namespace ET.Analyzer.Custom
{
    public static class CustomOnlyUniTaskAnalyzerRule
    {
        private const string Title = "请使用UniTask，不要使用其他Task";

        private const string MessageFormat = "请使用UniTask，不要使用其他Task";

        private const string Description = "请使用UniTask，不要使用其他Task.";

        public static readonly DiagnosticDescriptor Rule =
                new DiagnosticDescriptor(CustomDiagnosticIds.OnlyUniTaskAnalyzerRuleId,
                    Title,
                    MessageFormat,
                    DiagnosticCategories.Custom,
                    DiagnosticSeverity.Error,
                    true,
                    Description);
    }

    public static class CustomDeclarationUpperAnalyzerRule
    {
        private const string Title = "{0} '{1}' 命名开头需要大写";

        private const string MessageFormat = "{0} '{1}' 命名开头需要大写";

        private const string Description = "{0} '{1}' 命名开头需要大写.";

        public static readonly DiagnosticDescriptor Rule =
                new DiagnosticDescriptor(CustomDiagnosticIds.DeclarationUpperAnalyzerRuleId,
                    Title,
                    MessageFormat,
                    DiagnosticCategories.Custom,
                    DiagnosticSeverity.Error,
                    true,
                    Description);
    }

    public static class CustomDeclarationLowerAnalyzerRule
    {
        private const string Title = "{0} '{1}' 命名开头需要小写";

        private const string MessageFormat = "{0} '{1}' 命名开头需要小写";

        private const string Description = "{0} '{1}' 命名开头需要小写.";

        public static readonly DiagnosticDescriptor Rule =
                new DiagnosticDescriptor(CustomDiagnosticIds.DeclarationLowerAnalyzerRuleId,
                    Title,
                    MessageFormat,
                    DiagnosticCategories.Custom,
                    DiagnosticSeverity.Error,
                    true,
                    Description);
    }

    public static class CustomDeclarationEndCant_AnalyzerRule
    {
        private const string Title = "{0} '{1}' 命名不能'_'结尾";

        private const string MessageFormat = "{0} '{1}' 命名不能'_'结尾";

        private const string Description = "{0} '{1}' 命名不能'_'结尾.";

        public static readonly DiagnosticDescriptor Rule =
                new DiagnosticDescriptor(CustomDiagnosticIds.DeclarationEndCant_AnalyzerRuleId,
                    Title,
                    MessageFormat,
                    DiagnosticCategories.Custom,
                    DiagnosticSeverity.Error,
                    true,
                    Description);
    }

    public static class CustomLogMethodAnalyzerRule
    {
        private const string Title = "请使用UnityGameFramework.Runtime.Log，不要使用UnityEngine.Debug";
        private const string MessageFormat = "请使用UnityGameFramework.Runtime.Log，不要使用UnityEngine.Debug";
        private const string Description = "请使用UnityGameFramework.Runtime.Log，不要使用UnityEngine.Debug.";
        public static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(CustomDiagnosticIds.LogMethodAnalyzerRuleId,
                Title,
                MessageFormat,
                DiagnosticCategories.Custom,
                DiagnosticSeverity.Error,
                true,
                Description);
    }

    public static class CustomStringConcatAnalyzerRule
    {
        private const string Title = "请使用GameFramework.Utility.Text.Format，不要使用'+'、'string.Format'、'string.Concat'";
        private const string MessageFormat = "请使用GameFramework.Utility.Text.Format，不要使用'{0}'";
        private const string Description = "请使用GameFramework.Utility.Text.Format，不要使用'+'、'string.Format'、'string.Concat'.";
        public static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(CustomDiagnosticIds.StringConcatAnalyzerRuleId,
                Title,
                MessageFormat,
                DiagnosticCategories.Custom,
                DiagnosticSeverity.Error,
                true,
                Description);
    }
}

