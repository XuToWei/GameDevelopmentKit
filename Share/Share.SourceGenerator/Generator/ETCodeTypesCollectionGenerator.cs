// using System.Collections.Generic;
// using System.Text;
// using ET.Analyzer;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.CSharp;
// using Microsoft.CodeAnalysis.CSharp.Syntax;
//
// namespace ET.Generator;
//
// [Generator(LanguageNames.CSharp)]
// public class ETCodeTypesCollectionGenerator : ISourceGenerator
// {
//     public void Initialize(GeneratorInitializationContext context)
//     {
//         context.RegisterForSyntaxNotifications(() => ETCodeTypesCollectionSyntaxContextReceiver.Create());
//     }
//
//     public void Execute(GeneratorExecutionContext context)
//     {
//         if (context.SyntaxContextReceiver is not ETCodeTypesCollectionSyntaxContextReceiver receiver || receiver.allTypes.Count == 0)
//         {
//             return;
//         }
//
//         string code = $$"""
// using System;
// using System.Collections.Generic;
// namespace ET
// {
//     public class CodeTypesCollection
//     {
//         public static void AddCodeTypes(ref Dictionary<string, Type> allTypes, ref UnOrderMultiMapSet<Type, Type> types)
//         {
// {{GenerateAllTypesContent(receiver)}}
// {{GenerateTypesContent(receiver)}}
//         }
//     }
// }
// """;
//         string assemblyName = context.Compilation.AssemblyName ?? string.Empty;
//         context.AddSource($"ET.{assemblyName}.CodeTypesCollection.g.cs",code);
//     }
//
//     private string GenerateAllTypesContent(ETCodeTypesCollectionSyntaxContextReceiver receiver)
//     {
//         StringBuilder sb = new StringBuilder();
//         sb.AppendLine("            Type type = null;");
//         foreach (var type in receiver.allTypes)
//         {
//             sb.AppendLine($"            type = typeof({type});");
//             sb.AppendLine($"            allTypes[type.FullName] = type;");
//         }
//         return sb.ToString();
//     }
//
//     private string GenerateTypesContent(ETCodeTypesCollectionSyntaxContextReceiver receiver)
//     {
//         StringBuilder sb = new StringBuilder();
//         foreach (var types in receiver.types)
//         {
//             foreach (var type in types.Value)
//             {
//                 sb.AppendLine($"            types.Add(typeof({types.Key}), typeof({type}));");
//             }
//         }
//         return sb.ToString();
//     }
//     
//     class ETCodeTypesCollectionSyntaxContextReceiver : ISyntaxContextReceiver
//     {
//         internal static ISyntaxContextReceiver Create()
//         {
//             return new ETCodeTypesCollectionSyntaxContextReceiver();
//         }
//         
//         public HashSet<string> allTypes = new HashSet<string>();
//         public Dictionary<string, HashSet<string>> types = new Dictionary<string, HashSet<string>>();
//         
//         public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
//         {
//             if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.SemanticModel.Compilation.AssemblyName, AnalyzeAssembly.All))
//             {
//                 return;
//             }
//
//             if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax)
//             {
//                 return;
//             }
//             
//             var classTypeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
//             if (classTypeSymbol == null)
//             {
//                 return;
//             }
//
//             var baseType = classTypeSymbol.BaseType;
//
//             if (baseType == null || classTypeSymbol.IsUnboundGenericType)
//             {
//                 return;
//             }
//
//             allTypes.Add(baseType.ToString());
//
//             if (classTypeSymbol.IsAbstract)
//             {
//                 return;
//             }
//
//             foreach (AttributeData? attributeData in classTypeSymbol.GetAttributes())
//             {
//                 if (attributeData.AttributeClass != null && attributeData.AttributeClass.HasAttributeInTypeAndBaseTyes("ET.BaseAttribute"))
//                 {
//                     var type = attributeData.AttributeClass.ToString();
//                     if (!types.ContainsKey(type))
//                     {
//                         types.Add(type, new HashSet<string>());
//                     }
//                     types[type].Add(classTypeSymbol.ToString());
//                 }
//             }
//         }
//     }
// }