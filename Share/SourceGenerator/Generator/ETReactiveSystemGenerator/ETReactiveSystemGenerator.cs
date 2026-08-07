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
    private const string ReactiveHostInterfaceName = "ET.IETReactive";
    private const string VersionInterfaceName = "ReactiveBinding.IVersion";
    private const string ObserveMethodName = "ObserveChanges";
    private const string ResetMethodName = "ResetReactive";

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

        List<ClassDeclarationSyntax> hostDeclarations = OrderSyntaxNodes(receiver.HostDeclarations).ToList();
        List<ClassDeclarationSyntax> systemDeclarations = OrderSyntaxNodes(receiver.SystemDeclarations).ToList();

        GenerateReactiveHosts(context, hostDeclarations);
        if (systemDeclarations.Count == 0)
        {
            return;
        }

        HashSet<INamedTypeSymbol> processedSystems = new(SymbolEqualityComparer.Default);
        List<SystemCandidate> candidates = new();
        foreach (ClassDeclarationSyntax declaration in systemDeclarations)
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
        INamedTypeSymbol? versionInterface = context.Compilation.GetTypeByMetadataName(VersionInterfaceName);
        if (reactiveHostInterface == null || entityType == null)
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

            List<SourceModel> sources = CollectSources(context, declaration, host, versionInterface, out bool valid);

            if (!valid)
            {
                continue;
            }

            context.AddSource(
                $"ETReactiveHostGenerator.{GetMetadataName(host)}.g.cs",
                EmitReactiveHost(host, sources));
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

        List<IMethodSymbol> bindMethods = GetAttributedMethods(system, BindAttributeName);

        INamedTypeSymbol? versionInterface = context.Compilation.GetTypeByMetadataName(VersionInterfaceName);
        List<SourceModel> sources = CollectSources(context, declaration, owner, versionInterface, out bool sourcesValid);
        valid &= sourcesValid;

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

            if (reactiveIds.Count > 0 && !UsesOnlyNameofArguments(bindAttribute))
            {
                Report(
                    context,
                    ETReactiveDiagnosticRules.BindNameof,
                    GetAttributeLocation(bindAttribute, method, declaration),
                    method.Name);
                bindValid = false;
            }

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

        List<string> usedSourceIds = binds
                .SelectMany(static bind => bind.ReactiveIds)
                .Distinct(StringComparer.Ordinal)
                .ToList();
        HashSet<string> usedSourceIdSet = new(usedSourceIds, StringComparer.Ordinal);
        foreach (SourceModel source in sources.Where(source => !usedSourceIdSet.Contains(source.Id)))
        {
            Report(
                context,
                ETReactiveDiagnosticRules.UnusedSource,
                GetLocation(source.Member, declaration),
                source.Id);
        }

        sources = usedSourceIds.Select(id => sourceById[id]).ToList();
        valid &= ValidateGeneratedMethodCollision(context, declaration, system, owner, ObserveMethodName);
        valid &= ValidateGeneratedMethodCollision(context, declaration, system, owner, ResetMethodName);
        valid &= ValidateThrottle(context, declaration, system);

        if (!valid)
        {
            return;
        }

        string generatedCode = Emit(system, owner, sources, binds, GetThrottleCallCount(system));

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
        if (!string.Equals(attribute.AttributeClass?.ToDisplayString(), SystemAttributeName, StringComparison.Ordinal))
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

    /// <summary>
    /// 节流值从 System 的 ETReactiveSystemAttribute.ThrottleCount 读取。
    /// </summary>
    private static int GetThrottleCallCount(INamedTypeSymbol system)
    {
        if (!TryGetSystemAttribute(system, out AttributeData? attribute) || attribute == null)
        {
            return 1;
        }

        // 先检查命名参数
        foreach (var namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == "ThrottleCount" && namedArg.Value.Value is int callCount)
            {
                return callCount;
            }
        }

        // 再检查构造函数参数
        if (attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is int ctorCallCount)
        {
            return ctorCallCount;
        }

        return 1;
    }

    private static bool ValidateThrottle(GeneratorExecutionContext context, ClassDeclarationSyntax systemDeclaration, INamedTypeSymbol system)
    {
        if (!TryGetSystemAttribute(system, out AttributeData? attribute) || attribute == null)
        {
            return true;
        }

        int? callCount = null;

        // 检查命名参数
        foreach (var namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == "ThrottleCount" && namedArg.Value.Value is int value)
            {
                callCount = value;
                break;
            }
        }

        // 检查构造函数参数
        if (callCount == null && attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is int ctorValue)
        {
            callCount = ctorValue;
        }

        if (callCount.HasValue && callCount.Value < 1)
        {
            Report(
                context,
                ETReactiveDiagnosticRules.ThrottleValue,
                GetAttributeLocation(attribute, system, systemDeclaration),
                system.ToDisplayString(),
                callCount.Value.ToString());
            return false;
        }

        return true;
    }

    private static List<IMethodSymbol> GetAttributedMethods(INamedTypeSymbol system, string attributeName)
    {
        return OrderSymbols(system.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(method => !method.IsImplicitlyDeclared && GetAttribute(method, attributeName) != null))
                .ToList();
    }

    private static List<SourceModel> CollectSources(GeneratorExecutionContext context, ClassDeclarationSyntax declaration, INamedTypeSymbol owner, INamedTypeSymbol? versionInterface, out bool valid)
    {
        valid = true;
        List<SourceModel> sources = new();
        foreach (ISymbol member in GetAttributedSourceMembers(owner, SourceAttributeName))
        {
            if (!TryGetSourceType(member, out ITypeSymbol sourceType))
            {
                Report(context, ETReactiveDiagnosticRules.SourceSignature, GetLocation(member, declaration), member.Name);
                valid = false;
                continue;
            }

            bool isVersioned = ImplementsInterface(sourceType, versionInterface);
            if (!IsSupportedSourceType(sourceType, isVersioned))
            {
                Report(context, ETReactiveDiagnosticRules.UnsupportedSourceType, GetLocation(member, declaration), member.Name, sourceType.ToDisplayString());
                valid = false;
                continue;
            }

            if (IsCustomStructWithoutEqualityOperator(sourceType, isVersioned))
            {
                Report(context, ETReactiveDiagnosticRules.StructEquality, GetLocation(member, declaration), member.Name, sourceType.ToDisplayString());
                valid = false;
                continue;
            }

            sources.Add(new SourceModel(member, sourceType, isVersioned));
        }

        foreach (IGrouping<string, SourceModel> duplicate in sources.GroupBy(static source => source.Id, StringComparer.Ordinal).Where(static group => group.Count() > 1))
        {
            foreach (SourceModel source in duplicate.Skip(1))
            {
                Report(context, ETReactiveDiagnosticRules.DuplicateSource, GetLocation(source.Member, declaration), source.Id, owner.ToDisplayString());
            }

            valid = false;
        }

        return sources;
    }

    private static List<ISymbol> GetAttributedSourceMembers(INamedTypeSymbol owner, string attributeName)
    {
        return OrderSymbols(owner.GetMembers()
                .Where(static member => member is IFieldSymbol or IPropertySymbol or IMethodSymbol)
                .Where(member => !member.IsImplicitlyDeclared && GetAttribute(member, attributeName) != null))
                .ToList();
    }

    private static IOrderedEnumerable<TNode> OrderSyntaxNodes<TNode>(IEnumerable<TNode> nodes)
        where TNode: SyntaxNode
    {
        return nodes
                .OrderBy(static node => GetSyntaxTreeKey(node.SyntaxTree), StringComparer.Ordinal)
                .ThenBy(static node => node.SpanStart)
                .ThenBy(static node => node.RawKind);
    }

    private static IOrderedEnumerable<TSymbol> OrderSymbols<TSymbol>(IEnumerable<TSymbol> symbols)
        where TSymbol: ISymbol
    {
        return symbols
                .OrderBy(static symbol => GetSyntaxTreeKey(GetSourceLocation(symbol)), StringComparer.Ordinal)
                .ThenBy(static symbol => GetSourceStart(GetSourceLocation(symbol)))
                .ThenBy(static symbol => (int)symbol.Kind)
                .ThenBy(static symbol => symbol.ToDisplayString(), StringComparer.Ordinal);
    }

    private static Location GetSourceLocation(ISymbol symbol)
    {
        foreach (Location location in symbol.Locations)
        {
            if (location.IsInSource)
            {
                return location;
            }
        }

        return Location.None;
    }

    private static int GetSourceStart(Location location)
    {
        return location.IsInSource ? location.SourceSpan.Start : int.MaxValue;
    }

    private static string GetSyntaxTreeKey(Location location)
    {
        return location.SourceTree is SyntaxTree syntaxTree ? GetSyntaxTreeKey(syntaxTree) : "N:";
    }

    private static string GetSyntaxTreeKey(SyntaxTree syntaxTree)
    {
        if (!string.IsNullOrEmpty(syntaxTree.FilePath))
        {
            return $"P:{syntaxTree.FilePath.Replace('\\', '/')}";
        }

        StringBuilder key = new("C:");
        foreach (byte value in syntaxTree.GetText().GetChecksum())
        {
            key.Append(value.ToString("x2"));
        }

        return key.ToString();
    }

    private static bool TryGetSourceType(ISymbol member, out ITypeSymbol sourceType)
    {
        ITypeSymbol? resolvedType = member switch
        {
            IFieldSymbol { IsStatic: false, DeclaredAccessibility: Accessibility.Public } field => field.Type,
            IPropertySymbol
            {
                IsStatic: false,
                DeclaredAccessibility: Accessibility.Public,
                GetMethod.DeclaredAccessibility: Accessibility.Public,
                Parameters.Length: 0,
                RefKind: RefKind.None,
            } property => property.Type,
            IMethodSymbol
            {
                MethodKind: MethodKind.Ordinary,
                IsStatic: false,
                DeclaredAccessibility: Accessibility.Public,
                Arity: 0,
                ReturnsVoid: false,
                ReturnsByRef: false,
                ReturnsByRefReadonly: false,
                Parameters.Length: 0,
            } method => method.ReturnType,
            _ => null,
        };

        sourceType = resolvedType!;
        return resolvedType != null &&
               resolvedType.TypeKind != TypeKind.Error &&
               resolvedType.TypeKind != TypeKind.Pointer &&
               resolvedType.TypeKind != TypeKind.FunctionPointer &&
               !resolvedType.IsRefLikeType;
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

    private static bool UsesOnlyNameofArguments(AttributeData attribute)
    {
        if (attribute.ApplicationSyntaxReference?.GetSyntax() is not AttributeSyntax attributeSyntax || attributeSyntax.ArgumentList == null)
        {
            return true;
        }

        return attributeSyntax.ArgumentList.Arguments.Count > 0 && attributeSyntax.ArgumentList.Arguments.All(static argument => IsNameofSourceExpression(argument.Expression));
    }

    private static bool IsNameofSourceExpression(ExpressionSyntax expression)
    {
        while (expression is ParenthesizedExpressionSyntax parenthesized)
        {
            expression = parenthesized.Expression;
        }

        if (expression is InvocationExpressionSyntax
            {
                Expression: IdentifierNameSyntax identifier,
                ArgumentList.Arguments.Count: 1,
            } && identifier.Identifier.Text == "nameof")
        {
            return true;
        }

        InitializerExpressionSyntax? initializer = expression switch
        {
            ArrayCreationExpressionSyntax arrayCreation => arrayCreation.Initializer,
            ImplicitArrayCreationExpressionSyntax implicitArrayCreation => implicitArrayCreation.Initializer,
            _ => null,
        };
        return initializer != null && initializer.Expressions.Count > 0 && initializer.Expressions.All(static item => IsNameofSourceExpression(item));
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
        IReadOnlyList<SourceModel> sources,
        IReadOnlyList<BindModel> binds,
        int throttleCallCount)
    {
        StringBuilder code = new();
        code.AppendLine("// <auto-generated/>");

        string indentation = string.Empty;
        if (!system.ContainingNamespace.IsGlobalNamespace)
        {
            code.Append("namespace ").Append(system.ContainingNamespace.ToDisplayString()).AppendLine();
            code.AppendLine("{");
            indentation = "    ";
        }

        string ownerType = owner.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        string systemType = system.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        string bodyIndentation = indentation + "    ";
        string statementIndentation = bodyIndentation + "    ";
        Dictionary<string, List<BindModel>> singleBindsBySource = new(StringComparer.Ordinal);
        List<BindModel> multiBinds = binds.Where(static bind => bind.ReactiveIds.Count > 1).ToList();
        foreach (BindModel bind in binds.Where(static bind => bind.ReactiveIds.Count == 1))
        {
            string sourceId = bind.ReactiveIds[0];
            if (!singleBindsBySource.TryGetValue(sourceId, out List<BindModel>? sourceBinds))
            {
                sourceBinds = new List<BindModel>();
                singleBindsBySource.Add(sourceId, sourceBinds);
            }

            sourceBinds.Add(bind);
        }

        code.Append(indentation).Append("static partial class ").Append(EscapeIdentifier(system.Name)).AppendLine();
        code.Append(indentation).AppendLine("{");
        code.Append(bodyIndentation).Append("public static void ").Append(ObserveMethodName).Append("(this ").Append(ownerType).AppendLine(" self)");
        code.Append(bodyIndentation).AppendLine("{");
        code.Append(statementIndentation).AppendLine("if (self.InstanceId == 0)");
        code.Append(statementIndentation).AppendLine("{");
        code.Append(statementIndentation).AppendLine("    return;");
        code.Append(statementIndentation).AppendLine("}");
        code.AppendLine();
        if (throttleCallCount > 1)
        {
            code.Append(statementIndentation).Append("if (self.__ETReactiveInitialized && ++self.__ETReactiveCallCount < ").Append(throttleCallCount).AppendLine(")");
            code.Append(statementIndentation).AppendLine("{");
            code.Append(statementIndentation).AppendLine("    return;");
            code.Append(statementIndentation).AppendLine("}");
            code.Append(statementIndentation).AppendLine("self.__ETReactiveCallCount = 0;");
            code.AppendLine();
        }

        code.Append(statementIndentation).AppendLine("if (!self.__ETReactiveInitialized)");
        code.Append(statementIndentation).AppendLine("{");
        foreach (SourceModel source in sources)
        {
            string token = source.Id;
            code.Append(statementIndentation).Append("    self.__ETReactive").Append(token).Append(" = ").Append(source.ReadExpression).AppendLine(";");
            if (source.IsVersioned)
            {
                code.Append(statementIndentation).Append("    self.__ETReactive").Append(token).Append("Version = global::System.Object.ReferenceEquals(self.__ETReactive").Append(token).Append(", null) ? -1 : ((global::ReactiveBinding.IVersion)(object)self.__ETReactive").Append(token).AppendLine(").__Version;");
            }
        }

        code.Append(statementIndentation).AppendLine("    self.__ETReactiveInitialized = true;");
        foreach (BindModel bind in binds)
        {
            AppendEntityBindCall(code, statementIndentation + "    ", systemType, bind, static id => $"self.__ETReactive{id}", static id => $"self.__ETReactive{id}");
        }

        code.Append(statementIndentation).AppendLine("    return;");
        code.Append(statementIndentation).AppendLine("}");
        code.AppendLine();
        HashSet<string> sourcesNeedingFlags = new(
            multiBinds.SelectMany(static bind => bind.ReactiveIds),
            StringComparer.Ordinal);
        HashSet<string> sourcesNeedingOldValues = new(
            binds.Where(static bind => IsTransitionBind(bind)).SelectMany(static bind => bind.ReactiveIds),
            StringComparer.Ordinal);
        foreach (SourceModel source in sources)
        {
            string token = source.Id;
            string typeName = source.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (sourcesNeedingFlags.Contains(source.Id))
            {
                code.Append(statementIndentation).Append("bool __changed_").Append(token).AppendLine(" = false;");
            }

            if (!source.IsVersioned && sourcesNeedingOldValues.Contains(source.Id))
            {
                code.Append(statementIndentation).Append(typeName).Append(" __old_").Append(token).Append(" = self.__ETReactive").Append(token).AppendLine(";");
            }
        }

        code.AppendLine();
        foreach (SourceModel source in sources)
        {
            string token = source.Id;
            string typeName = source.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            code.Append(statementIndentation).Append(typeName).Append(" __current_").Append(token).Append(" = ").Append(source.ReadExpression).AppendLine(";");
            if (source.IsVersioned)
            {
                code.Append(statementIndentation).Append("int __current_").Append(token).Append("Version = global::System.Object.ReferenceEquals(__current_").Append(token).Append(", null) ? -1 : ((global::ReactiveBinding.IVersion)(object)__current_").Append(token).AppendLine(").__Version;");
                code.Append(statementIndentation).Append("if (!global::System.Object.ReferenceEquals(__current_").Append(token).Append(", self.__ETReactive").Append(token).Append(") || __current_").Append(token).Append("Version != self.__ETReactive").Append(token).AppendLine("Version)");
            }
            else
            {
                code.Append(statementIndentation).Append("if (").Append(GetInequalityExpression(source.Type, $"__current_{token}", $"self.__ETReactive{token}")).AppendLine(")");
            }

            code.Append(statementIndentation).AppendLine("{");
            if (sourcesNeedingFlags.Contains(source.Id))
            {
                code.Append(statementIndentation).Append("    __changed_").Append(token).AppendLine(" = true;");
            }

            code.Append(statementIndentation).Append("    self.__ETReactive").Append(token).Append(" = __current_").Append(token).AppendLine(";");
            if (source.IsVersioned)
            {
                code.Append(statementIndentation).Append("    self.__ETReactive").Append(token).Append("Version = __current_").Append(token).AppendLine("Version;");
            }

            if (singleBindsBySource.TryGetValue(source.Id, out List<BindModel>? singleBinds))
            {
                foreach (BindModel bind in singleBinds)
                {
                    AppendEntityBindCall(code, statementIndentation + "    ", systemType, bind, static id => $"__old_{id}", static id => $"self.__ETReactive{id}");
                }
            }

            code.Append(statementIndentation).AppendLine("}");
            code.AppendLine();
        }

        foreach (BindModel bind in multiBinds)
        {
            string changes = string.Join(" || ", bind.ReactiveIds.Select(static id => $"__changed_{id}"));
            code.Append(statementIndentation).Append("if (").Append(changes).AppendLine(")");
            code.Append(statementIndentation).AppendLine("{");
            AppendEntityBindCall(code, statementIndentation + "    ", systemType, bind, static id => $"__old_{id}", static id => $"self.__ETReactive{id}");
            code.Append(statementIndentation).AppendLine("}");
        }

        code.Append(bodyIndentation).AppendLine("}");
        code.AppendLine();
        code.Append(bodyIndentation).Append("public static void ").Append(ResetMethodName).Append("(this ").Append(ownerType).AppendLine(" self)");
        code.Append(bodyIndentation).AppendLine("{");
        code.Append(statementIndentation).AppendLine("self.__ETReactiveInitialized = false;");
        code.Append(bodyIndentation).AppendLine("}");
        code.Append(indentation).AppendLine("}");
        if (!system.ContainingNamespace.IsGlobalNamespace)
        {
            code.AppendLine("}");
        }

        return code.ToString();
    }

    private static bool IsTransitionBind(BindModel bind)
    {
        return bind.Method.Parameters.Length - 1 == bind.ReactiveIds.Count * 2;
    }

    private static void AppendEntityBindCall(StringBuilder code, string indentation, string systemType, BindModel bind, Func<string, string> oldValueExpression, Func<string, string> currentValueExpression)
    {
        code.Append(indentation).Append(systemType).Append('.').Append(EscapeIdentifier(bind.Method.Name)).Append("(self");
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

    private static string EmitReactiveHost(INamedTypeSymbol host, IReadOnlyList<SourceModel> sources)
    {
        StringBuilder code = new();
        code.AppendLine("// <auto-generated/>");

        string indentation = string.Empty;
        if (!host.ContainingNamespace.IsGlobalNamespace)
        {
            code.Append("namespace ").Append(host.ContainingNamespace.ToDisplayString()).AppendLine();
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
            code.Append(indentation).Append("partial ").Append(GetTypeDeclarationKeyword(type)).Append(' ').Append(EscapeIdentifier(type.Name));
            if (type.TypeParameters.Length > 0)
            {
                code.Append('<').Append(string.Join(", ", type.TypeParameters.Select(parameter => EscapeIdentifier(parameter.Name)))).Append('>');
            }

            code.AppendLine();
            code.Append(indentation).AppendLine("{");
            indentation += "    ";
        }

        AppendReactiveField(code, indentation, "public bool __ETReactiveInitialized;");

        foreach (SourceModel source in sources)
        {
            string typeName = source.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            AppendReactiveField(code, indentation, $"public {typeName} __ETReactive{source.Id};");
            if (source.IsVersioned)
            {
                AppendReactiveField(code, indentation, $"public int __ETReactive{source.Id}Version = -1;");
            }
        }

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

    private static void AppendReactiveField(StringBuilder code, string indentation, string declaration)
    {
        code.Append(indentation).AppendLine("[global::System.NonSerialized]");
        code.Append(indentation).AppendLine("[global::MemoryPack.MemoryPackIgnore]");
        code.Append(indentation).AppendLine("[global::MongoDB.Bson.Serialization.Attributes.BsonIgnore]");
        code.Append(indentation).AppendLine(declaration);
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

    private static Location GetAttributeLocation(AttributeData attribute, ISymbol symbol, ClassDeclarationSyntax fallback)
    {
        return attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? GetLocation(symbol, fallback);
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
        public SourceModel(ISymbol member, ITypeSymbol type, bool isVersioned)
        {
            this.Member = member;
            this.Type = type;
            this.IsVersioned = isVersioned;
        }

        public ISymbol Member { get; }

        public string Id => this.Member.Name;

        public ITypeSymbol Type { get; }

        public string ReadExpression => this.Member is IMethodSymbol ? $"self.{EscapeIdentifier(this.Id)}()" : $"self.{EscapeIdentifier(this.Id)}";

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
