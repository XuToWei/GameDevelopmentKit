using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ET.Analyzer.Custom
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CustomLogMethodAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(CustomLogMethodAnalyzerRule.Rule);

        public override void Initialize(AnalysisContext context)
        {
            if (!AnalyzerGlobalSetting.EnableAnalyzer)
            {
                return;
            }
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(this.AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.GameRuntimeAll))
            {
                return;
            }
            if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.CustomIgnorePathNames))
            {
                return;
            }
            var invocation = (InvocationExpressionSyntax)context.Node;
            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null)
            {
                return;
            }
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol as IMethodSymbol;
            if (symbol == null)
            {
                return;
            }
            // 检查是否为UnityEngine.Debug调用
            if (symbol.ContainingType.ToString() != "UnityEngine.Debug")
            {
                return;
            }
            //判断方法所在的类是不是GameFrameworkLog.ILogHelper的子类
            var classDeclaration = context.Node.GetParentClassDeclaration();
            if (classDeclaration != null)
            {
                var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration); 
                if (classSymbol != null&& classSymbol.HasInterface("GameFramework.ILogHelper"))
                {
                    return;
                }
            }
            Diagnostic diagnostic = Diagnostic.Create(CustomLogMethodAnalyzerRule.Rule, memberAccess.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
