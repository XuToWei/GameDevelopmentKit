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
//         context.RegisterForSyntaxNotifications(() => ETCodeTypesCollectionSyntaxContextReceiver.Create(new AttributeClassNameTemplate()));
//     }
//
//     public void Execute(GeneratorExecutionContext context)
//     {
//         if (context.SyntaxContextReceiver is not ETCodeTypesCollectionSyntaxContextReceiver receiver
//             || receiver.allTypes.Count < 1)
//         {
//             return;
//         }
//         string assemblyName = context.Compilation.AssemblyName ?? string.Empty;
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
//         context.AddSource($"ET.{assemblyName}.CodeTypesCollection.g.cs", code);
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
//             sb.AppendLine($"            type = typeof({types.Key});");
//             foreach (var type in types.Value)
//             {
//                 sb.AppendLine($"            types.Add(type, typeof({type}));");
//             }
//         }
//         return sb.ToString();
//     }
//
//     class ETCodeTypesCollectionSyntaxContextReceiver : ISyntaxContextReceiver
//     {
//         internal static ISyntaxContextReceiver Create(AttributeClassNameTemplate template)
//         {
//             return new ETCodeTypesCollectionSyntaxContextReceiver(template);
//         }
//
//         ETCodeTypesCollectionSyntaxContextReceiver(AttributeClassNameTemplate template)
//         {
//             this.template = template;
//         }
//         
//         private AttributeClassNameTemplate template;
//         public HashSet<string> allTypes = new ();
//         public Dictionary<string, HashSet<string>> types = new ();
//         
//         public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
//         {
//             DoTypeCollection(context);
//             DoSystemTypeCollection(context);
//         }
//
//         private void DoTypeCollection(GeneratorSyntaxContext context)
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
//             if (classTypeSymbol == null || classTypeSymbol.IsGenericType)
//             {
//                 return;
//             }
//
//             allTypes.Add(classTypeSymbol.ToString());
//
//             if (classTypeSymbol.IsAbstract)
//             {
//                 return;
//             }
//
//             foreach (AttributeData? attributeData in classTypeSymbol.GetAttributes())
//             {
//                 INamedTypeSymbol? attributeType = attributeData.AttributeClass;
//                 while (attributeType != null)
//                 {
//                     if (attributeType.ToString() == "ET.BaseAttribute")
//                     {
//                         if (attributeData.AttributeClass != null)
//                         {
//                             var type = attributeData.AttributeClass.ToString();
//                             if (!types.ContainsKey(type))
//                             {
//                                 types.Add(type, new HashSet<string>());
//                             }
//                             types[type].Add(classTypeSymbol.ToString());
//                             break;
//                         }
//                     }
//                     attributeType = attributeType.BaseType;
//                 }
//             }
//         }
//
//         private void DoSystemTypeCollection(GeneratorSyntaxContext context)
//         {
//             SyntaxNode node = context.Node;
//             if (node is not MethodDeclarationSyntax methodDeclarationSyntax)
//             {
//                 return;
//             }
//
//             if (methodDeclarationSyntax.AttributeLists.Count == 0)
//             {
//                 return;
//             }
//
//             bool found = false;
//             foreach (AttributeListSyntax attributeListSyntax in methodDeclarationSyntax.AttributeLists)
//             {
//                 AttributeSyntax? attribute = attributeListSyntax.Attributes.FirstOrDefault();
//                 if (attribute == null)
//                 {
//                     return;
//                 }
//
//                 string attributeName = attribute.Name.ToString();
//
//                 if (this.template.Contains(attributeName))
//                 {
//                     found = true;
//                 }
//             }
//
//             if (!found)
//             {
//                 return;
//             }
//
//             ClassDeclarationSyntax? parentClass = methodDeclarationSyntax.GetParentClassDeclaration();
//             if (parentClass == null)
//             {
//                 return;
//             }
//
//             string className = parentClass.Identifier.Text;
//             SemanticModel semanticModel = context.SemanticModel.Compilation.GetSemanticModel(parentClass.SyntaxTree);
//             INamedTypeSymbol? classTypeSymbol = semanticModel.GetDeclaredSymbol(parentClass);
//             
//             if (classTypeSymbol == null)
//             {
//                 return;
//             }
//
//             if (!classTypeSymbol.IsStatic || !parentClass.IsPartial())
//             {
//                 return;
//             }
//
//             INamespaceSymbol? namespaceSymbol = classTypeSymbol.ContainingNamespace;
//             string? namespaceName = namespaceSymbol?.Name;
//             while (namespaceSymbol?.ContainingNamespace != null)
//             {
//                 namespaceSymbol = namespaceSymbol.ContainingNamespace;
//                 if (string.IsNullOrEmpty(namespaceSymbol.Name))
//                 {
//                     break;
//                 }
//
//                 namespaceName = $"{namespaceSymbol.Name}.{namespaceName}";
//             }
//
//             if (namespaceName == null)
//             {
//                 return;
//             }
//
//             IMethodSymbol? methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
//             if (methodSymbol == null)
//             {
//                 return;
//             }
//             
//             ParameterSyntax? componentParam = methodDeclarationSyntax.ParameterList.Parameters.FirstOrDefault();
//             if (componentParam == null)
//             {
//                 return;
//             }
//             
//             string methodName = methodDeclarationSyntax.Identifier.Text;
//
//             List<string> argsTypesList = new List<string>();
//             List<string> argsTypeVarsList = new List<string>();
//             List<string> argsVarsList = new List<string>();
//             List<string> argsTypesWithout0List = new List<string>();
//             List<string> argsTypeVarsWithout0List = new List<string>();
//             List<string> argsVarsWithout0List = new List<string>();
//             for (int i = 0; i < methodSymbol.Parameters.Length; i++)
//             {
//                 string type = methodSymbol.Parameters[i].Type.ToString();
//                 type = type.Trim();
//                 if (type == "")
//                 {
//                     continue;
//                 }
//                 string name = $"{methodSymbol.Parameters[i].Name}";
//                 
//                 argsTypesList.Add(type);
//                 argsVarsList.Add(name);
//                 string typeName = $"{type} {name}";
//                 argsTypeVarsList.Add(typeName);
//
//                 if (i != 0)
//                 {
//                     argsTypesWithout0List.Add(type);
//                     argsTypeVarsWithout0List.Add(typeName);
//                     argsVarsWithout0List.Add(name);
//                 }
//             }
//
//             foreach (AttributeListSyntax attributeListSyntax in methodDeclarationSyntax.AttributeLists)
//             {
//                 AttributeSyntax? attribute = attributeListSyntax.Attributes.FirstOrDefault();
//                 if (attribute == null)
//                 {
//                     continue;
//                 }
//                 
//                 string attributeType = attribute.Name.ToString();
//                 string attributeString = $"ET.{attributeType}Attribute";
//                 
//                 string code = this.template.Get(attributeType);
//                 
//                 string argsTypes = string.Join(", ", argsTypesList);
//                 string argsTypesUnderLine = string.Join("_", argsTypesList).Replace(", ", "_").Replace(".", "_")
//                         .Replace("<", "_").Replace(">", "_").Replace("[]","Array").Replace("(","_").Replace(")","_");
//
//                 SpeicalProcessForArgs();
//                 
//                 code = code.Replace("$methodName$", methodName);
//                 code = code.Replace("$className$", className);
//                 code = code.Replace("$argsTypes$", argsTypes);
//                 code = code.Replace("$argsTypesUnderLine$", argsTypesUnderLine);
//
//                 for (int i = 0; i < argsTypesList.Count; ++i)
//                 {
//                     code = code.Replace($"$argsTypes{i}$", argsTypesList[i]);
//                 }
//                 
//                 void SpeicalProcessForArgs()
//                 {
//                     if ((attributeType == "EntitySystem" || attributeType == "LSEntitySystem") && methodName == Definition.GetComponentMethod)
//                     {
//                         argsTypes = argsTypes.Split(',')[0];
//                     }
//                 }
//
//                 code = $"{namespaceName}.{className}.{code}";
//                 allTypes.Add(code);
//                 
//                 if (!types.ContainsKey(attributeString))
//                 {
//                     types.Add(attributeString, new HashSet<string>());
//                 }
//                 types[attributeString].Add(code);
//             }
//         }
//     }
// }