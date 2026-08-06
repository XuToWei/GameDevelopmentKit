using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ET.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoubleUnderscoreFieldAccessAnalyzer: DiagnosticAnalyzer
    {
        private const string GeneratedFieldPrefix = "__";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DoubleUnderscoreFieldAccessAnalyzerRule.Rule);

        public override void Initialize(AnalysisContext context)
        {
            if (!AnalyzerGlobalSetting.EnableAnalyzer)
            {
                return;
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(analysisContext =>
            {
                if (AnalyzerHelper.IsAssemblyNeedAnalyze(analysisContext.Compilation.AssemblyName, AnalyzeAssembly.AllModelHotfix))
                {
                    analysisContext.RegisterSyntaxNodeAction(this.AnalyzeIdentifier, SyntaxKind.IdentifierName);
                }
            });
        }

        private void AnalyzeIdentifier(SyntaxNodeAnalysisContext context)
        {
            var identifier = (IdentifierNameSyntax)context.Node;
            if (!identifier.Identifier.ValueText.StartsWith(GeneratedFieldPrefix, StringComparison.Ordinal) || context.SemanticModel.GetSymbolInfo(identifier).Symbol is not IFieldSymbol field)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(DoubleUnderscoreFieldAccessAnalyzerRule.Rule, identifier.GetLocation(), field.Name));
        }
    }
}
