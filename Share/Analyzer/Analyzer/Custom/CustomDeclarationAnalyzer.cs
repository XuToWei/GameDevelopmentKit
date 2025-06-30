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
     public class CustomDeclarationAnalyzer: DiagnosticAnalyzer
     {
         public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
                 ImmutableArray.Create(CustomDeclarationUpperAnalyzerRule.Rule, CustomDeclarationLowerAnalyzerRule.Rule, CustomDeclarationEndCant_AnalyzerRule.Rule);

         public override void Initialize(AnalysisContext context)
         {
             if (!AnalyzerGlobalSetting.EnableAnalyzer)
             {
                 return;
             }

             context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
             context.EnableConcurrentExecution();
             context.RegisterSyntaxNodeAction(this.ClassDeclarationAnalyzeNode, SyntaxKind.ClassDeclaration);
             context.RegisterSyntaxNodeAction(this.StructDeclarationAnalyzeNode, SyntaxKind.StructDeclaration);
             context.RegisterSyntaxNodeAction(this.InterfaceDeclarationAnalyzeNode, SyntaxKind.InterfaceDeclaration);
             context.RegisterSyntaxNodeAction(this.EnumDeclarationAnalyzeNode, SyntaxKind.EnumDeclaration);
             context.RegisterSyntaxNodeAction(this.FieldDeclarationAnalyzeNode, SyntaxKind.FieldDeclaration);
         }

         private void ClassDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.All))
             {
                 return;
             }
             if (context.Node is ClassDeclarationSyntax declaration)
             {
                 var name = declaration.Identifier.Text;
                 if (!CustomAnalyzerHelper.CheckUpperDeclaration(name))
                 {
                     var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, declaration.Identifier.GetLocation(), "class", name);
                     context.ReportDiagnostic(diagnostic);
                 }
                 else if (!CustomAnalyzerHelper.CheckEndCant_Declaration(name))
                 {
                     var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, declaration.Identifier.GetLocation(), "class", name);
                     context.ReportDiagnostic(diagnostic);
                 }
             }
         }

         private void StructDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.All))
             {
                 return;
             }
             if (context.Node is StructDeclarationSyntax declaration)
             {
                 var name = declaration.Identifier.Text;
                 if (!CustomAnalyzerHelper.CheckUpperDeclaration(name))
                 {
                     var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, declaration.Identifier.GetLocation(), "struct", name);
                     context.ReportDiagnostic(diagnostic);
                 }
                 else if (!CustomAnalyzerHelper.CheckEndCant_Declaration(name))
                 {
                     var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, declaration.Identifier.GetLocation(), "struct", name);
                     context.ReportDiagnostic(diagnostic);
                 }
             }
         }
 
         private void InterfaceDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.All))
             {
                 return;
             }
             if (context.Node is InterfaceDeclarationSyntax declaration)
             {
                 var name = declaration.Identifier.Text;
                 if (!CustomAnalyzerHelper.CheckUpperDeclaration(name))
                 {
                     var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, declaration.Identifier.GetLocation(), "interface", name);
                     context.ReportDiagnostic(diagnostic);
                 }
                 else if (!CustomAnalyzerHelper.CheckEndCant_Declaration(name))
                 {
                     var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, declaration.Identifier.GetLocation(), "interface", name);
                     context.ReportDiagnostic(diagnostic);
                 }
             }
         }

         private void EnumDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.All))
             {
                 return;
             }
             if (context.Node is EnumDeclarationSyntax declaration)
             {
                 var name = declaration.Identifier.Text;
                 if (!CustomAnalyzerHelper.CheckUpperDeclaration(name))
                 {
                     var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, declaration.Identifier.GetLocation(), "enum", name);
                     context.ReportDiagnostic(diagnostic);
                 }
                 else if (!CustomAnalyzerHelper.CheckEndCant_Declaration(name))
                 {
                     var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, declaration.Identifier.GetLocation(), "enum", name);
                     context.ReportDiagnostic(diagnostic);
                 }
             }
         }

         private void FieldDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.All))
             {
                 return;
             }
             if (context.Node is FieldDeclarationSyntax declaration)
             {
                 foreach (var variable in declaration.Declaration.Variables)
                 {
                    var name = variable.Identifier.Text;
                    if (declaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
                    {
                        continue;
                    }
                    if (!CustomAnalyzerHelper.CheckEndCant_Declaration(name))
                    {
                        var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, variable.Identifier.GetLocation(), "field", name);
                        context.ReportDiagnostic(diagnostic);
                        continue;
                    }
                    if (declaration.Modifiers.Any(SyntaxKind.PrivateKeyword) || declaration.Modifiers.Any(SyntaxKind.PropertyKeyword))
                    {
                        if (!CustomAnalyzerHelper.CheckLowerDeclaration(name))
                        {
                            var diagnostic = Diagnostic.Create(CustomDeclarationLowerAnalyzerRule.Rule, variable.Identifier.GetLocation(), "field", name);
                            context.ReportDiagnostic(diagnostic);
                        }
                        continue;
                    }
                    if (declaration.Modifiers.Any(SyntaxKind.PublicKeyword) || declaration.Modifiers.Any(SyntaxKind.InternalKeyword))
                    {
                        if (!CustomAnalyzerHelper.CheckUpperDeclaration(name))
                        {
                            var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, variable.Identifier.GetLocation(), "field", name);
                            context.ReportDiagnostic(diagnostic);
                        }
                        continue;
                    }
                 }
             }
         }
     }
}