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
            if (HasIgnoreLogMethodAttribute(context, context.Node))
            {
                // 如果有IgnoreLogMethodAttribute特性，则不进行分析
                return;
            }
            // 检查是否为UnityEngine.Debug调用
            if (symbol.ContainingType.ToString() != "UnityEngine.Debug")
            {
                return;
            }
            if (!symbol.Name.StartsWith("Log"))
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
        
        private bool HasIgnoreLogMethodAttribute(SyntaxNodeAnalysisContext context, SyntaxNode syntaxNode)
        {
            const string IGNORE_LOG_METHOD_ATTRIBUTE = "Game.IgnoreLogMethodAttribute";
            
            // 检查包含该语法节点的类是否有IgnoreLogMethodAttribute特性
            var classDeclaration = syntaxNode.GetParentClassDeclaration();
            if (classDeclaration != null)
            {
                var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
                if (classSymbol != null && classSymbol.HasAttribute(IGNORE_LOG_METHOD_ATTRIBUTE))
                {
                    return true;
                }
            }
            
            // 检查包含该语法节点的方法是否有IgnoreLogMethodAttribute特性
            var methodDeclaration = syntaxNode.GetNeareastAncestor<MethodDeclarationSyntax>();
            if (methodDeclaration != null)
            {
                var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);
                if (methodSymbol != null && methodSymbol.HasAttribute(IGNORE_LOG_METHOD_ATTRIBUTE))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
