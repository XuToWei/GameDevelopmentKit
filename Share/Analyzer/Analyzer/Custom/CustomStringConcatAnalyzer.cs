using System.Collections.Immutable;
using System.Linq;
using ET.Analyzer.Custom;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ET.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CustomStringConcatAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(CustomStringConcatAnalyzerRule.Rule);

        public override void Initialize(AnalysisContext context)
        {
            if (!AnalyzerGlobalSetting.EnableAnalyzer)
            {
                return;
            }
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(this.AnalyzeBinaryExpression, SyntaxKind.AddExpression);
            context.RegisterSyntaxNodeAction(this.AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
            //内插
            //context.RegisterSyntaxNodeAction(this.AnalyzeInterpolatedStringExpression, SyntaxKind.InterpolatedStringExpression);
        }

        private void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
        {
            if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.GameRuntimeAll))
            {
                return;
            }
            if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.CustomIgnorePathNames))
            {
                return;
            }

            var binaryExpression = (BinaryExpressionSyntax)context.Node;
            
            // 检查是否为字符串拼接（+操作符）
            if (binaryExpression.OperatorToken.Kind() == SyntaxKind.PlusToken)
            {
                // 检查操作数是否包含字符串类型
                var leftType = context.SemanticModel.GetTypeInfo(binaryExpression.Left).Type;
                var rightType = context.SemanticModel.GetTypeInfo(binaryExpression.Right).Type;
                
                if (IsStringType(leftType) || IsStringType(rightType))
                {
                    // 检查是否使用了IgnoreStringConcatAttribute特性
                    if (HasIgnoreStringConcatAttribute(context, binaryExpression))
                    {
                        return;
                    }

                    var diagnostic = Diagnostic.Create(
                        CustomStringConcatAnalyzerRule.Rule, 
                        binaryExpression.OperatorToken.GetLocation(), 
                        "+");
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
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
            
            // 检查是否为string.Format调用
            var symbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (symbol != null && 
                symbol.ContainingType?.ToString() == "string")
            {
                // 检查是否使用了IgnoreStringConcatAttribute特性
                if (HasIgnoreStringConcatAttribute(context, invocation))
                {
                    return;
                }

                if (symbol.Name == "Format")
                {
                    var diagnostic = Diagnostic.Create(
                        CustomStringConcatAnalyzerRule.Rule, 
                        invocation.GetLocation(), 
                        "string.Format");
                    context.ReportDiagnostic(diagnostic);
                }
                else if (symbol.Name == "Concat")
                {
                    var diagnostic = Diagnostic.Create(
                        CustomStringConcatAnalyzerRule.Rule, 
                        invocation.GetLocation(), 
                        "string.Concat");
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private void AnalyzeInterpolatedStringExpression(SyntaxNodeAnalysisContext context)
        {
            if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.GameRuntimeAll))
            {
                return;
            }
            if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.CustomIgnorePathNames))
            {
                return;
            }

            var interpolatedString = (InterpolatedStringExpressionSyntax)context.Node;
            
            // 检查是否使用了IgnoreStringConcatAttribute特性
            if (HasIgnoreStringConcatAttribute(context, interpolatedString))
            {
                return;
            }
            
            // 检查字符串内插表达式
            var diagnostic = Diagnostic.Create(
                CustomStringConcatAnalyzerRule.Rule, 
                interpolatedString.GetLocation(), 
                "字符串内插");
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsStringType(ITypeSymbol? type)
        {
            return type != null && type.ToString() == "string";
        }

        private bool HasIgnoreStringConcatAttribute(SyntaxNodeAnalysisContext context, SyntaxNode syntaxNode)
        {
            const string IGNORE_STRING_CONCAT_ATTRIBUTE = "Game.IgnoreStringConcatAttribute";
            
            // 检查包含该语法节点的类是否有IgnoreStringConcatAttribute特性
            var classDeclaration = syntaxNode.GetParentClassDeclaration();
            if (classDeclaration != null)
            {
                var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
                if (classSymbol != null && classSymbol.HasAttribute(IGNORE_STRING_CONCAT_ATTRIBUTE))
                {
                    return true;
                }
            }
            
            // 检查包含该语法节点的方法是否有IgnoreStringConcatAttribute特性
            var methodDeclaration = syntaxNode.GetNeareastAncestor<MethodDeclarationSyntax>();
            if (methodDeclaration != null)
            {
                var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);
                if (methodSymbol != null && methodSymbol.HasAttribute(IGNORE_STRING_CONCAT_ATTRIBUTE))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
