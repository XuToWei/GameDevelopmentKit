using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ET.Analyzer.Custom
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OnlyUniTaskAnalyzer: DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
                ImmutableArray.Create(OnlyUniTaskAnalyzerRule.Rule);
        
        public override void Initialize(AnalysisContext context)
        {
            if (!AnalyzerGlobalSetting.EnableAnalyzer)
            {
                return;
            }
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(this.Analyzer, SyntaxKind.MethodDeclaration,SyntaxKind.LocalFunctionStatement);
        }
        
        private void Analyzer(SyntaxNodeAnalysisContext context)
        {
            if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.All))
            {
                return;
            }
            
            IMethodSymbol? methodSymbol = null;
            
            if (context.Node is MethodDeclarationSyntax methodDeclarationSyntax)
            {
                methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
            }
            else if (context.Node is LocalFunctionStatementSyntax localFunctionStatementSyntax)
            {
                methodSymbol = context.SemanticModel.GetDeclaredSymbol(localFunctionStatementSyntax) as IMethodSymbol;

            }
            if (methodSymbol == null || methodSymbol.ReturnsVoid)
            {
                return;
            }
            
            var returnType = methodSymbol.ReturnType;
            string namespaceName = "";
            if (returnType.ContainingNamespace != null)
            {
                namespaceName = returnType.ContainingNamespace.ToString();
            }
            if (methodSymbol.IsAsync)
            {
                if (namespaceName != "Cysharp.Threading.Tasks" || !returnType.Name.StartsWith("UniTask"))
                {
                    Diagnostic diagnostic = Diagnostic.Create(OnlyUniTaskAnalyzerRule.Rule, context.Node.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
            else
            {
                if (namespaceName == "System.Threading.Tasks" && returnType.Name.StartsWith("Task"))
                {
                    Diagnostic diagnostic = Diagnostic.Create(OnlyUniTaskAnalyzerRule.Rule, context.Node.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}