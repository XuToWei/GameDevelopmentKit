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
     public class DeclarationAnalyzer: DiagnosticAnalyzer
     {
         public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
                 ImmutableArray.Create(DeclarationUpperAnalyzerRule.Rule);

         public override void Initialize(AnalysisContext context)
         {
             if (!AnalyzerGlobalSetting.EnableAnalyzer)
             {
                 return;
             }

             context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
             context.EnableConcurrentExecution();
             context.RegisterSyntaxNodeAction(this.ClassDeclarationAnalyzeNode, SyntaxKind.ClassDeclaration);
         }

         private void ClassDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.All))
             {
                 return;
             }
             if (context.Node is ClassDeclarationSyntax classDeclaration)
             {
                 var className = classDeclaration.Identifier.Text;
                 if (!char.IsUpper(className[0]))
                 {
                     var diagnostic = Diagnostic.Create(DeclarationUpperAnalyzerRule.Rule, classDeclaration.Identifier.GetLocation(), "Class Name", className);
                     context.ReportDiagnostic(diagnostic);
                 }
             }
         }
     }
}