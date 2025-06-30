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
}

