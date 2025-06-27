using Microsoft.CodeAnalysis;

namespace ET.Analyzer.Custom
{
    public static class OnlyUniTaskAnalyzerRule
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

    public static class DeclarationUpperAnalyzerRule
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
}

