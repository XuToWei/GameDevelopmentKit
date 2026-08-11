using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using ET.Analyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ET.Generator;

[Generator(LanguageNames.CSharp)]
public class ETEntitySerializeFormatterGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<string> entities = context.SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax,
                static (generatorContext, _) => GetEntityName(generatorContext))
                .Where(static entityName => entityName != null)
                .Select(static (entityName, _) => entityName!);

        IncrementalValueProvider<(ImmutableArray<string> Entities, string? AssemblyName)> source = entities
                .Collect()
                .Combine(context.CompilationProvider.Select(static (compilation, _) => compilation.AssemblyName));

        context.RegisterSourceOutput(source, static (sourceContext, value) => Execute(sourceContext, value.Entities, value.AssemblyName));
    }

    private static string? GetEntityName(GeneratorSyntaxContext context)
    {
        if (!AnalyzerHelper.IsAssemblyNeedAnalyze(context.SemanticModel.Compilation.AssemblyName, AnalyzeAssembly.AllLogicModel) ||
            context.Node is not ClassDeclarationSyntax classDeclarationSyntax ||
            context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classTypeSymbol)
        {
            return null;
        }

        string? baseType = classTypeSymbol.BaseType?.ToString();
        if ((baseType != Definition.EntityType && baseType != Definition.LSEntityType) ||
            !classTypeSymbol.HasAttribute("MemoryPack.MemoryPackableAttribute"))
        {
            return null;
        }

        return classTypeSymbol.ToString();
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<string> candidates, string? assemblyName)
    {
        List<string> entities = candidates
                .Distinct()
                .OrderBy(static entityName => entityName, System.StringComparer.Ordinal)
                .ToList();
        if (entities.Count == 0)
        {
            return;
        }

        int count = entities.Count;
        string typeHashCodeMapDeclaration = GenerateTypeHashCodeMapDeclaration(entities);
        string serializeContent = GenerateSerializeContent(entities);
        string deserializeContent = GenerateDeserializeContent(entities);
        string genericTypeParam = assemblyName == AnalyzeAssembly.DotNet_Model ? "<TBufferWriter>" : "";
        string scopedCode = assemblyName == AnalyzeAssembly.DotNet_Model ? "scoped" : "";
        string code = $$"""
#nullable enable
#pragma warning disable CS0108 // hides inherited member
#pragma warning disable CS0162 // Unreachable code
#pragma warning disable CS0164 // This label has not been referenced
#pragma warning disable CS0219 // Variable assigned but never used
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8601 // Possible null reference assignment
#pragma warning disable CS8602
#pragma warning disable CS8604 // Possible null reference argument for parameter
#pragma warning disable CS8619
#pragma warning disable CS8620
#pragma warning disable CS8631 // The type cannot be used as type parameter in the generic type or method
#pragma warning disable CS8765 // Nullability of type of parameter
#pragma warning disable CS9074 // The 'scoped' modifier of parameter doesn't match overridden or implemented member
#pragma warning disable CA1050 // Declare types in namespaces.

using System;
using MemoryPack;

[global::MemoryPack.Internal.Preserve]
public class ETEntitySerializeFormatter : MemoryPackFormatter<global::{{Definition.EntityType}}>
{
    static readonly System.Collections.Generic.Dictionary<Type, long> __typeToTag = new({{count}})
    {
{{typeHashCodeMapDeclaration}}
    };

    [global::MemoryPack.Internal.Preserve]
    public override void Serialize{{genericTypeParam}}(ref MemoryPackWriter{{genericTypeParam}} writer,{{scopedCode}} ref global::{{Definition.EntityType}}? value)
    {

        if (value == null)
        {
            writer.WriteNullUnionHeader();
            return;
        }

        if (__typeToTag.TryGetValue(value.GetType(), out var tag))
        {
            writer.WriteValue<byte>(global::MemoryPack.MemoryPackCode.WideTag);
            writer.WriteValue<long>(tag);
            switch (tag)
            {
{{serializeContent}}
                default:
                    break;
            }
        }
        else
        {
            MemoryPackSerializationException.ThrowNotFoundInUnionType(value.GetType(), typeof(global::{{Definition.EntityType}}));
        }
    }

    [global::MemoryPack.Internal.Preserve]
    public override void Deserialize(ref MemoryPackReader reader,{{scopedCode}} ref global::{{Definition.EntityType}}? value)
    {

        bool isNull = reader.ReadValue<byte>() == global::MemoryPack.MemoryPackCode.NullObject;
        if (isNull)
        {
            value = default;
            return;
        }

        var tag = reader.ReadValue<long>();

        switch (tag)
        {
{{deserializeContent}}
            default:
                //MemoryPackSerializationException.ThrowInvalidTag(tag, typeof(global::IForExternalUnion));
                break;
        }
    }
}
namespace ET
{
    public static partial class EntitySerializeRegister
    {
        static partial void Register()
        {
            if (!global::MemoryPack.MemoryPackFormatterProvider.IsRegistered<global::{{Definition.EntityType}}>())
            {
                global::MemoryPack.MemoryPackFormatterProvider.Register(new ETEntitySerializeFormatter());
            }
        }
    }
}
""";
        context.AddSource("ETEntitySerializeFormatterGenerator.g.cs", code);
    }

    private static string GenerateTypeHashCodeMapDeclaration(IEnumerable<string> entities)
    {
        StringBuilder sb = new();
        foreach (string entityName in entities)
        {
            sb.AppendLine($$"""        { typeof(global::{{entityName}}), {{entityName.GetLongHashCode()}} },""");
        }
        return sb.ToString();
    }

    private static string GenerateSerializeContent(IEnumerable<string> entities)
    {
        StringBuilder sb = new();
        foreach (string entityName in entities)
        {
            sb.AppendLine($$"""                case {{entityName.GetLongHashCode()}}: writer.WritePackable(System.Runtime.CompilerServices.Unsafe.As<global::{{Definition.EntityType}}?, global::{{entityName}}>(ref value)); break;""");
        }
        return sb.ToString();
    }

    private static string GenerateDeserializeContent(IEnumerable<string> entities)
    {
        StringBuilder sb = new();
        foreach (string entityName in entities)
        {
            sb.AppendLine($$"""
            case {{entityName.GetLongHashCode()}}:
                    if(value is global::{{entityName}})
                    {
                        reader.ReadPackable(ref System.Runtime.CompilerServices.Unsafe.As<global::{{Definition.EntityType}}?, global::{{entityName}}>(ref value));
                    }else{
                        value = (global::{{entityName}})reader.ReadPackable<global::{{entityName}}>();
                    }
                    break;
""");
        }
        return sb.ToString();
    }
}
