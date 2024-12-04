using System.Collections.Generic;
using System.Text;
using ET.Analyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ET.Generator;

[Generator(LanguageNames.CSharp)]
public class ETCodeTypesCollectionGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => ETCodeTypesCollectionSyntaxContextReceiver.Create());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not ETCodeTypesCollectionSyntaxContextReceiver receiver || receiver.allTypesAccess.Count < 1 && receiver.allTypesReflect.Count < 1)
        {
            return;
        }
        string assemblyName = context.Compilation.AssemblyName ?? string.Empty;
        string code = $$"""
using System;
using System.Collections.Generic;
namespace ET
{
    public class CodeTypesCollection_{{assemblyName}}
    {
        public static void AddCodeTypes(ref Dictionary<string, Type> allTypes, ref UnOrderMultiMapSet<Type, Type> types)
        {
{{GenerateAllTypesContent(receiver)}}
{{GenerateTypesContent(receiver)}}
        }
    }
}
""";
        context.AddSource($"ET.{assemblyName}.CodeTypesCollection.g.cs", code);
    }

    private string GenerateAllTypesContent(ETCodeTypesCollectionSyntaxContextReceiver receiver)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("            Type type = null;");
        foreach (var type in receiver.allTypesAccess)
        {
            sb.AppendLine($"            type = typeof({type});");
            sb.AppendLine($"            allTypes[type.FullName] = type;");
        }
        foreach (var type in receiver.allTypesReflect)
        {
            sb.AppendLine($$"""            type = System.Type.GetType("{{type}}");""");
            sb.AppendLine($"            allTypes[type.FullName] = type;");
        }
        return sb.ToString();
    }

    private string GenerateTypesContent(ETCodeTypesCollectionSyntaxContextReceiver receiver)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var types in receiver.typesAccess)
        {
            sb.AppendLine($"type = typeof({types.Key});");
            foreach (var type in types.Value)
            {
                sb.AppendLine($"            types.Add(type, typeof({type}));");
            }
        }
        foreach (var types in receiver.typesReflect)
        {
            sb.AppendLine($"type = typeof({types.Key});");
            foreach (var type in types.Value)
            {
                sb.AppendLine($$"""            types.Add(type, System.Type.GetType("{{type}}"));""");
            }
        }
        return sb.ToString();
    }
    
    class ETCodeTypesCollectionSyntaxContextReceiver : ISyntaxContextReceiver
    {
        internal static ISyntaxContextReceiver Create()
        {
            return new ETCodeTypesCollectionSyntaxContextReceiver();
        }
        
        public HashSet<string> allTypesAccess = new HashSet<string>();
        public HashSet<string> allTypesReflect = new HashSet<string>();
        public Dictionary<string, HashSet<string>> typesAccess = new Dictionary<string, HashSet<string>>();
        public Dictionary<string, HashSet<string>> typesReflect = new Dictionary<string, HashSet<string>>();
        
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.SemanticModel.Compilation.AssemblyName, AnalyzeAssembly.All))
            {
                return;
            }

            if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax)
            {
                return;
            }
            
            var classTypeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
            if (classTypeSymbol == null || classTypeSymbol.IsGenericType)
            {
                return;
            }

            if (classTypeSymbol.DeclaredAccessibility == Accessibility.Public || classTypeSymbol.DeclaredAccessibility == Accessibility.Internal)
            {
                allTypesAccess.Add(classTypeSymbol.ToString());
            }
            else
            {
                allTypesReflect.Add(classTypeSymbol.ToString());
            }

            if (classTypeSymbol.IsAbstract)
            {
                return;
            }

            foreach (AttributeData? attributeData in classTypeSymbol.GetAttributes())
            {
                INamedTypeSymbol? attributeType = attributeData.AttributeClass;
                while (attributeType != null)
                {
                    if (attributeType.ToString() == "ET.BaseAttribute")
                    {
                        if (attributeData.AttributeClass != null)
                        {
                            var type = attributeData.AttributeClass.ToString();
                            if (classTypeSymbol.DeclaredAccessibility == Accessibility.Public || classTypeSymbol.DeclaredAccessibility == Accessibility.Internal)
                            {
                                if (!typesAccess.ContainsKey(type))
                                {
                                    typesAccess.Add(type, new HashSet<string>());
                                }
                                typesAccess[type].Add(classTypeSymbol.ToString());
                            }
                            else
                            {
                                if (!typesReflect.ContainsKey(type))
                                {
                                    typesReflect.Add(type, new HashSet<string>());
                                }
                                typesReflect[type].Add(classTypeSymbol.ToString());
                            }
                            break;
                        }
                    }
                    attributeType = attributeType.BaseType;
                }
            }
        }
    }
}