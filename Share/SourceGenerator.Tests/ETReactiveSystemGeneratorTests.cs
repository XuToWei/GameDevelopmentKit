using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using NUnit.Framework;

namespace Share.SourceGenerator.Tests;

[TestFixture]
public sealed class ETReactiveSystemGeneratorTests
{
    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.CSharp9);

    private static readonly IReadOnlyList<MetadataReference> FrameworkReferences =
        ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty)
        .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
        .Where(path => !string.Equals(
            path,
            typeof(ETReactiveSystemGeneratorTests).Assembly.Location,
            StringComparison.OrdinalIgnoreCase))
        .Select(static path => MetadataReference.CreateFromFile(path))
        .ToArray();

    [Test]
    public void Generated_system_is_fieldless_and_supports_all_callback_shapes()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [ETReactiveSystemOf(typeof(ReactiveOwner), nameof(ReactiveOwner.ReactiveState))]
                public static partial class ReactiveOwnerSystem
                {
                    [ETReactiveSource]
                    private static int Number(this ReactiveOwner self) => self.Number;

                    [ETReactiveSource]
                    private static string Text(this ReactiveOwner self) => self.Text;

                    [ETReactiveBind(nameof(Number))]
                    private static void RefreshWithoutValues(this ReactiveOwner self)
                    {
                        ++self.ZeroValueCalls;
                    }

                    [ETReactiveBind(nameof(Number), nameof(Text))]
                    private static void RefreshCurrentValues(this ReactiveOwner self, int number, string text)
                    {
                        ++self.CurrentValueCalls;
                        self.CurrentNumber = number;
                        self.CurrentText = text;
                    }

                    [ETReactiveBind(nameof(Number), nameof(Text))]
                    private static void RefreshOldAndCurrentValues(
                        this ReactiveOwner self,
                        int oldNumber,
                        int number,
                        string oldText,
                        string text)
                    {
                        ++self.OldAndCurrentCalls;
                        self.OldNumber = oldNumber;
                        self.NewNumber = number;
                        self.OldText = oldText;
                        self.NewText = text;
                    }
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference);
        AssertNoErrors(run);

        SyntaxNode generatedRoot = CSharpSyntaxTree.ParseText(run.GeneratedSource, ParseOptions).GetRoot();
        Assert.That(generatedRoot.DescendantNodes().OfType<FieldDeclarationSyntax>(), Is.Empty,
            "HotfixView 生成代码不能持有跨重载状态字段");
        Assert.That(generatedRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>(), Is.Empty,
            "HotfixView 生成代码不能持有跨重载状态属性");

        var loadContext = new DynamicAssemblyLoadContext();
        Assembly stableAssembly = Load(loadContext, stable.Image);
        byte[] hotfixImage = Emit(run.OutputCompilation);
        Assembly hotfixAssembly = Load(loadContext, hotfixImage);
        Type ownerType = stableAssembly.GetType("ET.ReactiveOwner", throwOnError: true)!;
        Type systemType = hotfixAssembly.GetType("Demo.ReactiveOwnerSystem", throwOnError: true)!;
        object owner = Activator.CreateInstance(ownerType)!;

        Set(owner, "Number", 7);
        Set(owner, "Text", "first");

        Invoke(systemType, "ObserveReactive", ownerType, owner);
        AssertOwner(owner, zeroCalls: 1, currentCalls: 1, pairCalls: 1,
            currentNumber: 7, currentText: "first",
            oldNumber: 7, newNumber: 7, oldText: "first", newText: "first");

        Invoke(systemType, "ObserveReactive", ownerType, owner);
        AssertOwner(owner, zeroCalls: 1, currentCalls: 1, pairCalls: 1,
            currentNumber: 7, currentText: "first",
            oldNumber: 7, newNumber: 7, oldText: "first", newText: "first");

        Set(owner, "Number", 9);
        Invoke(systemType, "ObserveReactive", ownerType, owner);
        AssertOwner(owner, zeroCalls: 2, currentCalls: 2, pairCalls: 2,
            currentNumber: 9, currentText: "first",
            oldNumber: 7, newNumber: 9, oldText: "first", newText: "first");

        Invoke(systemType, "ResetReactive", ownerType, owner);
        Invoke(systemType, "ObserveReactive", ownerType, owner);
        AssertOwner(owner, zeroCalls: 3, currentCalls: 3, pairCalls: 3,
            currentNumber: 9, currentText: "first",
            oldNumber: 9, newNumber: 9, oldText: "first", newText: "first");
    }

    [Test]
    public void Version_source_detects_internal_changes_and_value_replacement()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [ETReactiveSystemOf(typeof(ReactiveOwner), nameof(ReactiveOwner.ReactiveState))]
                public static partial class VersionReactiveSystem
                {
                    [ETReactiveSource]
                    private static VersionedValue Versioned(this ReactiveOwner self) => self.Versioned;

                    [ETReactiveBind(nameof(Versioned))]
                    private static void Refresh(this ReactiveOwner self, VersionedValue value)
                    {
                        ++self.VersionCalls;
                        self.ObservedVersion = value.Version;
                    }
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference);
        AssertNoErrors(run);

        var loadContext = new DynamicAssemblyLoadContext();
        Assembly stableAssembly = Load(loadContext, stable.Image);
        Assembly hotfixAssembly = Load(loadContext, Emit(run.OutputCompilation));
        Type ownerType = stableAssembly.GetType("ET.ReactiveOwner", throwOnError: true)!;
        Type valueType = stableAssembly.GetType("ET.VersionedValue", throwOnError: true)!;
        Type systemType = hotfixAssembly.GetType("Demo.VersionReactiveSystem", throwOnError: true)!;
        object owner = Activator.CreateInstance(ownerType)!;
        object originalValue = Get(owner, "Versioned")!;

        Invoke(systemType, "ObserveReactive", ownerType, owner);
        Invoke(systemType, "ObserveReactive", ownerType, owner);
        Assert.That(Get(owner, "VersionCalls"), Is.EqualTo(1));
        Assert.That(Get(owner, "ObservedVersion"), Is.EqualTo(0));

        valueType.GetMethod("IncrementVersion")!.Invoke(originalValue, null);
        Invoke(systemType, "ObserveReactive", ownerType, owner);
        Assert.That(Get(owner, "VersionCalls"), Is.EqualTo(2));
        Assert.That(Get(owner, "ObservedVersion"), Is.EqualTo(1));

        object replacement = Activator.CreateInstance(valueType)!;
        valueType.GetMethod("IncrementVersion")!.Invoke(replacement, null);
        Set(owner, "Versioned", replacement);
        Invoke(systemType, "ObserveReactive", ownerType, owner);
        Assert.That(Get(owner, "VersionCalls"), Is.EqualTo(3),
            "相同版本的新对象仍应由 EqualityComparer 检测为变化");
    }

    [Test]
    public void Version_source_rejects_old_current_callback()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [ETReactiveSystemOf(typeof(ReactiveOwner), nameof(ReactiveOwner.ReactiveState))]
                public static partial class InvalidVersionReactiveSystem
                {
                    [ETReactiveSource]
                    private static VersionedValue Versioned(this ReactiveOwner self) => self.Versioned;

                    [ETReactiveBind(nameof(Versioned))]
                    private static void Refresh(
                        this ReactiveOwner self,
                        VersionedValue oldValue,
                        VersionedValue currentValue)
                    {
                    }
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference);

        Assert.That(run.GeneratorDiagnostics.Select(static diagnostic => diagnostic.Id),
            Does.Contain("ET1109"));
    }

    [Test]
    public void Reload_version_invalidates_existing_runtime_group()
    {
        var state = new global::ET.ETReactiveState();
        global::ET.ETReactiveGroupState group = state.GetOrCreateGroup(17, 23, 1);
        global::ET.ETReactiveSlot<int> oldSlot = group.GetSlot<int>(0);
        oldSlot.Value = 42;
        group.Initialized = true;

        global::ET.ETReactiveRuntime.NotifyCodeReload();

        global::ET.ETReactiveGroupState reloadedGroup = state.GetOrCreateGroup(17, 23, 1);
        global::ET.ETReactiveSlot<int> newSlot = reloadedGroup.GetSlot<int>(0);
        Assert.Multiple(() =>
        {
            Assert.That(reloadedGroup, Is.SameAs(group));
            Assert.That(reloadedGroup.Initialized, Is.False);
            Assert.That(newSlot, Is.Not.SameAs(oldSlot));
            Assert.That(newSlot.Value, Is.Zero);
        });
    }

    [TestCase(
        "[ETReactiveBind(nameof(Value))] private static int Invalid(this ReactiveOwner self, int value) => value;",
        "ET1109")]
    [TestCase(
        "[ETReactiveBind(\"Missing\")] private static void Missing(this ReactiveOwner self) { }",
        "ET1107")]
    [TestCase(
        "[ETReactiveSource] private static HotfixPayload Payload(this ReactiveOwner self) => new HotfixPayload();",
        "ET1106")]
    public void Invalid_declarations_report_expected_diagnostic(string declaration, string diagnosticId)
    {
        StableAssembly stable = CompileStableAssembly();
        string source = $$"""
            using ET;

            namespace Demo
            {
                public sealed class HotfixPayload
                {
                }

                [ETReactiveSystemOf(typeof(ReactiveOwner), nameof(ReactiveOwner.ReactiveState))]
                public static partial class InvalidReactiveSystem
                {
                    [ETReactiveSource]
                    private static int Value(this ReactiveOwner self) => self.Number;

                    {{declaration}}
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference);

        Assert.That(run.GeneratorDiagnostics.Select(static diagnostic => diagnostic.Id),
            Does.Contain(diagnosticId),
            string.Join(Environment.NewLine, run.GeneratorDiagnostics.Select(static diagnostic => diagnostic.ToString())));
    }

    private static StableAssembly CompileStableAssembly()
    {
        string assemblyName = $"ET.ModelView.Tests.{Guid.NewGuid():N}";
        const string source = """
            using System;
            using System.Collections.Generic;
            using ReactiveBinding;

            namespace ReactiveBinding
            {
                public interface IVersion
                {
                    int Version { get; }
                    IVersion Parent { get; set; }
                    void IncrementVersion();
                }
            }

            namespace ET
            {
                [AttributeUsage(AttributeTargets.Class)]
                public sealed class ETReactiveSystemOfAttribute : Attribute
                {
                    public ETReactiveSystemOfAttribute(Type type, string stateMemberName)
                    {
                        Type = type;
                        StateMemberName = stateMemberName;
                    }

                    public Type Type { get; }
                    public string StateMemberName { get; }
                }

                [AttributeUsage(AttributeTargets.Method)]
                public sealed class ETReactiveSourceAttribute : Attribute
                {
                }

                [AttributeUsage(AttributeTargets.Method)]
                public sealed class ETReactiveBindAttribute : Attribute
                {
                    public ETReactiveBindAttribute(params string[] reactiveIds)
                    {
                        ReactiveIds = reactiveIds;
                    }

                    public string[] ReactiveIds { get; }
                }

                public sealed class ETReactiveState
                {
                    private readonly Dictionary<long, ETReactiveGroupState> groups =
                        new Dictionary<long, ETReactiveGroupState>();

                    public ETReactiveGroupState GetOrCreateGroup(long groupId, long schemaId, int slotCount)
                    {
                        ETReactiveGroupState group;
                        if (!groups.TryGetValue(groupId, out group))
                        {
                            group = new ETReactiveGroupState(schemaId, slotCount);
                            groups.Add(groupId, group);
                        }
                        else
                        {
                            group.EnsureSchema(schemaId, slotCount);
                        }

                        return group;
                    }

                    public void Reset(long groupId)
                    {
                        ETReactiveGroupState group;
                        if (groups.TryGetValue(groupId, out group))
                        {
                            group.Reset();
                        }
                    }
                }

                public sealed class ETReactiveGroupState
                {
                    private long schemaId;
                    private object[] slots;

                    public ETReactiveGroupState(long schemaId, int slotCount)
                    {
                        this.schemaId = schemaId;
                        slots = new object[slotCount];
                    }

                    public bool Initialized { get; set; }

                    public ETReactiveSlot<T> GetSlot<T>(int index)
                    {
                        object value = slots[index];
                        if (value == null)
                        {
                            var slot = new ETReactiveSlot<T>();
                            slots[index] = slot;
                            return slot;
                        }

                        return (ETReactiveSlot<T>)value;
                    }

                    public void EnsureSchema(long value, int slotCount)
                    {
                        if (schemaId == value && slots.Length == slotCount)
                        {
                            return;
                        }

                        schemaId = value;
                        slots = new object[slotCount];
                        Initialized = false;
                    }

                    public void Reset()
                    {
                        Initialized = false;
                    }
                }

                public sealed class ETReactiveSlot<T>
                {
                    public T Value { get; set; }
                }

                public sealed class VersionedValue : IVersion
                {
                    public int Version { get; private set; }
                    public IVersion Parent { get; set; }

                    public void IncrementVersion()
                    {
                        ++Version;
                    }
                }

                public sealed class ReactiveOwner
                {
                    public readonly ETReactiveState ReactiveState = new ETReactiveState();

                    public int Number;
                    public string Text = string.Empty;
                    public VersionedValue Versioned = new VersionedValue();

                    public int ZeroValueCalls;
                    public int CurrentValueCalls;
                    public int OldAndCurrentCalls;
                    public int VersionCalls;
                    public int ObservedVersion;
                    public int CurrentNumber;
                    public string CurrentText = string.Empty;
                    public int OldNumber;
                    public int NewNumber;
                    public string OldText = string.Empty;
                    public string NewText = string.Empty;
                }
            }
            """;

        CSharpCompilation compilation = CreateCompilation(assemblyName, source, Array.Empty<MetadataReference>());
        byte[] image = Emit(compilation);
        return new StableAssembly(MetadataReference.CreateFromImage(image), image);
    }

    private static GeneratorRun RunGenerator(string source, MetadataReference stableReference)
    {
        CSharpCompilation compilation = CreateCompilation(
            $"Game.ET.Code.HotfixView.Tests.{Guid.NewGuid():N}",
            source,
            new[] { stableReference });

        ISourceGenerator generator = CreateGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new[] { generator }, parseOptions: ParseOptions);
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out Compilation outputCompilation,
            out _);

        GeneratorDriverRunResult runResult = driver.GetRunResult();
        string generatedSource = string.Join(
            Environment.NewLine,
            runResult.Results.SelectMany(static result => result.GeneratedSources)
                .Select(static generated => generated.SourceText.ToString()));
        Diagnostic[] diagnostics = runResult.Diagnostics
            .Concat(runResult.Results.SelectMany(static result => result.Diagnostics))
            .GroupBy(static diagnostic => (diagnostic.Id, diagnostic.Location.SourceSpan, diagnostic.GetMessage()))
            .Select(static group => group.First())
            .ToArray();

        return new GeneratorRun((CSharpCompilation)outputCompilation, generatedSource, diagnostics);
    }

    private static ISourceGenerator CreateGenerator()
    {
        Assembly generatorAssembly = typeof(ET.Generator.ETSystemGenerator).Assembly;
        Type generatorType = generatorAssembly.GetTypes()
            .Single(type => type.Name == "ETReactiveSystemGenerator");
        return (ISourceGenerator)Activator.CreateInstance(generatorType)!;
    }

    private static CSharpCompilation CreateCompilation(
        string assemblyName,
        string source,
        IEnumerable<MetadataReference> additionalReferences)
    {
        return CSharpCompilation.Create(
            assemblyName,
            new[] { CSharpSyntaxTree.ParseText(source, ParseOptions) },
            FrameworkReferences.Concat(additionalReferences),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));
    }

    private static byte[] Emit(Compilation compilation)
    {
        using var stream = new MemoryStream();
        EmitResult result = compilation.Emit(stream);
        Assert.That(result.Success, Is.True,
            string.Join(Environment.NewLine, result.Diagnostics.Select(static diagnostic => diagnostic.ToString())));
        return stream.ToArray();
    }

    private static Assembly Load(AssemblyLoadContext context, byte[] image)
    {
        using var stream = new MemoryStream(image, writable: false);
        return context.LoadFromStream(stream);
    }

    private static void AssertNoErrors(GeneratorRun run)
    {
        Diagnostic[] diagnostics = run.GeneratorDiagnostics
            .Concat(run.OutputCompilation.GetDiagnostics())
            .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToArray();
        Assert.That(diagnostics, Is.Empty,
            string.Join(Environment.NewLine, diagnostics.Select(static diagnostic => diagnostic.ToString())));
        Assert.That(run.GeneratedSource, Is.Not.Empty);
    }

    private static void Invoke(Type systemType, string methodName, Type ownerType, object owner)
    {
        MethodInfo method = systemType.GetMethod(
            methodName,
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: new[] { ownerType },
            modifiers: null)!;
        Assert.That(method, Is.Not.Null, $"未生成 {methodName}({ownerType.Name})");
        method.Invoke(null, new[] { owner });
    }

    private static void Set(object instance, string fieldName, object value)
    {
        instance.GetType().GetField(fieldName)!.SetValue(instance, value);
    }

    private static object? Get(object instance, string fieldName)
    {
        return instance.GetType().GetField(fieldName)!.GetValue(instance);
    }

    private static void AssertOwner(
        object owner,
        int zeroCalls,
        int currentCalls,
        int pairCalls,
        int currentNumber,
        string currentText,
        int oldNumber,
        int newNumber,
        string oldText,
        string newText)
    {
        Assert.Multiple(() =>
        {
            Assert.That(Get(owner, "ZeroValueCalls"), Is.EqualTo(zeroCalls));
            Assert.That(Get(owner, "CurrentValueCalls"), Is.EqualTo(currentCalls));
            Assert.That(Get(owner, "OldAndCurrentCalls"), Is.EqualTo(pairCalls));
            Assert.That(Get(owner, "CurrentNumber"), Is.EqualTo(currentNumber));
            Assert.That(Get(owner, "CurrentText"), Is.EqualTo(currentText));
            Assert.That(Get(owner, "OldNumber"), Is.EqualTo(oldNumber));
            Assert.That(Get(owner, "NewNumber"), Is.EqualTo(newNumber));
            Assert.That(Get(owner, "OldText"), Is.EqualTo(oldText));
            Assert.That(Get(owner, "NewText"), Is.EqualTo(newText));
        });
    }

    private sealed record StableAssembly(MetadataReference Reference, byte[] Image);

    private sealed record GeneratorRun(
        CSharpCompilation OutputCompilation,
        string GeneratedSource,
        IReadOnlyList<Diagnostic> GeneratorDiagnostics);

    private sealed class DynamicAssemblyLoadContext : AssemblyLoadContext
    {
        public DynamicAssemblyLoadContext()
            : base(isCollectible: false)
        {
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
