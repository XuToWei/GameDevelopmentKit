using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ET.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class ETReactiveSystemGenerator: ISourceGenerator
{
    private const string SystemAttributeName = "ET.ETReactiveSystemOfAttribute";
    private const string SourceAttributeName = "ET.ETReactiveSourceAttribute";
    private const string BindAttributeName = "ET.ETReactiveBindAttribute";
    private const string ReactiveStateTypeName = "ET.ETReactiveState";
    private const string VersionInterfaceName = "ReactiveBinding.IVersion";
    private const string ObserveMethodName = "ObserveReactive";
    private const string ResetMethodName = "ResetReactive";

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxReceiver receiver || receiver.SystemDeclarations.Count == 0)
        {
            return;
        }

        HashSet<INamedTypeSymbol> processedSystems = new(SymbolEqualityComparer.Default);
        foreach (ClassDeclarationSyntax declaration in receiver.SystemDeclarations)
        {
            SemanticModel semanticModel = context.Compilation.GetSemanticModel(declaration.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not INamedTypeSymbol system ||
                !processedSystems.Add(system))
            {
                continue;
            }

            GenerateSystem(context, declaration, system);
        }
    }

    private static void GenerateSystem(
        GeneratorExecutionContext context,
        ClassDeclarationSyntax declaration,
        INamedTypeSymbol system)
    {
        AttributeData? systemAttribute = GetAttribute(system, SystemAttributeName);
        if (systemAttribute == null)
        {
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

        if (!TryGetOwnerAndStateMemberName(systemAttribute, out INamedTypeSymbol? owner, out string? stateMemberName) ||
            owner == null ||
            stateMemberName == null ||
            SymbolEqualityComparer.Default.Equals(owner.ContainingAssembly, context.Compilation.Assembly))
        {
            Report(
                context,
                ETReactiveDiagnosticRules.Owner,
                declaration.Identifier.GetLocation(),
                system.ToDisplayString());
            return;
        }

        INamedTypeSymbol? reactiveStateType = context.Compilation.GetTypeByMetadataName(ReactiveStateTypeName);
        string? generatedStateMemberName = null;
        if (reactiveStateType == null ||
            !TryGetStateMember(
                context.Compilation,
                system,
                owner,
                stateMemberName,
                reactiveStateType,
                out generatedStateMemberName))
        {
            Report(
                context,
                ETReactiveDiagnosticRules.StateMember,
                declaration.Identifier.GetLocation(),
                owner.ToDisplayString(),
                stateMemberName);
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

            if (ContainsTypeFromAssembly(method.ReturnType, context.Compilation.Assembly))
            {
                Report(
                    context,
                    ETReactiveDiagnosticRules.HotfixSourceType,
                    GetLocation(method, declaration),
                    method.Name,
                    method.ReturnType.ToDisplayString());
                valid = false;
                continue;
            }

            sources.Add(new SourceModel(
                method,
                ImplementsInterface(method.ReturnType, versionInterface)));
        }

        sources.Sort(static (left, right) => StringComparer.Ordinal.Compare(left.Id, right.Id));
        Dictionary<string, SourceModel> sourceById = new(StringComparer.Ordinal);
        int slotCount = 0;
        foreach (SourceModel source in sources)
        {
            source.ValueSlotIndex = slotCount++;
            if (source.IsVersioned)
            {
                source.VersionSlotIndex = slotCount++;
            }

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

        valid &= ValidateGeneratedMethodCollision(context, declaration, system, owner, ObserveMethodName);
        valid &= ValidateGeneratedMethodCollision(context, declaration, system, owner, ResetMethodName);

        if (!valid || generatedStateMemberName == null)
        {
            return;
        }

        long groupId = ComputeHash(
            $"{system.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}|" +
            owner.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        long schemaId = ComputeSchemaHash(context, system, sources, binds, slotCount);
        string generatedCode = Emit(
            system,
            owner,
            generatedStateMemberName,
            sources,
            binds,
            groupId,
            schemaId,
            slotCount);

        context.AddSource(
            $"ETReactiveSystemGenerator.{GetMetadataName(system)}.g.cs",
            generatedCode);
    }

    private static bool TryGetOwnerAndStateMemberName(
        AttributeData attribute,
        out INamedTypeSymbol? owner,
        out string? stateMemberName)
    {
        owner = null;
        stateMemberName = null;
        if (attribute.ConstructorArguments.Length != 2 ||
            attribute.ConstructorArguments[0].Value is not INamedTypeSymbol ownerValue ||
            attribute.ConstructorArguments[1].Value is not string stateMemberNameValue ||
            string.IsNullOrWhiteSpace(stateMemberNameValue))
        {
            return false;
        }

        owner = ownerValue;
        stateMemberName = stateMemberNameValue;
        return true;
    }

    private static bool TryGetStateMember(
        Compilation compilation,
        INamedTypeSymbol system,
        INamedTypeSymbol owner,
        string stateMemberName,
        INamedTypeSymbol reactiveStateType,
        out string? generatedStateMemberName)
    {
        generatedStateMemberName = null;
        INamedTypeSymbol? currentType = owner;
        while (currentType != null)
        {
            ImmutableArray<ISymbol> members = currentType.GetMembers(stateMemberName);
            if (members.Length > 0)
            {
                foreach (ISymbol member in members)
                {
                    if (member is IFieldSymbol field &&
                        !field.IsStatic &&
                        SymbolEqualityComparer.Default.Equals(field.Type, reactiveStateType) &&
                        compilation.IsSymbolAccessibleWithin(field, system))
                    {
                        generatedStateMemberName = EscapeIdentifier(field.Name);
                        return true;
                    }

                    if (member is IPropertySymbol property &&
                        !property.IsStatic &&
                        !property.IsIndexer &&
                        property.GetMethod != null &&
                        SymbolEqualityComparer.Default.Equals(property.Type, reactiveStateType) &&
                        compilation.IsSymbolAccessibleWithin(property.GetMethod, system))
                    {
                        generatedStateMemberName = EscapeIdentifier(property.Name);
                        return true;
                    }
                }

                return false;
            }

            currentType = currentType.BaseType;
        }

        return false;
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

    private static bool ContainsTypeFromAssembly(ITypeSymbol type, IAssemblySymbol assembly)
    {
        if (type is IArrayTypeSymbol arrayType)
        {
            return ContainsTypeFromAssembly(arrayType.ElementType, assembly);
        }

        if (type is IPointerTypeSymbol pointerType)
        {
            return ContainsTypeFromAssembly(pointerType.PointedAtType, assembly);
        }

        if (type is IFunctionPointerTypeSymbol functionPointerType)
        {
            return ContainsTypeFromAssembly(functionPointerType.Signature.ReturnType, assembly) ||
                   functionPointerType.Signature.Parameters.Any(parameter =>
                       ContainsTypeFromAssembly(parameter.Type, assembly));
        }

        if (type is ITypeParameterSymbol typeParameter)
        {
            return SymbolEqualityComparer.Default.Equals(typeParameter.ContainingAssembly, assembly) ||
                   typeParameter.ConstraintTypes.Any(constraint => ContainsTypeFromAssembly(constraint, assembly));
        }

        if (type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        if (SymbolEqualityComparer.Default.Equals(namedType.ContainingAssembly, assembly))
        {
            return true;
        }

        return namedType.TypeArguments.Any(argument => ContainsTypeFromAssembly(argument, assembly));
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

    private static string Emit(
        INamedTypeSymbol system,
        INamedTypeSymbol owner,
        string stateMemberName,
        IReadOnlyList<SourceModel> sources,
        IReadOnlyList<BindModel> binds,
        long groupId,
        long schemaId,
        int slotCount)
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
        string groupIdExpression = ToLongExpression(groupId);
        string schemaIdExpression = ToLongExpression(schemaId);
        code.Append(indentation)
                .Append("static partial class ")
                .Append(EscapeIdentifier(system.Name))
                .AppendLine();
        code.Append(indentation).AppendLine("{");
        string bodyIndentation = indentation + "    ";
        string statementIndentation = bodyIndentation + "    ";

        code.Append(bodyIndentation)
                .Append("public static void ")
                .Append(ObserveMethodName)
                .Append("(this ")
                .Append(ownerType)
                .AppendLine(" self)");
        code.Append(bodyIndentation).AppendLine("{");
        code.Append(statementIndentation)
                .Append("global::ET.ETReactiveGroupState __reactiveGroup = self.")
                .Append(stateMemberName)
                .Append(".GetOrCreateGroup(")
                .Append(groupIdExpression)
                .Append(", ")
                .Append(schemaIdExpression)
                .Append(", ")
                .Append(slotCount)
                .AppendLine(");");
        code.Append(statementIndentation)
                .AppendLine("bool __reactiveInitialized = __reactiveGroup.Initialized;");

        for (int index = 0; index < sources.Count; ++index)
        {
            SourceModel source = sources[index];
            string typeName = source.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            string sourceMethodName = EscapeIdentifier(source.Method.Name);
            code.AppendLine();
            code.Append(statementIndentation)
                    .Append(typeName)
                    .Append(" __reactiveCurrent")
                    .Append(index)
                    .Append(" = ")
                    .Append(sourceMethodName)
                    .AppendLine("(self);");
            code.Append(statementIndentation)
                    .Append("global::ET.ETReactiveSlot<")
                    .Append(typeName)
                    .Append("> __reactiveSlot")
                    .Append(index)
                    .Append(" = __reactiveGroup.GetSlot<")
                    .Append(typeName)
                    .Append(">(")
                    .Append(source.ValueSlotIndex)
                    .AppendLine(");");
            code.Append(statementIndentation)
                    .Append(typeName)
                    .Append(" __reactiveOld")
                    .Append(index)
                    .Append(" = __reactiveInitialized ? __reactiveSlot")
                    .Append(index)
                    .Append(".Value : __reactiveCurrent")
                    .Append(index)
                    .AppendLine(";");
            code.Append(statementIndentation)
                    .Append("bool __reactiveChanged")
                    .Append(index)
                    .Append(" = !__reactiveInitialized || !global::System.Collections.Generic.EqualityComparer<")
                    .Append(typeName)
                    .Append(">.Default.Equals(__reactiveOld")
                    .Append(index)
                    .Append(", __reactiveCurrent")
                    .Append(index)
                    .Append(')');

            if (source.IsVersioned)
            {
                code.AppendLine(";");
                code.Append(statementIndentation)
                        .Append("int __reactiveCurrentVersion")
                        .Append(index)
                        .Append(" = global::System.Object.ReferenceEquals(__reactiveCurrent")
                        .Append(index)
                        .Append(", null) ? 0 : ((global::ReactiveBinding.IVersion)(object)__reactiveCurrent")
                        .Append(index)
                        .AppendLine(").Version;");
                code.Append(statementIndentation)
                        .Append("global::ET.ETReactiveSlot<int> __reactiveVersionSlot")
                        .Append(index)
                        .Append(" = __reactiveGroup.GetSlot<int>(")
                        .Append(source.VersionSlotIndex)
                        .AppendLine(");");
                code.Append(statementIndentation)
                        .Append("__reactiveChanged")
                        .Append(index)
                        .Append(" = __reactiveChanged")
                        .Append(index)
                        .Append(" || __reactiveVersionSlot")
                        .Append(index)
                        .Append(".Value != __reactiveCurrentVersion")
                        .Append(index)
                        .AppendLine(";");
            }
            else
            {
                code.AppendLine(";");
            }
        }

        if (sources.Count > 0)
        {
            code.AppendLine();
        }

        for (int index = 0; index < sources.Count; ++index)
        {
            SourceModel source = sources[index];
            code.Append(statementIndentation)
                    .Append("__reactiveSlot")
                    .Append(index)
                    .Append(".Value = __reactiveCurrent")
                    .Append(index)
                    .AppendLine(";");
            if (source.IsVersioned)
            {
                code.Append(statementIndentation)
                        .Append("__reactiveVersionSlot")
                        .Append(index)
                        .Append(".Value = __reactiveCurrentVersion")
                        .Append(index)
                        .AppendLine(";");
            }
        }

        code.Append(statementIndentation).AppendLine("__reactiveGroup.Initialized = true;");

        foreach (BindModel bind in binds)
        {
            code.AppendLine();
            code.Append(statementIndentation).Append("if (");
            for (int sourceIndex = 0; sourceIndex < bind.ReactiveIds.Count; ++sourceIndex)
            {
                if (sourceIndex > 0)
                {
                    code.Append(" || ");
                }

                int modelIndex = GetSourceIndex(sources, bind.ReactiveIds[sourceIndex]);
                code.Append("__reactiveChanged").Append(modelIndex);
            }

            code.AppendLine(")");
            code.Append(statementIndentation).AppendLine("{");
            code.Append(statementIndentation)
                    .Append("    ")
                    .Append(EscapeIdentifier(bind.Method.Name))
                    .Append("(self");

            int valueParameterCount = bind.Method.Parameters.Length - 1;
            if (valueParameterCount == bind.ReactiveIds.Count)
            {
                foreach (string reactiveId in bind.ReactiveIds)
                {
                    code.Append(", __reactiveCurrent").Append(GetSourceIndex(sources, reactiveId));
                }
            }
            else if (valueParameterCount == bind.ReactiveIds.Count * 2)
            {
                foreach (string reactiveId in bind.ReactiveIds)
                {
                    int sourceIndex = GetSourceIndex(sources, reactiveId);
                    code.Append(", __reactiveOld").Append(sourceIndex);
                    code.Append(", __reactiveCurrent").Append(sourceIndex);
                }
            }

            code.AppendLine(");");
            code.Append(statementIndentation).AppendLine("}");
        }

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
                .Append("self.")
                .Append(stateMemberName)
                .Append(".Reset(")
                .Append(groupIdExpression)
                .AppendLine(");");
        code.Append(bodyIndentation).AppendLine("}");
        code.Append(indentation).AppendLine("}");

        if (!system.ContainingNamespace.IsGlobalNamespace)
        {
            code.AppendLine("}");
        }

        return code.ToString();
    }

    private static int GetSourceIndex(IReadOnlyList<SourceModel> sources, string id)
    {
        for (int index = 0; index < sources.Count; ++index)
        {
            if (StringComparer.Ordinal.Equals(sources[index].Id, id))
            {
                return index;
            }
        }

        throw new InvalidOperationException($"Reactive source '{id}' was not validated.");
    }

    private static long ComputeSchemaHash(
        GeneratorExecutionContext context,
        INamedTypeSymbol system,
        IReadOnlyList<SourceModel> sources,
        IReadOnlyList<BindModel> binds,
        int slotCount)
    {
        StringBuilder schema = new();
        schema.Append(GetMetadataName(system)).Append('|').Append(slotCount);
        foreach (SourceModel source in sources)
        {
            schema.Append("|S:")
                    .Append(source.Id)
                    .Append(':')
                    .Append(source.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                    .Append(':')
                    .Append(source.IsVersioned ? 'V' : 'E');
            AppendMethodSyntax(context, schema, source.Method);
        }

        foreach (BindModel bind in binds)
        {
            schema.Append("|B:")
                    .Append(bind.Method.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                    .Append(':')
                    .Append(string.Join(",", bind.ReactiveIds));
            AppendMethodSyntax(context, schema, bind.Method);
        }

        return ComputeHash(schema.ToString());
    }

    private static void AppendMethodSyntax(
        GeneratorExecutionContext context,
        StringBuilder schema,
        IMethodSymbol method)
    {
        SyntaxReference? syntaxReference = method.DeclaringSyntaxReferences.FirstOrDefault();
        if (syntaxReference?.GetSyntax(context.CancellationToken) is MethodDeclarationSyntax methodSyntax)
        {
            schema.Append(':').Append(methodSyntax.ToFullString());
        }
    }

    private static long ComputeHash(string text)
    {
        const ulong seed = 1313;
        ulong hash = 0;
        unchecked
        {
            foreach (char character in text)
            {
                hash = hash * seed + (byte)(character >> 8);
                hash = hash * seed + (byte)(character & byte.MaxValue);
            }
        }

        return unchecked((long)hash);
    }

    private static string ToLongExpression(long value)
    {
        return $"unchecked((long)0x{unchecked((ulong)value):X16}UL)";
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

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is not ClassDeclarationSyntax declaration || declaration.AttributeLists.Count == 0 ||
                context.SemanticModel.GetDeclaredSymbol(declaration) is not INamedTypeSymbol system ||
                GetAttribute(system, SystemAttributeName) == null)
            {
                return;
            }

            this.SystemDeclarations.Add(declaration);
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

        public int ValueSlotIndex { get; set; }

        public int VersionSlotIndex { get; set; } = -1;
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
