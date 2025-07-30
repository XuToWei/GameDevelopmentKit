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
             context.RegisterSyntaxNodeAction(this.PropertyDeclarationAnalyzeNode, SyntaxKind.PropertyDeclaration);
             context.RegisterSyntaxNodeAction(this.LocalDeclarationStatementAnalyzeNode, SyntaxKind.LocalDeclarationStatement);
             context.RegisterSyntaxNodeAction(this.LocalFunctionStatementAnalyzeNode, SyntaxKind.LocalFunctionStatement);
             context.RegisterSyntaxNodeAction(this.MethodDeclarationAnalyzeNode, SyntaxKind.MethodDeclaration);
         }

         private void ClassDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.GameAll))
             {
                 return;
             }
             if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.CustomIgnorePathNames))
             {
                 return;
             }
             if (context.Node is not ClassDeclarationSyntax declaration)
             {
                 return;
             }
             const string TYPE_NAME = "class";
             var name = declaration.Identifier.Text;
             if (!name.CheckUpperDeclaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
             else if (!name.CheckEndCant_Declaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
         }

         private void StructDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.GameAll))
             {
                 return;
             }
             if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.CustomIgnorePathNames))
             {
                 return;
             }
             if (context.Node is not StructDeclarationSyntax declaration)
             {
                 return;
             }
             const string TYPE_NAME = "struct";
             var name = declaration.Identifier.Text;
             if (!name.CheckUpperDeclaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
             else if (!name.CheckEndCant_Declaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
         }
 
         private void InterfaceDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.GameAll))
             {
                 return;
             }
             if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.CustomIgnorePathNames))
             {
                 return;
             }
             if (context.Node is not InterfaceDeclarationSyntax declaration)
             {
                 return;
             }
             const string TYPE_NAME = "interface";
             var name = declaration.Identifier.Text;
             if (!name.CheckUpperDeclaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
             else if (!name.CheckEndCant_Declaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
         }

         private void EnumDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.GameAll))
             {
                 return;
             }
             if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.CustomIgnorePathNames))
             {
                 return;
             }
             if (context.Node is not EnumDeclarationSyntax declaration)
             {
                 return;
             }
             const string TYPE_NAME = "enum";
             var name = declaration.Identifier.Text;
             if (!name.CheckUpperDeclaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
             else if (!name.CheckEndCant_Declaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
         }

         private void FieldDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             const string TYPE_NAME = "field";
             const string IGNORE_FIELD_ATTRIBUTE = "Game.IgnoreFieldDeclarationAttribute";

             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.GameAll))
             {
                 return;
             }
             if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.CustomIgnorePathNames))
             {
                 return;
             }
             if (context.Node is not FieldDeclarationSyntax declaration)
             {
                 return;
             }
             foreach (var variable in declaration.Declaration.Variables)
             {
                var symbol = context.SemanticModel.GetDeclaredSymbol(variable);
                if (symbol != null && symbol.HasAttribute(IGNORE_FIELD_ATTRIBUTE))
                {
                    continue;
                }
                var name = variable.Identifier.Text;
                if (!name.CheckEndCant_Declaration())
                {
                    var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, variable.Identifier.GetLocation(), TYPE_NAME, name);
                    context.ReportDiagnostic(diagnostic);
                }
                else if (declaration.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    if (!name.CheckUpperDeclaration())
                    {
                        var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, variable.Identifier.GetLocation(), TYPE_NAME, name);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                else if (declaration.Modifiers.Any(SyntaxKind.PublicKeyword) || declaration.Modifiers.Any(SyntaxKind.InternalKeyword))
                {
                    if (!name.CheckUpperDeclaration())
                    {
                        var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, variable.Identifier.GetLocation(), TYPE_NAME, name);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                else if (declaration.Modifiers.Any(SyntaxKind.PrivateKeyword) || declaration.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    if (!name.CheckLowerDeclaration())
                    {
                        var diagnostic = Diagnostic.Create(CustomDeclarationLowerAnalyzerRule.Rule, variable.Identifier.GetLocation(), TYPE_NAME, name);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
             }
         }

         private void PropertyDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             const string TYPE_NAME = "property";
             const string IGNORE_PROPERTY_ATTRIBUTE = "Game.IgnorePropertyDeclarationAttribute";
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.GameAll))
             {
                 return;
             }
             if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.CustomIgnorePathNames))
             {
                 return;
             }
             if (context.Node is not PropertyDeclarationSyntax declaration)
             {
                 return;
             }
             if (declaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
             {
                 return;
             }
             var symbol = context.SemanticModel.GetDeclaredSymbol(declaration);
             if (symbol != null && symbol.HasAttribute(IGNORE_PROPERTY_ATTRIBUTE))
             {
                 return;
             }
             var name = declaration.Identifier.Text;
             if (!name.CheckEndCant_Declaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
             else if (declaration.Modifiers.Any(SyntaxKind.PublicKeyword) || declaration.Modifiers.Any(SyntaxKind.InternalKeyword))
             {
                 if (!name.CheckUpperDeclaration())
                 {
                     var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                     context.ReportDiagnostic(diagnostic);
                 }
             }
             else if (declaration.Modifiers.Any(SyntaxKind.PrivateKeyword) || declaration.Modifiers.Any(SyntaxKind.ProtectedKeyword))
             {
                 if (!name.CheckLowerDeclaration())
                 {
                     var diagnostic = Diagnostic.Create(CustomDeclarationLowerAnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                     context.ReportDiagnostic(diagnostic);
                 }
             }
         }

         private void LocalDeclarationStatementAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.GameAll))
             {
                 return;
             }
             if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.CustomIgnorePathNames))
             {
                 return;
             }
             if (context.Node is not LocalDeclarationStatementSyntax declaration)
             {
                 return;
             }
             const string TYPE_NAME = "local declaration";
             foreach (var variable in declaration.Declaration.Variables)
             {
                 var name = variable.Identifier.Text;
                 if (!name.CheckEndCant_Declaration())
                 {
                     var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, variable.Identifier.GetLocation(), TYPE_NAME, name);
                     context.ReportDiagnostic(diagnostic);
                 }
                 else if (declaration.Modifiers.Any(SyntaxKind.ConstKeyword))
                 {
                     if (!name.CheckUpperDeclaration())
                     {
                         var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, variable.Identifier.GetLocation(), TYPE_NAME, name);
                         context.ReportDiagnostic(diagnostic);
                     }
                 }
                 else if (!name.CheckLowerDeclaration())
                 {
                     var diagnostic = Diagnostic.Create(CustomDeclarationLowerAnalyzerRule.Rule, variable.Identifier.GetLocation(), TYPE_NAME, name);
                     context.ReportDiagnostic(diagnostic);
                 }
             }
         }

         private void LocalFunctionStatementAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.GameAll))
             {
                 return;
             }
             if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.CustomIgnorePathNames))
             {
                 return;
             }
             if (context.Node is not LocalFunctionStatementSyntax declaration)
             {
                 return;
             }
             const string TYPE_NAME = "local function";
             var name = declaration.Identifier.Text;
             if (!name.CheckEndCant_Declaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
             else if (!name.CheckUpperDeclaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
         }

         private void MethodDeclarationAnalyzeNode(SyntaxNodeAnalysisContext context)
         {
             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.Compilation.AssemblyName, CustomAnalyzeAssembly.GameAll))
             {
                 return;
             }
             if (context.Node.SyntaxTree.FilePath.IsFilterPath(AnalyzerGlobalSetting.CustomIgnorePathNames))
             {
                 return;
             }
             if (context.Node is not LocalFunctionStatementSyntax declaration)
             {
                 return;
             }
             const string TYPE_NAME = "method";
             var name = declaration.Identifier.Text;
             if (!name.CheckEndCant_Declaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationEndCant_AnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
             else if (!name.CheckUpperDeclaration())
             {
                 var diagnostic = Diagnostic.Create(CustomDeclarationUpperAnalyzerRule.Rule, declaration.Identifier.GetLocation(), TYPE_NAME, name);
                 context.ReportDiagnostic(diagnostic);
             }
         }
     }
}