using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ET.Analyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ET.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class ETReactiveSystemGenerator: ISourceGenerator
{
    private const string SystemAttributeName = "ET.ETReactiveSystemAttribute";
    private const string EntitySystemAttributeName = "ET.EntitySystemOfAttribute";
    private const string SourceAttributeName = "ET.ETReactiveSourceAttribute";
    private const string BindAttributeName = "ET.ETReactiveBindAttribute";
    private const string EntityTypeName = "ET.Entity";
    private const string ReactiveHostInterfaceName = "ET.IETReactiveHost";
    private const string VersionInterfaceName = "ReactiveBinding.IVersion";
    private const string ObserveMethodName = "ObserveChanges";
    private const string ResetMethodName = "ResetReactive";
    private const string ClearMethodName = "ClearReactive";

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxReceiver receiver)
        {
            return;
        }

        GenerateReactiveHosts(context, receiver.HostDeclarations);
        if (receiver.SystemDeclarations.Count == 0)
        {
            return;
        }

        HashSet<INamedTypeSymbol> processedSystems = new(SymbolEqualityComparer.Default);
        List<SystemCandidate> candidates = new();
        foreach (ClassDeclarationSyntax declaration in receiver.SystemDeclarations)
        {
            SemanticModel semanticModel = context.Compilation.GetSemanticModel(declaration.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not INamedTypeSymbol system ||
                !processedSystems.Add(system))
            {
                continue;
            }

            INamedTypeSymbol? owner = null;
            if (TryGetSystemAttribute(system, out AttributeData? attribute) &&
                attribute != null)
            {
                TryGetOwner(system, attribute, out owner);
            }

            candidates.Add(new SystemCandidate(declaration, system, owner));
        }

        Dictionary<INamedTypeSymbol, int> ownerCounts = new(SymbolEqualityComparer.Default);
        foreach (SystemCandidate candidate in candidates)
        {
            if (candidate.Owner == null)
            {
                continue;
            }

            ownerCounts.TryGetValue(candidate.Owner, out int count);
            ownerCounts[candidate.Owner] = count + 1;
        }

        HashSet<INamedTypeSymbol> referencedOwners = GetReferencedReactiveOwners(context.Compilation);
        foreach (SystemCandidate candidate in candidates)
        {
            bool duplicateOwner = candidate.Owner != null &&
                                  (ownerCounts[candidate.Owner] > 1 || referencedOwners.Contains(candidate.Owner));
            GenerateSystem(context, candidate.Declaration, candidate.System, duplicateOwner);
        }
    }

    private static void GenerateReactiveHosts(
        GeneratorExecutionContext context,
        IReadOnlyList<ClassDeclarationSyntax> declarations)
    {
        if (declarations.Count == 0)
        {
            return;
        }

        INamedTypeSymbol? reactiveHostInterface = context.Compilation.GetTypeByMetadataName(ReactiveHostInterfaceName);
        INamedTypeSymbol? entityType = context.Compilation.GetTypeByMetadataName(EntityTypeName);
        IPropertySymbol? reactiveObserverProperty = reactiveHostInterface?.GetMembers("ReactiveObserver")
                .OfType<IPropertySymbol>()
                .FirstOrDefault();
        if (reactiveHostInterface == null || entityType == null || reactiveObserverProperty == null)
        {
            return;
        }

        HashSet<INamedTypeSymbol> processedHosts = new(SymbolEqualityComparer.Default);
        foreach (ClassDeclarationSyntax declaration in declarations)
        {
            SemanticModel semanticModel = context.Compilation.GetSemanticModel(declaration.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not INamedTypeSymbol host ||
                !processedHosts.Add(host))
            {
                continue;
            }

            if (!IsOrInheritsFrom(host, entityType))
            {
                Report(
                    context,
                    ETReactiveDiagnosticRules.OwnerInterface,
                    declaration.Identifier.GetLocation(),
                    host.ToDisplayString());
                continue;
            }

            if (!IsPartialTypeHierarchy(host))
            {
                Report(
                    context,
                    ETReactiveDiagnosticRules.HostDeclaration,
                    declaration.Identifier.GetLocation(),
                    host.ToDisplayString());
                continue;
            }

            if (host.FindImplementationForInterfaceMember(reactiveObserverProperty) != null)
            {
                continue;
            }

            context.AddSource(
                $"ETReactiveHostGenerator.{GetMetadataName(host)}.g.cs",
                EmitReactiveHost(host));
        }
    }

    private static void GenerateSystem(
        GeneratorExecutionContext context,
        ClassDeclarationSyntax declaration,
        INamedTypeSymbol system,
        bool duplicateOwner)
    {
        if (!TryGetSystemAttribute(system, out AttributeData? systemAttribute) || systemAttribute == null)
        {
            Report(
                context,
                ETReactiveDiagnosticRules.Owner,
                declaration.Identifier.GetLocation(),
                system.ToDisplayString());
            return;
        }

        bool valid = true;
        if (!system.IsStatic || system.Arity != 0 || system.ContainingType != null ||
            !declaration.Modifiers.Any(static modifier => modifier.IsKind(SyntaxKind.PartialKeyword)))
        {
            Report(
                context,
                ETReactiveDiagnosticRules.SystemDeclaration,
                declaration.Identifier.GetLocation(),
                system.ToDisplayString());
            valid = false;
        }

        if (!TryGetOwner(system, systemAttribute, out INamedTypeSymbol? owner) ||
            owner == null ||
            SymbolEqualityComparer.Default.Equals(owner.ContainingAssembly, context.Compilation.Assembly))
        {
            Report(
                context,
                ETReactiveDiagnosticRules.Owner,
                declaration.Identifier.GetLocation(),
                system.ToDisplayString());
            return;
        }

        if (duplicateOwner)
        {
            Report(
                context,
                ETReactiveDiagnosticRules.DuplicateOwner,
                declaration.Identifier.GetLocation(),
                owner.ToDisplayString());
            valid = false;
        }

        INamedTypeSymbol? reactiveHostInterface = context.Compilation.GetTypeByMetadataName(ReactiveHostInterfaceName);
        INamedTypeSymbol? entityType = context.Compilation.GetTypeByMetadataName(EntityTypeName);
        if (!IsOrInheritsFrom(owner, entityType) || !ImplementsInterface(owner, reactiveHostInterface))
        {
            Report(
                context,
                ETReactiveDiagnosticRules.OwnerInterface,
                declaration.Identifier.GetLocation(),
                owner.ToDisplayString());
            valid = false;
        }

        List<IMethodSymbol> sourceMethods = GetAttributedMethods(system, SourceAttributeName);
        List<IMethodSymbol> bindMethods = GetAttributedMethods(system, BindAttributeName);

        foreach (IGrouping<string, IMethodSymbol> duplicateGroup in sourceMethods
                     .GroupBy(static method => method.Name, StringComparer.Ordinal)
                     .Where(static group => group.Count() > 1))
        {
            IMethodSymbol duplicate = duplicateGroup.Skip(1).First();
            Report(
                context,
                ETReactiveDiagnosticRules.DuplicateSource,
                GetLocation(duplicate, declaration),
                duplicateGroup.Key,
                system.ToDisplayString());
            valid = false;
        }

        INamedTypeSymbol? versionInterface = context.Compilation.GetTypeByMetadataName(VersionInterfaceName);
        List<SourceModel> sources = new();
        foreach (IMethodSymbol method in sourceMethods)
        {
            if (!IsValidSourceSignature(method, owner))
            {
                Report(
                    context,
                    ETReactiveDiagnosticRules.SourceSignature,
                    GetLocation(method, declaration),
                    method.Name);
                valid = false;
                continue;
            }

            bool isVersioned = ImplementsInterface(method.ReturnType, versionInterface);
            if (!IsSupportedSourceType(method.ReturnType, isVersioned))
            {
                Report(
                    context,
                    ETReactiveDiagnosticRules.UnsupportedSourceType,
                    GetLocation(method, declaration),
                    method.Name,
                    method.ReturnType.ToDisplayString());
                valid = false;
                continue;
            }

            if (IsCustomStructWithoutEqualityOperator(method.ReturnType, isVersioned))
            {
                Report(
                    context,
                    ETReactiveDiagnosticRules.StructEquality,
                    GetLocation(method, declaration),
                    method.Name,
                    method.ReturnType.ToDisplayString());
                valid = false;
                continue;
            }

            sources.Add(new SourceModel(method, isVersioned));
        }

        sources.Sort(static (left, right) => StringComparer.Ordinal.Compare(left.Id, right.Id));
        Dictionary<string, SourceModel> sourceById = new(StringComparer.Ordinal);
        foreach (SourceModel source in sources)
        {
            if (!sourceById.ContainsKey(source.Id))
            {
                sourceById.Add(source.Id, source);
            }
        }

        List<BindModel> binds = new();
        foreach (IMethodSymbol method in bindMethods)
        {
            AttributeData bindAttribute = GetAttribute(method, BindAttributeName)!;
            List<string> reactiveIds = GetReactiveIds(bindAttribute);
            bool bindValid = true;

            if (reactiveIds.Count == 0 || reactiveIds.Any(static id => string.IsNullOrWhiteSpace(id)))
            {
                Report(
                    context,
                    ETReactiveDiagnosticRules.MissingSource,
                    GetLocation(method, declaration),
                    method.Name,
                    reactiveIds.Count == 0 ? "<empty>" : string.Join(", ", reactiveIds));
                bindValid = false;
            }

            List<string> duplicateIds = reactiveIds
                    .Where(static id => !string.IsNullOrWhiteSpace(id))
                    .GroupBy(static id => id, StringComparer.Ordinal)
                    .Where(static group => group.Count() > 1)
                    .Select(static group => group.Key)
                    .ToList();
            if (duplicateIds.Count > 0)
            {
                Report(
                    context,
                    ETReactiveDiagnosticRules.DuplicateBindSource,
                    GetLocation(method, declaration),
                    method.Name,
                    string.Join(", ", duplicateIds));
                bindValid = false;
            }

            List<string> missingIds = reactiveIds
                    .Where(id => !string.IsNullOrWhiteSpace(id) && !sourceById.ContainsKey(id))
                    .Distinct(StringComparer.Ordinal)
                    .ToList();
            if (missingIds.Count > 0)
            {
                Report(
                    context,
                    ETReactiveDiagnosticRules.MissingSource,
                    GetLocation(method, declaration),
                    method.Name,
                    string.Join(", ", missingIds));
                bindValid = false;
            }

            if (!IsValidBindSignature(method, owner, reactiveIds, sourceById))
            {
                Report(
                    context,
                    ETReactiveDiagnosticRules.BindSignature,
                    GetLocation(method, declaration),
                    method.Name,
                    GetExpectedBindSignature(owner, reactiveIds, sourceById));
                bindValid = false;
            }

            if (bindValid)
            {
                binds.Add(new BindModel(method, reactiveIds));
            }
            else
            {
                valid = false;
            }
        }

        binds.Sort(static (left, right) =>
        {
            int nameResult = StringComparer.Ordinal.Compare(left.Method.Name, right.Method.Name);
            return nameResult != 0
                    ? nameResult
                    : StringComparer.Ordinal.Compare(left.Method.ToDisplayString(), right.Method.ToDisplayString());
        });

        HashSet<string> usedSourceIds = new(
            binds.SelectMany(static bind => bind.ReactiveIds),
            StringComparer.Ordinal);
        foreach (SourceModel source in sources.Where(source => !usedSourceIds.Contains(source.Id)))
        {
            Report(
                context,
                ETReactiveDiagnosticRules.UnusedSource,
                GetLocation(source.Method, declaration),
                source.Id);
        }

        sources.RemoveAll(source => !usedSourceIds.Contains(source.Id));
        valid &= ValidateGeneratedMethodCollision(context, declaration, system, owner, ObserveMethodName);
        valid &= ValidateGeneratedMethodCollision(context, declaration, system, owner, ResetMethodName);
        valid &= ValidateGeneratedMethodCollision(context, declaration, system, owner, ClearMethodName);

        if (!valid)
        {
            return;
        }

        string observerTypeName = GetGeneratedObserverTypeName(system);
        string generatedCode = Emit(
            system,
            owner,
            observerTypeName,
            sources,
            binds);

        context.AddSource(
            $"ETReactiveSystemGenerator.{GetMetadataName(system)}.g.cs",
            generatedCode);
    }

    private static bool TryGetOwner(
        INamedTypeSymbol system,
        AttributeData attribute,
        out INamedTypeSymbol? owner)
    {
        owner = null;
        if (!string.Equals(attribute.AttributeClass?.ToDisplayString(), SystemAttributeName, StringComparison.Ordinal) ||
            attribute.ConstructorArguments.Length != 0)
        {
            return false;
        }

        AttributeData? entitySystemAttribute = GetAttribute(system, EntitySystemAttributeName);
        if (entitySystemAttribute == null ||
            entitySystemAttribute.ConstructorArguments.Length < 1 ||
            entitySystemAttribute.ConstructorArguments[0].Value is not INamedTypeSymbol inferredOwner)
        {
            return false;
        }

        owner = inferredOwner;
        return true;
    }

    private static List<IMethodSymbol> GetAttributedMethods(INamedTypeSymbol system, string attributeName)
    {
        return system.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(method => !method.IsImplicitlyDeclared && GetAttribute(method, attributeName) != null)
                .OrderBy(static method => method.Name, StringComparer.Ordinal)
                .ThenBy(static method => method.ToDisplayString(), StringComparer.Ordinal)
                .ToList();
    }

    private static bool IsValidSourceSignature(IMethodSymbol method, INamedTypeSymbol owner)
    {
        return method.MethodKind == MethodKind.Ordinary &&
               method.IsStatic &&
               method.Arity == 0 &&
               !method.ReturnsVoid &&
               method.RefKind == RefKind.None &&
               method.ReturnType.TypeKind != TypeKind.Error &&
               method.ReturnType.TypeKind != TypeKind.Pointer &&
               method.ReturnType.TypeKind != TypeKind.FunctionPointer &&
               !method.ReturnType.IsRefLikeType &&
               method.Parameters.Length == 1 &&
               method.Parameters[0].RefKind == RefKind.None &&
               SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, owner);
    }

    private static bool IsValidBindSignature(
        IMethodSymbol method,
        INamedTypeSymbol owner,
        IReadOnlyList<string> reactiveIds,
        IReadOnlyDictionary<string, SourceModel> sourceById)
    {
        if (method.MethodKind != MethodKind.Ordinary ||
            !method.IsStatic ||
            method.Arity != 0 ||
            !method.ReturnsVoid ||
            method.Parameters.Length == 0 ||
            method.Parameters.Any(static parameter => parameter.RefKind != RefKind.None) ||
            !SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, owner))
        {
            return false;
        }

        if (reactiveIds.Count == 0 || reactiveIds.Any(id => !sourceById.ContainsKey(id)))
        {
            return true;
        }

        int valueParameterCount = method.Parameters.Length - 1;
        if (valueParameterCount == 0)
        {
            return true;
        }

        if (valueParameterCount == reactiveIds.Count)
        {
            for (int index = 0; index < reactiveIds.Count; ++index)
            {
                if (!SymbolEqualityComparer.Default.Equals(
                        method.Parameters[index + 1].Type,
                        sourceById[reactiveIds[index]].Type))
                {
                    return false;
                }
            }

            return true;
        }

        if (valueParameterCount == reactiveIds.Count * 2)
        {
            if (reactiveIds.Any(id => sourceById[id].IsVersioned))
            {
                return false;
            }

            for (int index = 0; index < reactiveIds.Count; ++index)
            {
                ITypeSymbol sourceType = sourceById[reactiveIds[index]].Type;
                if (!SymbolEqualityComparer.Default.Equals(method.Parameters[index * 2 + 1].Type, sourceType) ||
                    !SymbolEqualityComparer.Default.Equals(method.Parameters[index * 2 + 2].Type, sourceType))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    private static string GetExpectedBindSignature(
        INamedTypeSymbol owner,
        IReadOnlyList<string> reactiveIds,
        IReadOnlyDictionary<string, SourceModel> sourceById)
    {
        string ownerType = owner.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        if (reactiveIds.Count == 0 || reactiveIds.Any(id => !sourceById.ContainsKey(id)))
        {
            return $"({ownerType} owner, ...)";
        }

        string[] sourceTypes = reactiveIds
                .Select(id => sourceById[id].Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))
                .ToArray();
        string current = string.Join(", ", sourceTypes);
        if (reactiveIds.Any(id => sourceById[id].IsVersioned))
        {
            return $"({ownerType} owner) | ({ownerType} owner, {current})";
        }

        string oldCurrent = string.Join(", ", sourceTypes.SelectMany(static type => new[] { type, type }));
        return $"({ownerType} owner) | ({ownerType} owner, {current}) | ({ownerType} owner, {oldCurrent})";
    }

    private static List<string> GetReactiveIds(AttributeData attribute)
    {
        List<string> result = new();
        if (attribute.ConstructorArguments.Length == 0)
        {
            return result;
        }

        TypedConstant argument = attribute.ConstructorArguments[0];
        if (argument.Kind != TypedConstantKind.Array)
        {
            if (argument.Value is string singleValue)
            {
                result.Add(singleValue);
            }

            return result;
        }

        foreach (TypedConstant item in argument.Values)
        {
            result.Add(item.Value as string ?? string.Empty);
        }

        return result;
    }

    private static bool ValidateGeneratedMethodCollision(
        GeneratorExecutionContext context,
        ClassDeclarationSyntax declaration,
        INamedTypeSymbol system,
        INamedTypeSymbol owner,
        string methodName)
    {
        foreach (ISymbol member in system.GetMembers(methodName))
        {
            bool collision = member is not IMethodSymbol method ||
                             method.Arity == 0 &&
                             method.Parameters.Length == 1 &&
                             method.Parameters[0].RefKind == RefKind.None &&
                             SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, owner);
            if (!collision)
            {
                continue;
            }

            Report(
                context,
                ETReactiveDiagnosticRules.GeneratedMethodCollision,
                GetLocation(member, declaration),
                system.ToDisplayString(),
                methodName,
                owner.ToDisplayString());
            return false;
        }

        return true;
    }

    private static HashSet<INamedTypeSymbol> GetReferencedReactiveOwners(Compilation compilation)
    {
        HashSet<INamedTypeSymbol> owners = new(SymbolEqualityComparer.Default);
        foreach (IAssemblySymbol assembly in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            if (!AnalyzeAssembly.AllHotfix.Contains(assembly.Identity.Name, StringComparer.Ordinal))
            {
                continue;
            }

            foreach (INamedTypeSymbol type in EnumerateTypes(assembly.GlobalNamespace))
            {
                if (!TryGetSystemAttribute(type, out AttributeData? attribute) ||
                    attribute == null ||
                    !TryGetOwner(type, attribute, out INamedTypeSymbol? owner) ||
                    owner == null)
                {
                    continue;
                }

                owners.Add(owner);
            }
        }

        return owners;
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateTypes(INamespaceSymbol namespaceSymbol)
    {
        foreach (INamedTypeSymbol type in namespaceSymbol.GetTypeMembers())
        {
            foreach (INamedTypeSymbol nestedType in EnumerateTypes(type))
            {
                yield return nestedType;
            }
        }

        foreach (INamespaceSymbol nestedNamespace in namespaceSymbol.GetNamespaceMembers())
        {
            foreach (INamedTypeSymbol type in EnumerateTypes(nestedNamespace))
            {
                yield return type;
            }
        }
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateTypes(INamedTypeSymbol type)
    {
        yield return type;
        foreach (INamedTypeSymbol nestedType in type.GetTypeMembers())
        {
            foreach (INamedTypeSymbol descendant in EnumerateTypes(nestedType))
            {
                yield return descendant;
            }
        }
    }

    private static bool ImplementsInterface(ITypeSymbol type, INamedTypeSymbol? interfaceType)
    {
        if (interfaceType == null || type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        return SymbolEqualityComparer.Default.Equals(namedType, interfaceType) ||
               namedType.AllInterfaces.Any(item => SymbolEqualityComparer.Default.Equals(item, interfaceType));
    }

    private static bool DirectlyImplementsInterface(INamedTypeSymbol type, INamedTypeSymbol? interfaceType)
    {
        if (interfaceType == null)
        {
            return false;
        }

        return type.Interfaces.Any(item =>
            SymbolEqualityComparer.Default.Equals(item, interfaceType) ||
            item.AllInterfaces.Any(inherited => SymbolEqualityComparer.Default.Equals(inherited, interfaceType)));
    }

    private static bool IsPartialTypeHierarchy(INamedTypeSymbol type)
    {
        for (INamedTypeSymbol? current = type; current != null; current = current.ContainingType)
        {
            bool hasSourceDeclaration = false;
            foreach (SyntaxReference syntaxReference in current.DeclaringSyntaxReferences)
            {
                if (syntaxReference.GetSyntax() is not TypeDeclarationSyntax declaration)
                {
                    return false;
                }

                hasSourceDeclaration = true;
                if (!declaration.Modifiers.Any(static modifier => modifier.IsKind(SyntaxKind.PartialKeyword)))
                {
                    return false;
                }
            }

            if (!hasSourceDeclaration)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsOrInheritsFrom(INamedTypeSymbol type, INamedTypeSymbol? baseType)
    {
        if (baseType == null)
        {
            return false;
        }

        INamedTypeSymbol? current = type;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
            {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }

    private static bool IsSupportedSourceType(ITypeSymbol type, bool isVersioned)
    {
        return type.IsValueType ||
               type.SpecialType == SpecialType.System_String ||
               isVersioned;
    }

    private static bool IsCustomStructWithoutEqualityOperator(ITypeSymbol type, bool isVersioned)
    {
        if (isVersioned ||
            !type.IsValueType ||
            type.SpecialType != SpecialType.None ||
            type.TypeKind == TypeKind.Enum ||
            type is not INamedTypeSymbol namedType ||
            namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            return false;
        }

        return !namedType.GetMembers("op_Equality")
                .OfType<IMethodSymbol>()
                .Any(static method => method.MethodKind == MethodKind.UserDefinedOperator);
    }

    private static string GetInequalityExpression(ITypeSymbol type, string oldValue, string currentValue)
    {
        if (type.SpecialType == SpecialType.System_Single)
        {
            return $"(global::System.Single.IsNaN({oldValue}) ? !global::System.Single.IsNaN({currentValue}) : " +
                   $"global::System.Single.IsNaN({currentValue}) || ({oldValue} != {currentValue} && " +
                   $"(global::System.Single.IsInfinity({oldValue}) || global::System.Single.IsInfinity({currentValue}) || " +
                   $"global::System.Math.Abs({oldValue} - {currentValue}) > 1e-6f)))";
        }

        if (type.SpecialType == SpecialType.System_Double)
        {
            return $"(global::System.Double.IsNaN({oldValue}) ? !global::System.Double.IsNaN({currentValue}) : " +
                   $"global::System.Double.IsNaN({currentValue}) || ({oldValue} != {currentValue} && " +
                   $"(global::System.Double.IsInfinity({oldValue}) || global::System.Double.IsInfinity({currentValue}) || " +
                   $"global::System.Math.Abs({oldValue} - {currentValue}) > 1e-9d)))";
        }

        return $"{oldValue} != {currentValue}";
    }

    private static string Emit(
        INamedTypeSymbol system,
        INamedTypeSymbol owner,
        string observerTypeName,
        IReadOnlyList<SourceModel> sources,
        IReadOnlyList<BindModel> binds)
    {
        StringBuilder code = new();
        code.AppendLine("// <auto-generated/>");

        string indentation = string.Empty;
        if (!system.ContainingNamespace.IsGlobalNamespace)
        {
            code.Append("namespace ")
                    .Append(system.ContainingNamespace.ToDisplayString())
                    .AppendLine();
            code.AppendLine("{");
            indentation = "    ";
        }

        string ownerType = owner.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        string systemType = system.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        code.Append(indentation)
                .Append("static partial class ")
                .Append(EscapeIdentifier(system.Name))
                .AppendLine();
        code.Append(indentation).AppendLine("{");
        string bodyIndentation = indentation + "    ";
        string statementIndentation = bodyIndentation + "    ";
        string observerBodyIndentation = statementIndentation + "    ";

        code.Append(bodyIndentation).AppendLine("[global::ET.EnableClass]");
        code.Append(bodyIndentation)
                .Append("[global::ET.ETReactiveObserver(typeof(")
                .Append(ownerType)
                .AppendLine("))]");
        code.Append(bodyIndentation)
                .Append("private sealed class ")
                .Append(observerTypeName)
                .Append(" : global::ET.IETReactiveObserver")
                .AppendLine();
        code.Append(bodyIndentation).AppendLine("{");
        code.Append(statementIndentation).AppendLine("private int dllVersion;");
        code.Append(statementIndentation).AppendLine("private long ownerInstanceId;");
        if (sources.Count > 0)
        {
            code.Append(statementIndentation)
                    .Append("private ")
                    .Append(ownerType)
                    .AppendLine(" owner = default!;");
            code.Append(statementIndentation).AppendLine("private bool __reactive_initialized;");
        }

        for (int index = 0; index < sources.Count; ++index)
        {
            SourceModel source = sources[index];
            string token = source.Id;
            string typeName = source.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            code.Append(statementIndentation)
                    .Append("private ")
                    .Append(typeName)
                    .Append(" __reactive_")
                    .Append(token)
                    .AppendLine(" = default!;");
            if (source.IsVersioned)
            {
                code.Append(statementIndentation)
                        .Append("private int __reactive_")
                        .Append(token)
                        .AppendLine("_version = -1;");
            }
        }

        code.AppendLine();
        code.Append(statementIndentation).AppendLine("public int DllVersion => this.dllVersion;");
        code.AppendLine();
        code.Append(statementIndentation)
                .AppendLine("public long OwnerInstanceId => this.ownerInstanceId;");
        code.AppendLine();
        code.Append(statementIndentation)
                .AppendLine("public void Initialize(global::ET.IETReactiveHost reactiveHost, int dllVersion)");
        code.Append(statementIndentation).AppendLine("{");
        code.Append(observerBodyIndentation)
                .Append(ownerType)
                .Append(" owner = (")
                .Append(ownerType)
                .AppendLine(")reactiveHost;");
        code.Append(observerBodyIndentation).AppendLine("this.dllVersion = dllVersion;");
        code.Append(observerBodyIndentation).AppendLine("this.ownerInstanceId = owner.InstanceId;");
        if (sources.Count > 0)
        {
            code.Append(observerBodyIndentation).AppendLine("this.owner = owner;");
        }
        code.Append(statementIndentation).AppendLine("}");
        code.AppendLine();
        code.Append(statementIndentation).AppendLine("public void Recycle()");
        code.Append(statementIndentation).AppendLine("{");
        code.Append(observerBodyIndentation).AppendLine("if (this.ownerInstanceId == 0)");
        code.Append(observerBodyIndentation).AppendLine("{");
        code.Append(observerBodyIndentation).AppendLine("    return;");
        code.Append(observerBodyIndentation).AppendLine("}");
        code.AppendLine();
        code.Append(observerBodyIndentation).AppendLine("this.dllVersion = 0;");
        code.Append(observerBodyIndentation).AppendLine("this.ownerInstanceId = 0;");
        if (sources.Count > 0)
        {
            code.Append(observerBodyIndentation).AppendLine("this.owner = default!;");
            code.Append(observerBodyIndentation).AppendLine("this.__reactive_initialized = false;");
        }
        foreach (SourceModel source in sources)
        {
            string token = source.Id;
            code.Append(observerBodyIndentation)
                    .Append("this.__reactive_")
                    .Append(token)
                    .AppendLine(" = default!;");
            if (source.IsVersioned)
            {
                code.Append(observerBodyIndentation)
                        .Append("this.__reactive_")
                        .Append(token)
                        .AppendLine("_version = -1;");
            }
        }
        code.Append(statementIndentation).AppendLine("}");
        code.AppendLine();
        code.Append(statementIndentation).AppendLine("public void ObserveChanges()");
        code.Append(statementIndentation).AppendLine("{");
        if (sources.Count > 0)
        {
            code.Append(observerBodyIndentation).AppendLine("if (!this.__reactive_initialized)");
            code.Append(observerBodyIndentation).AppendLine("{");
            code.Append(observerBodyIndentation).AppendLine("    this.__reactive_initialized = true;");

            foreach (SourceModel source in sources)
            {
                string token = source.Id;
                code.Append(observerBodyIndentation)
                        .Append("    this.__reactive_")
                        .Append(token)
                        .Append(" = ")
                        .Append(systemType)
                        .Append('.')
                        .Append(EscapeIdentifier(source.Method.Name))
                        .AppendLine("(this.owner);");
                if (source.IsVersioned)
                {
                    code.Append(observerBodyIndentation)
                            .Append("    this.__reactive_")
                            .Append(token)
                            .Append("_version = global::System.Object.ReferenceEquals(this.__reactive_")
                            .Append(token)
                            .Append(", null) ? -1 : ((global::ReactiveBinding.IVersion)(object)this.__reactive_")
                            .Append(token)
                            .AppendLine(").__Version;");
                }
            }

            foreach (BindModel bind in binds)
            {
                AppendBindCall(
                    code,
                    observerBodyIndentation + "    ",
                    systemType,
                    bind,
                    static id => $"this.__reactive_{id}",
                    static id => $"this.__reactive_{id}");
            }

            code.Append(observerBodyIndentation).AppendLine("    return;");
            code.Append(observerBodyIndentation).AppendLine("}");
            code.AppendLine();

            List<BindModel> multiBinds = binds.Where(static bind => bind.ReactiveIds.Count > 1).ToList();
            Dictionary<string, List<BindModel>> singleBindsBySource = new(StringComparer.Ordinal);
            foreach (BindModel bind in binds.Where(static bind => bind.ReactiveIds.Count == 1))
            {
                string reactiveId = bind.ReactiveIds[0];
                if (!singleBindsBySource.TryGetValue(reactiveId, out List<BindModel>? sourceBinds))
                {
                    sourceBinds = new List<BindModel>();
                    singleBindsBySource.Add(reactiveId, sourceBinds);
                }

                sourceBinds.Add(bind);
            }

            HashSet<string> sourcesNeedingFlags = new(
                multiBinds.SelectMany(static bind => bind.ReactiveIds),
                StringComparer.Ordinal);
            HashSet<string> sourcesNeedingOldValues = new(
                binds.Where(static bind => bind.Method.Parameters.Length - 1 == bind.ReactiveIds.Count * 2)
                        .SelectMany(static bind => bind.ReactiveIds),
                StringComparer.Ordinal);

            foreach (SourceModel source in sources)
            {
                string token = source.Id;
                if (sourcesNeedingFlags.Contains(source.Id))
                {
                    code.Append(observerBodyIndentation)
                            .Append("bool __changed_")
                            .Append(token)
                            .AppendLine(" = false;");
                }

                if (sourcesNeedingOldValues.Contains(source.Id))
                {
                    code.Append(observerBodyIndentation)
                            .Append(source.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                            .Append(" __old_")
                            .Append(token)
                            .Append(" = this.__reactive_")
                            .Append(token)
                            .AppendLine(";");
                }
            }

            if (sourcesNeedingFlags.Count > 0 || sourcesNeedingOldValues.Count > 0)
            {
                code.AppendLine();
            }

            foreach (SourceModel source in sources)
            {
                string token = source.Id;
                string typeName = source.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                code.Append(observerBodyIndentation)
                        .Append(typeName)
                        .Append(" __current_")
                        .Append(token)
                        .Append(" = ")
                        .Append(systemType)
                        .Append('.')
                        .Append(EscapeIdentifier(source.Method.Name))
                        .AppendLine("(this.owner);");

                if (source.IsVersioned)
                {
                    code.Append(observerBodyIndentation)
                            .Append("int __current_")
                            .Append(token)
                            .Append("_version = global::System.Object.ReferenceEquals(__current_")
                            .Append(token)
                            .Append(", null) ? -1 : ((global::ReactiveBinding.IVersion)(object)__current_")
                            .Append(token)
                            .AppendLine(").__Version;");
                    code.Append(observerBodyIndentation)
                            .Append("if (!global::System.Object.ReferenceEquals(__current_")
                            .Append(token)
                            .Append(", this.__reactive_")
                            .Append(token)
                            .Append(") || __current_")
                            .Append(token)
                            .Append("_version != this.__reactive_")
                            .Append(token)
                            .AppendLine("_version)");
                }
                else
                {
                    code.Append(observerBodyIndentation)
                            .Append("if (")
                            .Append(GetInequalityExpression(
                                source.Type,
                                $"this.__reactive_{token}",
                                $"__current_{token}"))
                            .AppendLine(")");
                }

                code.Append(observerBodyIndentation).AppendLine("{");
                if (sourcesNeedingFlags.Contains(source.Id))
                {
                    code.Append(observerBodyIndentation)
                            .Append("    __changed_")
                            .Append(token)
                            .AppendLine(" = true;");
                }

                code.Append(observerBodyIndentation)
                        .Append("    this.__reactive_")
                        .Append(token)
                        .Append(" = __current_")
                        .Append(token)
                        .AppendLine(";");
                if (source.IsVersioned)
                {
                    code.Append(observerBodyIndentation)
                            .Append("    this.__reactive_")
                            .Append(token)
                            .Append("_version = __current_")
                            .Append(token)
                            .AppendLine("_version;");
                }

                if (singleBindsBySource.TryGetValue(source.Id, out List<BindModel>? singleBinds))
                {
                    foreach (BindModel bind in singleBinds)
                    {
                        AppendBindCall(
                            code,
                            observerBodyIndentation + "    ",
                            systemType,
                            bind,
                            static id => $"__old_{id}",
                            static id => $"this.__reactive_{id}");
                    }
                }

                code.Append(observerBodyIndentation).AppendLine("}");
                code.AppendLine();
            }

            foreach (BindModel bind in multiBinds)
            {
                code.Append(observerBodyIndentation).Append("if (");
                for (int index = 0; index < bind.ReactiveIds.Count; ++index)
                {
                    if (index > 0)
                    {
                        code.Append(" || ");
                    }

                    code.Append("__changed_").Append(bind.ReactiveIds[index]);
                }

                code.AppendLine(")");
                code.Append(observerBodyIndentation).AppendLine("{");
                AppendBindCall(
                    code,
                    observerBodyIndentation + "    ",
                    systemType,
                    bind,
                    static id => $"__old_{id}",
                    static id => $"this.__reactive_{id}");
                code.Append(observerBodyIndentation).AppendLine("}");
                code.AppendLine();
            }
        }

        code.Append(statementIndentation).AppendLine("}");
        code.AppendLine();
        code.Append(statementIndentation).AppendLine("public void ResetChanges()");
        code.Append(statementIndentation).AppendLine("{");
        if (sources.Count > 0)
        {
            code.Append(observerBodyIndentation).AppendLine("this.__reactive_initialized = false;");
        }
        code.Append(statementIndentation).AppendLine("}");
        code.Append(bodyIndentation).AppendLine("}");
        code.AppendLine();

        code.Append(bodyIndentation)
                .Append("public static void ")
                .Append(ObserveMethodName)
                .Append("(this ")
                .Append(ownerType)
                .AppendLine(" self)");
        code.Append(bodyIndentation).AppendLine("{");
        code.Append(statementIndentation).AppendLine("if (self.InstanceId == 0)");
        code.Append(statementIndentation).AppendLine("{");
        code.Append(statementIndentation).AppendLine("    return;");
        code.Append(statementIndentation).AppendLine("}");
        code.AppendLine();
        code.Append(statementIndentation)
                .AppendLine("global::ET.IETReactiveHost reactiveHost = self;");
        code.Append(statementIndentation)
                .AppendLine("global::ET.ETReactiveSystem reactiveSystem = global::ET.ETReactiveSystem.Instance;");
        code.Append(statementIndentation).AppendLine("int dllVersion = reactiveSystem.DllVersion;");
        code.Append(statementIndentation)
                .AppendLine("if (reactiveHost.ReactiveObserver is not global::ET.IETReactiveObserver observer ||")
                .Append(statementIndentation)
                .AppendLine("    observer.DllVersion != dllVersion || observer.OwnerInstanceId != self.InstanceId)");
        code.Append(statementIndentation).AppendLine("{");
        code.Append(statementIndentation).AppendLine("    if (reactiveHost.ReactiveObserver is global::ET.IETReactiveObserver oldObserver)");
        code.Append(statementIndentation).AppendLine("    {");
        code.Append(statementIndentation).AppendLine("        reactiveSystem.Recycle(oldObserver);");
        code.Append(statementIndentation).AppendLine("    }");
        code.AppendLine();
        code.Append(statementIndentation)
                .Append("    observer = reactiveSystem.Rent(typeof(")
                .Append(ownerType)
                .AppendLine("), reactiveHost);");
        code.Append(statementIndentation).AppendLine("    reactiveHost.ReactiveObserver = observer;");
        code.Append(statementIndentation).AppendLine("}");
        code.AppendLine();
        code.Append(statementIndentation).AppendLine("observer.ObserveChanges();");
        code.Append(bodyIndentation).AppendLine("}");
        code.AppendLine();
        code.Append(bodyIndentation)
                .Append("public static void ")
                .Append(ResetMethodName)
                .Append("(this ")
                .Append(ownerType)
                .AppendLine(" self)");
        code.Append(bodyIndentation).AppendLine("{");
        code.Append(statementIndentation)
                .AppendLine("if (((global::ET.IETReactiveHost)self).ReactiveObserver is global::ET.IETReactiveObserver observer &&")
                .Append(statementIndentation)
                .AppendLine("    observer.DllVersion == global::ET.ETReactiveSystem.Instance.DllVersion &&")
                .Append(statementIndentation)
                .AppendLine("    observer.OwnerInstanceId == self.InstanceId)");
        code.Append(statementIndentation).AppendLine("{");
        code.Append(statementIndentation).AppendLine("    observer.ResetChanges();");
        code.Append(statementIndentation).AppendLine("}");
        code.Append(bodyIndentation).AppendLine("}");
        code.AppendLine();
        code.Append(bodyIndentation)
                .Append("public static void ")
                .Append(ClearMethodName)
                .Append("(this ")
                .Append(ownerType)
                .AppendLine(" self)");
        code.Append(bodyIndentation).AppendLine("{");
        code.Append(statementIndentation)
                .AppendLine("global::ET.IETReactiveHost reactiveHost = self;");
        code.Append(statementIndentation).AppendLine("global::ReactiveBinding.IReactiveObserver currentObserver = reactiveHost.ReactiveObserver;");
        code.Append(statementIndentation).AppendLine("reactiveHost.ReactiveObserver = null!;");
        code.Append(statementIndentation).AppendLine("if (currentObserver is global::ET.IETReactiveObserver pooledObserver)");
        code.Append(statementIndentation).AppendLine("{");
        code.Append(statementIndentation).AppendLine("    global::ET.ETReactiveSystem.Instance.Recycle(pooledObserver);");
        code.Append(statementIndentation).AppendLine("}");
        code.Append(bodyIndentation).AppendLine("}");
        code.Append(indentation).AppendLine("}");

        if (!system.ContainingNamespace.IsGlobalNamespace)
        {
            code.AppendLine("}");
        }

        return code.ToString();
    }

    private static string EmitReactiveHost(INamedTypeSymbol host)
    {
        StringBuilder code = new();
        code.AppendLine("// <auto-generated/>");

        string indentation = string.Empty;
        if (!host.ContainingNamespace.IsGlobalNamespace)
        {
            code.Append("namespace ")
                    .Append(host.ContainingNamespace.ToDisplayString())
                    .AppendLine();
            code.AppendLine("{");
            indentation = "    ";
        }

        Stack<INamedTypeSymbol> typeHierarchy = new();
        for (INamedTypeSymbol? current = host; current != null; current = current.ContainingType)
        {
            typeHierarchy.Push(current);
        }

        int declarationCount = typeHierarchy.Count;
        foreach (INamedTypeSymbol type in typeHierarchy)
        {
            code.Append(indentation)
                    .Append("partial ")
                    .Append(GetTypeDeclarationKeyword(type))
                    .Append(' ')
                    .Append(EscapeIdentifier(type.Name));
            if (type.TypeParameters.Length > 0)
            {
                code.Append('<')
                        .Append(string.Join(", ", type.TypeParameters.Select(parameter => EscapeIdentifier(parameter.Name))))
                        .Append('>');
            }

            code.AppendLine();
            code.Append(indentation).AppendLine("{");
            indentation += "    ";
        }

        code.Append(indentation).AppendLine("[global::MemoryPack.MemoryPackIgnore]");
        code.Append(indentation).AppendLine("[global::MongoDB.Bson.Serialization.Attributes.BsonIgnore]");
        code.Append(indentation)
                .AppendLine("public global::ReactiveBinding.IReactiveObserver ReactiveObserver { get; set; }");

        for (int index = 0; index < declarationCount; ++index)
        {
            indentation = indentation.Substring(0, indentation.Length - 4);
            code.Append(indentation).AppendLine("}");
        }

        if (!host.ContainingNamespace.IsGlobalNamespace)
        {
            code.AppendLine("}");
        }

        return code.ToString();
    }

    private static string GetTypeDeclarationKeyword(INamedTypeSymbol type)
    {
        SyntaxNode? declaration = type.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
        if (declaration is StructDeclarationSyntax)
        {
            return "struct";
        }

        if (declaration is RecordDeclarationSyntax)
        {
            return "record";
        }

        return "class";
    }

    private static void AppendBindCall(
        StringBuilder code,
        string indentation,
        string systemType,
        BindModel bind,
        Func<string, string> oldValueExpression,
        Func<string, string> currentValueExpression)
    {
        code.Append(indentation)
                .Append(systemType)
                .Append('.')
                .Append(EscapeIdentifier(bind.Method.Name))
                .Append("(this.owner");

        int valueParameterCount = bind.Method.Parameters.Length - 1;
        if (valueParameterCount == bind.ReactiveIds.Count)
        {
            foreach (string reactiveId in bind.ReactiveIds)
            {
                code.Append(", ").Append(currentValueExpression(reactiveId));
            }
        }
        else if (valueParameterCount == bind.ReactiveIds.Count * 2)
        {
            foreach (string reactiveId in bind.ReactiveIds)
            {
                code.Append(", ").Append(oldValueExpression(reactiveId));
                code.Append(", ").Append(currentValueExpression(reactiveId));
            }
        }

        code.AppendLine(");");
    }

    private static string GetGeneratedObserverTypeName(INamedTypeSymbol system)
    {
        const string baseName = "__ETReactiveObserver";
        string candidate = baseName;
        int suffix = 0;
        while (system.GetTypeMembers(candidate).Length > 0)
        {
            candidate = $"{baseName}_{++suffix}";
        }

        return candidate;
    }

    private static string GetMetadataName(INamedTypeSymbol type)
    {
        Stack<string> names = new();
        INamedTypeSymbol? currentType = type;
        while (currentType != null)
        {
            names.Push(currentType.MetadataName);
            currentType = currentType.ContainingType;
        }

        string typeName = string.Join("+", names);
        return type.ContainingNamespace.IsGlobalNamespace
                ? typeName
                : $"{type.ContainingNamespace.ToDisplayString()}.{typeName}";
    }

    private static string EscapeIdentifier(string identifier)
    {
        return SyntaxFacts.GetKeywordKind(identifier) != SyntaxKind.None ||
               SyntaxFacts.GetContextualKeywordKind(identifier) != SyntaxKind.None
                ? "@" + identifier
                : identifier;
    }

    private static AttributeData? GetAttribute(ISymbol symbol, string metadataName)
    {
        return symbol.GetAttributes().FirstOrDefault(attribute =>
            string.Equals(attribute.AttributeClass?.ToDisplayString(), metadataName, StringComparison.Ordinal));
    }

    private static bool TryGetSystemAttribute(INamedTypeSymbol system, out AttributeData? attribute)
    {
        attribute = GetAttribute(system, SystemAttributeName);
        return attribute != null;
    }

    private static Location GetLocation(ISymbol symbol, ClassDeclarationSyntax fallback)
    {
        return symbol.Locations.FirstOrDefault(static location => location.IsInSource) ?? fallback.Identifier.GetLocation();
    }

    private static void Report(
        GeneratorExecutionContext context,
        DiagnosticDescriptor descriptor,
        Location location,
        params object[] messageArguments)
    {
        context.ReportDiagnostic(Diagnostic.Create(descriptor, location, messageArguments));
    }

    private sealed class SyntaxReceiver: ISyntaxContextReceiver
    {
        public List<ClassDeclarationSyntax> SystemDeclarations { get; } = new();

        public List<ClassDeclarationSyntax> HostDeclarations { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is not ClassDeclarationSyntax declaration ||
                context.SemanticModel.GetDeclaredSymbol(declaration) is not INamedTypeSymbol type)
            {
                return;
            }

            if (declaration.AttributeLists.Count > 0 && GetAttribute(type, SystemAttributeName) != null)
            {
                this.SystemDeclarations.Add(declaration);
            }

            if (declaration.BaseList == null)
            {
                return;
            }

            INamedTypeSymbol? reactiveHostInterface =
                    context.SemanticModel.Compilation.GetTypeByMetadataName(ReactiveHostInterfaceName);
            if (DirectlyImplementsInterface(type, reactiveHostInterface))
            {
                this.HostDeclarations.Add(declaration);
            }
        }
    }

    private sealed class SourceModel
    {
        public SourceModel(IMethodSymbol method, bool isVersioned)
        {
            this.Method = method;
            this.IsVersioned = isVersioned;
        }

        public IMethodSymbol Method { get; }

        public string Id => this.Method.Name;

        public ITypeSymbol Type => this.Method.ReturnType;

        public bool IsVersioned { get; }
    }

    private sealed class SystemCandidate
    {
        public SystemCandidate(
            ClassDeclarationSyntax declaration,
            INamedTypeSymbol system,
            INamedTypeSymbol? owner)
        {
            this.Declaration = declaration;
            this.System = system;
            this.Owner = owner;
        }

        public ClassDeclarationSyntax Declaration { get; }

        public INamedTypeSymbol System { get; }

        public INamedTypeSymbol? Owner { get; }
    }

    private sealed class BindModel
    {
        public BindModel(IMethodSymbol method, IReadOnlyList<string> reactiveIds)
        {
            this.Method = method;
            this.ReactiveIds = reactiveIds;
        }

        public IMethodSymbol Method { get; }

        public IReadOnlyList<string> ReactiveIds { get; }
    }
}
