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
                    DiagnosticCategories.Hotfix,
                    DiagnosticSeverity.Error,
                    true,
                    Description);
    }
}

