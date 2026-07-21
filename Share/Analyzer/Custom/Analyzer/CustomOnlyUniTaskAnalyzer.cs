using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ET.Analyzer.Custom
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CustomOnlyUniTaskAnalyzer: DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
                ImmutableArray.Create(CustomOnlyUniTaskAnalyzerRule.Rule);
        
        public override void Initialize(AnalysisContext context)
        {
            if (!AnalyzerGlobalSetting.EnableAnalyzer)
            {
                return;
            }
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(this.Analyzer, SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement);
        }
        
        private void Analyzer(SyntaxNodeAnalysisContext context)
        {
            if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.All))
            {
                return;
            }
            if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.IgnorePathNames))
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

            bool violatesRule = methodSymbol.IsAsync
                    ? !(namespaceName == "Cysharp.Threading.Tasks" && returnType.Name.StartsWith("UniTask"))
                    : namespaceName == "System.Threading.Tasks" && returnType.Name.StartsWith("Task");
            if (!violatesRule)
            {
                return;
            }

            // A method cannot change the return type imposed by an interface or a base
            // virtual method. In that case the inherited contract takes precedence over
            // the project's UniTask convention.
            if (methodSymbol.OverriddenMethod != null &&
                SymbolEqualityComparer.Default.Equals(returnType, methodSymbol.OverriddenMethod.ReturnType))
            {
                return;
            }

            foreach (IMethodSymbol interfaceMethod in methodSymbol.ExplicitInterfaceImplementations)
            {
                if (SymbolEqualityComparer.Default.Equals(returnType, interfaceMethod.ReturnType))
                {
                    return;
                }
            }

            INamedTypeSymbol? containingType = methodSymbol.ContainingType;
            if (containingType != null)
            {
                foreach (INamedTypeSymbol interfaceType in containingType.AllInterfaces)
                {
                    foreach (ISymbol interfaceMember in interfaceType.GetMembers(methodSymbol.Name))
                    {
                        if (interfaceMember is not IMethodSymbol interfaceMethod)
                        {
                            continue;
                        }

                        ISymbol? implementation = containingType.FindImplementationForInterfaceMember(interfaceMethod);
                        if (SymbolEqualityComparer.Default.Equals(implementation, methodSymbol) &&
                            SymbolEqualityComparer.Default.Equals(returnType, interfaceMethod.ReturnType))
                        {
                            return;
                        }
                    }
                }
            }

            Diagnostic diagnostic = Diagnostic.Create(CustomOnlyUniTaskAnalyzerRule.Rule, context.Node.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
