using System;
using System.Collections.Generic;
using System.Linq;
using ET.Analyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ET.Generator;

[Generator(LanguageNames.CSharp)]
public class ETSystemGenerator: IIncrementalGenerator
{
    private static readonly AttributeTemplate Templates = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> declarations = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => IsCandidateClass(node),
            static (generatorContext, _) => (ClassDeclarationSyntax)generatorContext.Node);

        context.RegisterSourceOutput(
            declarations.Combine(context.CompilationProvider),
            static (sourceContext, value) => GenerateCSFiles(value.Left, sourceContext, value.Right));
    }

    private static bool IsCandidateClass(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax classDeclaration)
        {
            return false;
        }

        return classDeclaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Any(static method => HasSupportedAttribute(method));
    }

    private static bool HasSupportedAttribute(MethodDeclarationSyntax method)
    {
        return method.AttributeLists.Any(static list =>
        {
            AttributeSyntax? attribute = list.Attributes.FirstOrDefault();
            return attribute != null && Templates.Contains(attribute.Name.ToString());
        });
    }

    /// <summary>
    /// 每个静态类生成一个cs文件
    /// </summary>
    private static void GenerateCSFiles(ClassDeclarationSyntax classDeclarationSyntax, SourceProductionContext context, Compilation compilation)
    {
        HashSet<MethodDeclarationSyntax> methodDeclarationSyntaxes = new(classDeclarationSyntax.Members
                .OfType<MethodDeclarationSyntax>()
                .Where(static method => HasSupportedAttribute(method)));
        string className = classDeclarationSyntax.Identifier.Text;
        SemanticModel semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
        INamedTypeSymbol? classTypeSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
        if (classTypeSymbol == null)
        {
            return;
        }

        if (!classTypeSymbol.IsStatic || !classDeclarationSyntax.IsPartial())
        {
            Diagnostic diagnostic = Diagnostic.Create(ETSystemMethodIsInStaticPartialClassRule.Rule, classDeclarationSyntax.GetLocation(),
                classDeclarationSyntax.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
            return;
        }

        INamespaceSymbol? namespaceSymbol = classTypeSymbol?.ContainingNamespace;
        string? namespaceName = namespaceSymbol?.Name;
        while (namespaceSymbol?.ContainingNamespace != null)
        {
            namespaceSymbol = namespaceSymbol.ContainingNamespace;
            if (string.IsNullOrEmpty(namespaceSymbol.Name))
            {
                break;
            }

            namespaceName = $"{namespaceSymbol.Name}.{namespaceName}";
        }

        if (namespaceName == null)
        {
            throw new Exception($"{className} namespace is null");
        }

        GenerateSystemCodeByTemplate(namespaceName, className, methodDeclarationSyntaxes, context, semanticModel);
    }

    /// <summary>
    /// 根据模板生成System代码
    /// </summary>
    private static void GenerateSystemCodeByTemplate(string namespaceName, string className,
    HashSet<MethodDeclarationSyntax> methodDeclarationSyntaxes, SourceProductionContext context, SemanticModel semanticModel)
    {
        foreach (MethodDeclarationSyntax? methodDeclarationSyntax in methodDeclarationSyntaxes)
        {
            IMethodSymbol? methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax) as IMethodSymbol;
            if (methodSymbol == null)
            {
                continue;
            }

            ParameterSyntax? componentParam = methodDeclarationSyntax.ParameterList.Parameters.FirstOrDefault();
            if (componentParam == null)
            {
                continue;
            }

            string methodName = methodDeclarationSyntax.Identifier.Text;
            string? componentName = componentParam.Type?.ToString();

            List<string> argsTypesList = new List<string>();
            List<string> argsTypeVarsList = new List<string>();
            List<string> argsVarsList = new List<string>();
            List<string> argsTypesWithout0List = new List<string>();
            List<string> argsTypeVarsWithout0List = new List<string>();
            List<string> argsVarsWithout0List = new List<string>();
            for (int i = 0; i < methodSymbol.Parameters.Length; i++)
            {
                string type = methodSymbol.Parameters[i].Type.ToString();
                type = type.Trim();
                if (type == "")
                {
                    continue;
                }
                string name = $"{methodSymbol.Parameters[i].Name}";
                
                argsTypesList.Add(type);
                argsVarsList.Add(name);
                string typeName = $"{type} {name}";
                argsTypeVarsList.Add(typeName);

                if (i != 0)
                {
                    argsTypesWithout0List.Add(type);
                    argsTypeVarsWithout0List.Add(typeName);
                    argsVarsWithout0List.Add(name);
                }
            }
            
            foreach (AttributeListSyntax attributeListSyntax in methodDeclarationSyntax.AttributeLists)
            {
                AttributeSyntax? attribute = attributeListSyntax.Attributes.FirstOrDefault();
                if (attribute == null)
                {
                    continue;
                }

                string attributeType = attribute.Name.ToString();
                string attributeString = $"[{attribute.ToString()}]";
                    
                string template = Templates.Get(attributeType);
                    
                string code = $$"""
namespace {{namespaceName}}
{
    public static partial class {{className}}
    {
        {{template}}
    }
}
""";

                string argsVars = string.Join(", ", argsVarsList);
                string argsTypes = string.Join(", ", argsTypesList);
                string argsTypesVars = string.Join(", ", argsTypeVarsList);
                string argsTypesUnderLine = string.Join("_", argsTypesList).Replace(", ", "_").Replace(".", "_")
                        .Replace("<", "_").Replace(">", "_").Replace("[]","Array").Replace("(","_").Replace(")","_");
                string argsTypesWithout0 = string.Join(", ", argsTypesWithout0List);
                string argsVarsWithout0 = string.Join(", ", argsVarsWithout0List);
                string argsTypesVarsWithout0 = string.Join(", ", argsTypeVarsWithout0List);

                SpecialProcessForArgs();
                
                if (methodSymbol.ReturnType.ToDisplayString() == "void")
                {
                    code = code.Replace("$returnType$", "void");
                    code = code.Replace("$return$", "");
                }else{
                    code = code.Replace("$returnType$", methodSymbol.ReturnType.ToDisplayString());
                    code = code.Replace("$return$", "return ");
                }
                
                code = code.Replace("$attribute$", attributeString);
                code = code.Replace("$attributeType$", attributeType);
                code = code.Replace("$methodName$", methodName);
                code = code.Replace("$className$", className);
                code = code.Replace("$entityType$", componentName);
                code = code.Replace("$argsTypes$", argsTypes);
                code = code.Replace("$argsTypesUnderLine$", argsTypesUnderLine);
                code = code.Replace("$argsTypesVars$", argsTypesVars);
                code = code.Replace("$argsVars$", argsVars);
                code = code.Replace("$argsTypesWithout0$", argsTypesWithout0);
                code = code.Replace("$argsVarsWithout0$", argsVarsWithout0);
                code = code.Replace("$argsTypesVarsWithout0$", argsTypesVarsWithout0);

                for (int i = 0; i < argsTypesList.Count; ++i)
                {
                    code = code.Replace($"$argsTypes{i}$", argsTypesList[i]);
                    code = code.Replace($"$argsVars{i}$", argsVarsList[i]);
                }

                string fileName = $"{namespaceName}.{className}.{methodName}.{argsTypesUnderLine}.g.cs";
                
                context.AddSource(fileName, code);
                
                void SpecialProcessForArgs()
                {
                    if ((attributeType == "EntitySystem" || attributeType == "LSEntitySystem" ||
                            attributeType == "UGFUIFormSystem" || attributeType == "UGFUIWidgetSystem" || attributeType == "UGFEntitySystem")
                        && methodName == Definition.GetComponentMethod)
                    {
                        argsTypes = argsTypes.Split(',')[0];
                    }
                }
            }
        }
    }
}