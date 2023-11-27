using System.Collections.Generic;
using System.Linq;
using System.Text;
using ET.Analyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ET.Generator;

[Generator(LanguageNames.CSharp)]
public class ETGetComponentGenerator : ISourceGenerator
{
    
    public void Initialize(GeneratorInitializationContext context)
    {
        //context.RegisterForSyntaxNotifications(SyntaxContextReceiver.Create);
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxContextReceiver receiver || receiver.ComponentOfDatas.Count==0 )
        {
            return;
        }

        foreach (var kv in receiver.ComponentOfDatas)
        {
            GenerateCSFiles(context, kv.Key,kv.Value);
        }

    }
    
    private void GenerateCSFiles(GeneratorExecutionContext context,string nameSpace, List<ComponentOfData> componentOfDatas)
    {
        StringBuilder contenSb = new StringBuilder();

        foreach (var componentOfData in componentOfDatas)
        {
            GenerateGetComponentCode(context, componentOfData, contenSb);
        }

        string? assemblyName = context.Compilation.AssemblyName?.Replace(".", "_");
        string code = @"""
namespace {{nameSpace}}
{
    public static class {{assemblyName}}ETGetComponentExtension
    {
        {{contenSb}}
    }
}
""";
        code = code.Replace("{nameSpace}", nameSpace);
        code = code.Replace("{assemblyName}", assemblyName);
        code = code.Replace("{contenSb}", contenSb.ToString());
        context.AddSource($"ETGetComponentGenerator.{nameSpace}.g.cs", code);
    }

    private void GenerateGetComponentCode(GeneratorExecutionContext context,ComponentOfData componentOfData,StringBuilder contenSb)
    {
        string getComponentName = componentOfData.ComponentName;
        string parentEntityName = componentOfData.ParentEntityName;
        string code = @"""

        public static {{getComponentName}} Get{{getComponentName}} (this {{parentEntityName}} self)
        {
            return self.GetComponent<{{getComponentName}}>();
        }

""";
        code = code.Replace("{getComponentName}", getComponentName);
        code = code.Replace("{parentEntityName}", parentEntityName);
        contenSb.AppendLine(code);
    }

    struct ComponentOfData
    {
        public string ComponentName;

        public string ParentEntityName;
    }

    class SyntaxContextReceiver: ISyntaxContextReceiver
    {
        internal static ISyntaxContextReceiver Create()
        {
            return new SyntaxContextReceiver();
        }
        
        public Dictionary<string, List<ComponentOfData>> ComponentOfDatas = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            SyntaxNode node = context.Node;
            if (node is not ClassDeclarationSyntax classDeclarationSyntax)
            {
                return;
            }

            if (classDeclarationSyntax.AttributeLists.Count==0)
            {
                return;
            }
            
            var classTypeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
            if (classTypeSymbol==null)
            {
                return;
            }
            // 筛选出含有 ComponentOf标签的
            var componentOfAttrData = classTypeSymbol.GetFirstAttribute(Definition.ComponentOfAttribute);
            if (componentOfAttrData==null)
            {
                return;
            }

            // 忽略无Type参数的
            if (componentOfAttrData.ConstructorArguments[0].Value is not INamedTypeSymbol parentEntityTypeSymbol)
            {
                return;
            }
            
            
            string? nameSpace = classTypeSymbol.GetNameSpace();
            if (nameSpace==null)
            {
                return;
            }

            if (!ComponentOfDatas.ContainsKey(nameSpace))
            {
                ComponentOfDatas.Add(nameSpace,new());
            }

            this.ComponentOfDatas[nameSpace].Add(new ComponentOfData()
            {
                ComponentName = classTypeSymbol.Name,
                ParentEntityName = parentEntityTypeSymbol.Name,
            });
        }
    }
}

