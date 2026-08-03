using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using NUnit.Framework;
using ReactiveBinding;

namespace Share.SourceGenerator.Tests;

[TestFixture]
public sealed partial class ETReactiveSystemGeneratorTests
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

    private static readonly IReadOnlyList<MetadataReference> RuntimeReferences = new[]
    {
        MetadataReference.CreateFromFile(typeof(global::ET.IETReactiveHost).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(IVersion).Assembly.Location),
    };

    [SetUp]
    public void SetUp()
    {
        global::ET.World.Instance.Dispose();
    }

    [TearDown]
    public void TearDown()
    {
        global::ET.World.Instance.Dispose();
    }

    [Test]
    public void Generated_system_uses_a_hotfix_observer_and_supports_all_callback_shapes()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
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
        ClassDeclarationSyntax systemDeclaration = generatedRoot.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single(static declaration => declaration.Identifier.ValueText == "ReactiveOwnerSystem");
        Assert.That(systemDeclaration.Members.OfType<FieldDeclarationSyntax>(), Is.Empty,
            "System 自身仍应保持无字段");
        Assert.That(systemDeclaration.Members.OfType<PropertyDeclarationSyntax>(), Is.Empty,
            "System 自身仍应保持无属性");
        ClassDeclarationSyntax observerDeclaration = systemDeclaration.Members
            .OfType<ClassDeclarationSyntax>()
            .Single(static declaration => declaration.Identifier.ValueText == "__ETReactiveObserver");
        Assert.Multiple(() =>
        {
            Assert.That(observerDeclaration.BaseList?.ToString(), Does.Contain("IETReactiveObserver"));
            Assert.That(observerDeclaration.Members.OfType<FieldDeclarationSyntax>().Count(), Is.GreaterThanOrEqualTo(4),
                "DLL 版本、owner、initialized 与每个 source 的对比值都应存放在生成的 Hotfix Observer 中");
            Assert.That(run.GeneratedSource, Does.Contain("[global::ET.ETReactiveObserver(typeof(global::ET.ReactiveOwner))]"));
            Assert.That(run.GeneratedSource, Does.Contain("public int DllVersion => this.dllVersion;"));
            Assert.That(run.GeneratedSource,
                Does.Contain("reactiveSystem.Rent(typeof(global::ET.ReactiveOwner), reactiveHost)"));
            Assert.That(run.GeneratedSource, Does.Not.Contain("__reactive_pool"));
            Assert.That(run.GeneratedSource, Does.Not.Contain("ETReactiveState"));
            Assert.That(run.GeneratedSource, Does.Not.Contain("ETReactiveSlot"));
            Assert.That(run.GeneratedSource, Does.Not.Contain("ETReactiveObserverHandle"));
            Assert.That(run.GeneratedSource, Does.Not.Contain("ETReactiveObserverComponent"));
            Assert.That(run.GeneratedSource, Does.Not.Contain("self.Root()"));
        });

        var loadContext = new DynamicAssemblyLoadContext();
        Assembly stableAssembly = Load(loadContext, stable.Image);
        byte[] hotfixImage = Emit(run.OutputCompilation);
        Assembly hotfixAssembly = Load(loadContext, hotfixImage);
        Type ownerType = stableAssembly.GetType("ET.ReactiveOwner", throwOnError: true)!;
        Type systemType = hotfixAssembly.GetType("Demo.ReactiveOwnerSystem", throwOnError: true)!;
        object owner = Activator.CreateInstance(ownerType)!;

        Set(owner, "Number", 7);
        Set(owner, "Text", "first");

        Invoke(systemType, "ObserveChanges", ownerType, owner);
        object observerAfterFirstObserve = ownerType.GetProperty("ReactiveObserver")!.GetValue(owner)!;
        AssertOwner(owner, zeroCalls: 1, currentCalls: 1, pairCalls: 1,
            currentNumber: 7, currentText: "first",
            oldNumber: 7, newNumber: 7, oldText: "first", newText: "first");

        Invoke(systemType, "ObserveChanges", ownerType, owner);
        AssertOwner(owner, zeroCalls: 1, currentCalls: 1, pairCalls: 1,
            currentNumber: 7, currentText: "first",
            oldNumber: 7, newNumber: 7, oldText: "first", newText: "first");

        Set(owner, "Number", 9);
        Invoke(systemType, "ObserveChanges", ownerType, owner);
        AssertOwner(owner, zeroCalls: 2, currentCalls: 2, pairCalls: 2,
            currentNumber: 9, currentText: "first",
            oldNumber: 7, newNumber: 9, oldText: "first", newText: "first");

        Invoke(systemType, "ResetReactive", ownerType, owner);
        Invoke(systemType, "ObserveChanges", ownerType, owner);
        AssertOwner(owner, zeroCalls: 3, currentCalls: 3, pairCalls: 3,
            currentNumber: 9, currentText: "first",
            oldNumber: 9, newNumber: 9, oldText: "first", newText: "first");
        Assert.That(ownerType.GetProperty("ReactiveObserver")!.GetValue(owner), Is.SameAs(observerAfterFirstObserve),
            "DLL 版本不变时，每帧 ObserveChanges 不应重建 observer");

        object observerBeforeClear = ownerType.GetProperty("ReactiveObserver")!.GetValue(owner)!;
        Invoke(systemType, "ClearReactive", ownerType, owner);
        Assert.That(ownerType.GetProperty("ReactiveObserver")!.GetValue(owner), Is.Null);
        Invoke(systemType, "ObserveChanges", ownerType, owner);
        Assert.That(ownerType.GetProperty("ReactiveObserver")!.GetValue(owner), Is.SameAs(observerBeforeClear),
            "ClearReactive 后应从生成类型自己的缓存池复用 Observer");
        AssertOwner(owner, zeroCalls: 4, currentCalls: 4, pairCalls: 4,
            currentNumber: 9, currentText: "first",
            oldNumber: 9, newNumber: 9, oldText: "first", newText: "first");

        loadContext.Unload();
    }

    [Test]
    public void Version_source_detects_internal_changes_and_value_replacement()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
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

        Invoke(systemType, "ObserveChanges", ownerType, owner);
        Invoke(systemType, "ObserveChanges", ownerType, owner);
        Assert.That(Get(owner, "VersionCalls"), Is.EqualTo(1));
        Assert.That(Get(owner, "ObservedVersion"), Is.EqualTo(0));

        valueType.GetMethod("IncrementVersion")!.Invoke(originalValue, null);
        Invoke(systemType, "ObserveChanges", ownerType, owner);
        Assert.That(Get(owner, "VersionCalls"), Is.EqualTo(2));
        Assert.That(Get(owner, "ObservedVersion"), Is.EqualTo(1));

        object replacement = Activator.CreateInstance(valueType)!;
        valueType.GetMethod("IncrementVersion")!.Invoke(replacement, null);
        Set(owner, "Versioned", replacement);
        Invoke(systemType, "ObserveChanges", ownerType, owner);
        Assert.That(Get(owner, "VersionCalls"), Is.EqualTo(3),
            "相同版本但引用身份不同的新对象必须被检测为变化");

        loadContext.Unload();
    }

    [Test]
    public void Version_source_rejects_old_current_callback()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
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
    public void Reactive_system_pools_current_observers_and_drops_stale_versions()
    {
        global::ET.CodeTypes.Instance.SetTypes(new[] { typeof(RuntimeReactiveObserver) });
        global::ET.ETReactiveSystem system = global::ET.World.Instance.AddSingleton<global::ET.ETReactiveSystem>();
        var entity = new RuntimeReactiveEntity(1);
        global::ET.IETReactiveObserver firstObserver = system.Rent(typeof(RuntimeReactiveEntity), entity);

        Assert.Multiple(() =>
        {
            Assert.That(system.GetObserverType(typeof(RuntimeReactiveEntity)), Is.EqualTo(typeof(RuntimeReactiveObserver)));
            Assert.That(firstObserver.DllVersion, Is.EqualTo(system.DllVersion));
            Assert.That(firstObserver.OwnerInstanceId, Is.EqualTo(entity.InstanceId));
        });

        system.Recycle(firstObserver);
        global::ET.IETReactiveObserver pooledObserver = system.Rent(typeof(RuntimeReactiveEntity), entity);
        Assert.That(pooledObserver, Is.SameAs(firstObserver));

        int previousDllVersion = system.DllVersion;
        global::ET.ETReactiveSystem reloadedSystem =
            global::ET.World.Instance.AddSingleton<global::ET.ETReactiveSystem>();
        reloadedSystem.Recycle(pooledObserver);
        global::ET.IETReactiveObserver reloadedObserver =
            reloadedSystem.Rent(typeof(RuntimeReactiveEntity), entity);
        Assert.That(reloadedObserver, Is.Not.SameAs(pooledObserver),
            "旧 DLL 版本的 observer 只能释放，不能进入当前版本对象池");
        Assert.Multiple(() =>
        {
            Assert.That(reloadedSystem.DllVersion, Is.GreaterThan(previousDllVersion));
            Assert.That(reloadedObserver.DllVersion, Is.EqualTo(reloadedSystem.DllVersion));
        });
    }

    [Test]
    public void System_attribute_uses_entity_system_owner_interface()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class ReactiveOwnerSystem
                {
                    [ETReactiveSource]
                    private static int Number(this ReactiveOwner self) => self.Number;

                    [ETReactiveBind(nameof(Number))]
                    private static void Refresh(this ReactiveOwner self, int value)
                    {
                        self.CurrentNumber = value;
                    }
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference);

        AssertNoErrors(run);
        Assert.Multiple(() =>
        {
            Assert.That(run.GeneratedSource, Does.Contain("IETReactiveHost"));
            Assert.That(run.GeneratedSource, Does.Contain("ReactiveObserver"));
            Assert.That(run.GeneratedSource, Does.Contain("IReactiveObserver"));
            Assert.That(run.GeneratedSource, Does.Not.Contain("ReactiveHandle"));
        });
    }

    [TestCase("")]
    [TestCase("[LSEntitySystemOf(typeof(ReactiveOwner))]")]
    public void System_attribute_requires_entity_system_of_on_the_same_class(string otherSystemAttribute)
    {
        StableAssembly stable = CompileStableAssembly();
        string source = $$"""
            using ET;

            namespace Demo
            {
                {{otherSystemAttribute}}
                [ETReactiveSystem]
                public static partial class InvalidReactiveSystem
                {
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference);

        Assert.That(
            run.GeneratorDiagnostics.Select(static diagnostic => diagnostic.Id),
            Does.Contain("ET1102"));
        Assert.That(run.GeneratedSource, Is.Empty);
    }

    [Test]
    public void Multiple_reactive_systems_for_same_owner_report_diagnostic()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class FirstReactiveSystem
                {
                }

                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class SecondReactiveSystem
                {
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference);

        Assert.That(
            run.GeneratorDiagnostics.Count(static diagnostic => diagnostic.Id == "ET1111"),
            Is.EqualTo(2));
        Assert.That(run.GeneratedSource, Is.Empty);
    }

    [Test]
    public void Reactive_system_in_referenced_hotfix_assembly_reserves_owner()
    {
        StableAssembly stable = CompileStableAssembly();
        const string hotfixSource = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class HotfixReactiveSystem
                {
                }
            }
            """;
        GeneratorRun hotfixRun = RunGenerator(
            hotfixSource,
            stable.Reference,
            "Game.ET.Code.Hotfix");
        AssertNoErrors(hotfixRun);
        MetadataReference hotfixReference = MetadataReference.CreateFromImage(Emit(hotfixRun.OutputCompilation));

        const string hotfixViewSource = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class HotfixViewReactiveSystem
                {
                }
            }
            """;
        GeneratorRun hotfixViewRun = RunGenerator(
            hotfixViewSource,
            stable.Reference,
            "Game.ET.Code.HotfixView",
            new[] { hotfixReference });

        Assert.That(
            hotfixViewRun.GeneratorDiagnostics.Select(static diagnostic => diagnostic.Id),
            Does.Contain("ET1111"));
    }

    [TestCase("dynamic", "self.Boxed")]
    [TestCase("object", "self.Boxed")]
    [TestCase("PlainReference", "self.Reference")]
    public void Unsupported_reference_source_types_report_diagnostic(string typeName, string expression)
    {
        StableAssembly stable = CompileStableAssembly();
        string source = $$"""
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class InvalidReactiveSystem
                {
                    [ETReactiveSource]
                    private static {{typeName}} Value(this ReactiveOwner self) => {{expression}};
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference);

        Assert.That(
            run.GeneratorDiagnostics.Select(static diagnostic => diagnostic.Id),
            Does.Contain("ET1112"));
    }

    [Test]
    public void Custom_struct_without_equality_operator_reports_diagnostic()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class InvalidReactiveSystem
                {
                    [ETReactiveSource]
                    private static PlainStruct Value(this ReactiveOwner self) => self.Plain;
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference);

        Assert.That(
            run.GeneratorDiagnostics.Select(static diagnostic => diagnostic.Id),
            Does.Contain("ET1113"));
    }

    [Test]
    public void Supported_source_type_from_sibling_hotfix_assembly_is_stored_in_generated_observer()
    {
        StableAssembly stable = CompileStableAssembly();
        const string siblingSource = """
            namespace HotfixTypes
            {
                public struct Payload
                {
                    public int Value;

                    public static bool operator ==(Payload left, Payload right) => left.Value == right.Value;
                    public static bool operator !=(Payload left, Payload right) => !(left == right);
                    public override bool Equals(object obj) => obj is Payload other && this == other;
                    public override int GetHashCode() => Value;
                }
            }
            """;
        CSharpCompilation siblingCompilation = CreateCompilation(
            "Game.ET.Code.Hotfix",
            siblingSource,
            Array.Empty<MetadataReference>());
        MetadataReference siblingReference = MetadataReference.CreateFromImage(Emit(siblingCompilation));

        const string source = """
            using ET;
            using HotfixTypes;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class InvalidReactiveSystem
                {
                    [ETReactiveSource]
                    private static Payload Value(this ReactiveOwner self) => new Payload { Value = self.Number };

                    [ETReactiveBind(nameof(Value))]
                    private static void Refresh(this ReactiveOwner self, Payload value)
                    {
                        self.CurrentNumber = value.Value;
                    }
                }
            }
            """;

        GeneratorRun run = RunGenerator(
            source,
            stable.Reference,
            "Game.ET.Code.HotfixView",
            new[] { siblingReference });

        AssertNoErrors(run);
        Assert.That(run.GeneratedSource, Does.Contain("global::HotfixTypes.Payload __reactive_Value"));
    }

    [Test]
    public void Owner_without_reactive_interface_reports_diagnostic()
    {
        const string stableSource = """
            namespace InvalidModel
            {
                public sealed class InvalidReactiveOwner : ET.Entity
                {
                }
            }
            """;
        MetadataReference stableReference = CompileReference(
            $"InvalidModel.Tests.{Guid.NewGuid():N}",
            stableSource);
        const string source = """
            using ET;
            using InvalidModel;

            namespace Demo
            {
                [EntitySystemOf(typeof(InvalidReactiveOwner))]
                [ETReactiveSystem]
                public static partial class InvalidReactiveSystem
                {
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stableReference);

        Assert.That(run.GeneratorDiagnostics.Select(static diagnostic => diagnostic.Id),
            Does.Contain("ET1103"));
        Assert.That(run.GeneratedSource, Is.Empty);
    }

    [Test]
    public void Float_and_double_sources_use_reactive_binding_epsilon_semantics()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class FloatingPointReactiveSystem
                {
                    [ETReactiveSource]
                    private static float SingleValue(this ReactiveOwner self) => self.SingleValue;

                    [ETReactiveSource]
                    private static double DoubleValue(this ReactiveOwner self) => self.DoubleValue;

                    [ETReactiveBind(nameof(SingleValue))]
                    private static void RefreshSingle(this ReactiveOwner self)
                    {
                        ++self.SingleCalls;
                    }

                    [ETReactiveBind(nameof(DoubleValue))]
                    private static void RefreshDouble(this ReactiveOwner self)
                    {
                        ++self.DoubleCalls;
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
        Type systemType = hotfixAssembly.GetType("Demo.FloatingPointReactiveSystem", throwOnError: true)!;
        object owner = Activator.CreateInstance(ownerType)!;

        Set(owner, "SingleValue", 1f);
        Set(owner, "DoubleValue", 1d);
        Invoke(systemType, "ObserveChanges", ownerType, owner);

        Set(owner, "SingleValue", 1.0000005f);
        Set(owner, "DoubleValue", 1.0000000005d);
        Invoke(systemType, "ObserveChanges", ownerType, owner);
        Assert.That(Get(owner, "SingleCalls"), Is.EqualTo(1));
        Assert.That(Get(owner, "DoubleCalls"), Is.EqualTo(1));

        Set(owner, "SingleValue", 1.000002f);
        Set(owner, "DoubleValue", 1.000000002d);
        Invoke(systemType, "ObserveChanges", ownerType, owner);
        Assert.That(Get(owner, "SingleCalls"), Is.EqualTo(2));
        Assert.That(Get(owner, "DoubleCalls"), Is.EqualTo(2));

        Set(owner, "SingleValue", float.NaN);
        Set(owner, "DoubleValue", double.NaN);
        Invoke(systemType, "ObserveChanges", ownerType, owner);
        Invoke(systemType, "ObserveChanges", ownerType, owner);
        Assert.That(Get(owner, "SingleCalls"), Is.EqualTo(3));
        Assert.That(Get(owner, "DoubleCalls"), Is.EqualTo(3));

        loadContext.Unload();
    }

    [Test]
    public void Change_detection_order_matches_reactive_binding_native_generation()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class OrderedReactiveSystem
                {
                    [ETReactiveSource]
                    private static int A(this ReactiveOwner self) => self.Number;

                    [ETReactiveSource]
                    private static int B(this ReactiveOwner self) => self.CurrentNumber;

                    [ETReactiveBind(nameof(A))]
                    private static void ApplyA(this ReactiveOwner self, int value)
                    {
                        self.CurrentNumber = value * 10;
                    }

                    [ETReactiveBind(nameof(B))]
                    private static void ApplyB(this ReactiveOwner self, int value)
                    {
                        self.NewNumber = value;
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
        Type systemType = hotfixAssembly.GetType("Demo.OrderedReactiveSystem", throwOnError: true)!;
        object owner = Activator.CreateInstance(ownerType)!;

        Set(owner, "Number", 1);
        Invoke(systemType, "ObserveChanges", ownerType, owner);
        Assert.That(Get(owner, "NewNumber"), Is.EqualTo(0));

        Set(owner, "Number", 2);
        Invoke(systemType, "ObserveChanges", ownerType, owner);
        Assert.That(Get(owner, "NewNumber"), Is.EqualTo(20),
            "A 的回调修改 B 后，B 应像 ReactiveBinding 原生代码一样在同一次 Observe 中检测到变化");

        loadContext.Unload();
    }

    [Test]
    public void Unused_source_is_reported_and_not_evaluated_by_generated_code()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class ReactiveOwnerSystem
                {
                    [ETReactiveSource]
                    private static int Unused(this ReactiveOwner self) => self.Number;
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference);

        Assert.That(run.GeneratorDiagnostics.Single(static diagnostic => diagnostic.Id == "ET1114").Severity,
            Is.EqualTo(DiagnosticSeverity.Warning));
        Assert.That(run.GeneratedSource, Does.Not.Contain("Unused(self)"));
        AssertNoErrors(run);
    }

    [Test]
    public void Stable_owner_interface_only_exposes_the_observer_reference()
    {
        Type interfaceType = typeof(global::ET.IETReactiveHost);
        Assert.Multiple(() =>
        {
            Assert.That(interfaceType.GetFields(), Is.Empty);
            Assert.That(interfaceType.GetProperties().Select(static property => (property.Name, property.PropertyType)),
                Is.EquivalentTo(new[] { ("ReactiveObserver", typeof(IReactiveObserver)) }));
        });
    }

    [Test]
    public void Partial_reactive_host_gets_generated_observer_property()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                public sealed partial class GeneratedReactiveHost : Entity, IETReactiveHost
                {
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference);

        AssertNoErrors(run);
        Assert.Multiple(() =>
        {
            Assert.That(run.GeneratedSource, Does.Contain("partial class GeneratedReactiveHost"));
            Assert.That(run.GeneratedSource, Does.Contain("[global::MemoryPack.MemoryPackIgnore]"));
            Assert.That(run.GeneratedSource,
                Does.Contain("[global::MongoDB.Bson.Serialization.Attributes.BsonIgnore]"));
            Assert.That(run.GeneratedSource,
                Does.Contain("public global::ReactiveBinding.IReactiveObserver ReactiveObserver { get; set; }"));
        });
    }

    [Test]
    public void Non_partial_reactive_host_reports_diagnostic()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                public sealed class InvalidReactiveHost : Entity, IETReactiveHost
                {
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference);

        Assert.That(run.GeneratorDiagnostics.Select(static diagnostic => diagnostic.Id), Does.Contain("ET1106"));
        Assert.That(run.GeneratedSource, Is.Empty);
    }

    [Test]
    public void Adding_and_removing_sources_requires_only_hotfix_reload()
    {
        StableAssembly stable = CompileStableAssembly();
        const string oneSource = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class SchemaReactiveSystem
                {
                    [ETReactiveSource]
                    private static int Number(this ReactiveOwner self) => self.Number;

                    [ETReactiveBind(nameof(Number))]
                    private static void Refresh(this ReactiveOwner self, int number)
                    {
                        ++self.ZeroValueCalls;
                        self.CurrentNumber = number;
                    }
                }
            }
            """;
        const string twoSources = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class SchemaReactiveSystem
                {
                    [ETReactiveSource]
                    private static int Number(this ReactiveOwner self) => self.Number;

                    [ETReactiveSource]
                    private static string Text(this ReactiveOwner self) => self.Text;

                    [ETReactiveBind(nameof(Number), nameof(Text))]
                    private static void Refresh(this ReactiveOwner self, int number, string text)
                    {
                        ++self.CurrentValueCalls;
                        self.CurrentNumber = number;
                        self.CurrentText = text;
                    }
                }
            }
            """;

        GeneratorRun oneSourceRun = RunGenerator(oneSource, stable.Reference, "Game.ET.Code.HotfixView");
        GeneratorRun twoSourcesRun = RunGenerator(twoSources, stable.Reference, "Game.ET.Code.HotfixView");
        AssertNoErrors(oneSourceRun);
        AssertNoErrors(twoSourcesRun);

        Assembly stableAssembly = LoadDefault(stable.Image);
        Type ownerType = stableAssembly.GetType("ET.ReactiveOwner", throwOnError: true)!;
        object owner = Activator.CreateInstance(ownerType)!;
        Set(owner, "Number", 11);
        Set(owner, "Text", "two");

        WeakReference firstContext = ExecuteAndUnloadHotfix(
            stableAssembly,
            Emit(oneSourceRun.OutputCompilation),
            owner,
            "Demo.SchemaReactiveSystem");
        AssertContextCollected(firstContext);

        WeakReference secondContext = ExecuteAndUnloadHotfix(
            stableAssembly,
            Emit(twoSourcesRun.OutputCompilation),
            owner,
            "Demo.SchemaReactiveSystem");
        AssertContextCollected(secondContext);

        WeakReference thirdContext = ExecuteAndUnloadHotfix(
            stableAssembly,
            Emit(oneSourceRun.OutputCompilation),
            owner,
            "Demo.SchemaReactiveSystem");
        AssertContextCollected(thirdContext);

        Assert.Multiple(() =>
        {
            Assert.That(Get(owner, "ZeroValueCalls"), Is.EqualTo(2));
            Assert.That(Get(owner, "CurrentValueCalls"), Is.EqualTo(1));
            Assert.That(Get(owner, "CurrentNumber"), Is.EqualTo(11));
            Assert.That(Get(owner, "CurrentText"), Is.EqualTo("two"));
        });
    }

    [Test]
    public void Generated_hotfix_can_be_collected_and_loaded_again_with_stable_owner_interface()
    {
        StableAssembly stable = CompileStableAssembly();
        const string source = """
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
                public static partial class ReloadableReactiveSystem
                {
                    [ETReactiveSource]
                    private static int Number(this ReactiveOwner self) => self.Number;

                    [ETReactiveBind(nameof(Number))]
                    private static void Refresh(this ReactiveOwner self)
                    {
                        ++self.ZeroValueCalls;
                    }
                }
            }
            """;

        GeneratorRun run = RunGenerator(source, stable.Reference, "Game.ET.Code.HotfixView");
        AssertNoErrors(run);
        byte[] hotfixImage = Emit(run.OutputCompilation);
        Assembly stableAssembly = LoadDefault(stable.Image);
        Type ownerType = stableAssembly.GetType("ET.ReactiveOwner", throwOnError: true)!;
        object owner = Activator.CreateInstance(ownerType)!;

        (WeakReference firstContext, WeakReference secondContext) = ExecuteReloadAndUnloadHotfix(
            stableAssembly,
            hotfixImage,
            owner,
            "Demo.ReloadableReactiveSystem");
        AssertContextCollected(firstContext);
        AssertContextCollected(secondContext);
        Assert.That(Get(owner, "ZeroValueCalls"), Is.EqualTo(2));
    }

    [TestCase(
        "[ETReactiveBind(nameof(Value))] private static int Invalid(this ReactiveOwner self, int value) => value;",
        "ET1109")]
    [TestCase(
        "[ETReactiveBind(\"Missing\")] private static void Missing(this ReactiveOwner self) { }",
        "ET1107")]
    public void Invalid_declarations_report_expected_diagnostic(string declaration, string diagnosticId)
    {
        StableAssembly stable = CompileStableAssembly();
        string source = $$"""
            using ET;

            namespace Demo
            {
                [EntitySystemOf(typeof(ReactiveOwner))]
                [ETReactiveSystem]
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
            using ReactiveBinding;

            namespace ET
            {
                public sealed class VersionedValue : IVersion
                {
                    public int Version => __Version;
                    public int __Version { get; set; }
                    public IVersion __Parent { get; set; }

                    public void IncrementVersion()
                    {
                        __IncrementVersion();
                    }

                    public void __IncrementVersion()
                    {
                        ++__Version;
                        __Parent?.__IncrementVersion();
                    }

                    public void __Reset()
                    {
                        __Version = 0;
                        __Parent = null;
                    }

                    public void Reset()
                    {
                        __Reset();
                    }

                    public override bool Equals(object obj)
                    {
                        return obj is VersionedValue other && other.Version == Version;
                    }

                    public override int GetHashCode()
                    {
                        return Version;
                    }
                }

                public sealed class PlainReference
                {
                    public int Value;
                }

                public struct PlainStruct
                {
                    public int Value;
                }

                public struct EquatableStruct
                {
                    public int Value;

                    public static bool operator ==(EquatableStruct left, EquatableStruct right)
                    {
                        return left.Value == right.Value;
                    }

                    public static bool operator !=(EquatableStruct left, EquatableStruct right)
                    {
                        return !(left == right);
                    }

                    public override bool Equals(object obj)
                    {
                        return obj is EquatableStruct other && this == other;
                    }

                    public override int GetHashCode()
                    {
                        return Value;
                    }
                }

                public sealed class ReactiveOwner : Entity, IETReactiveHost
                {
                    private static long nextInstanceId;

                    public ReactiveOwner()
                    {
                        this.InstanceId = ++nextInstanceId;
                    }

                    public IReactiveObserver ReactiveObserver { get; set; }

                    public int Number;
                    public string Text = string.Empty;
                    public VersionedValue Versioned = new VersionedValue();
                    public PlainReference Reference = new PlainReference();
                    public PlainStruct Plain;
                    public EquatableStruct Equatable;
                    public object Boxed = new object();
                    public float SingleValue;
                    public double DoubleValue;

                    public int ZeroValueCalls;
                    public int CurrentValueCalls;
                    public int OldAndCurrentCalls;
                    public int VersionCalls;
                    public int ObservedVersion;
                    public int SingleCalls;
                    public int DoubleCalls;
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

    private static MetadataReference CompileReference(string assemblyName, string source)
    {
        CSharpCompilation compilation = CreateCompilation(
            assemblyName,
            source,
            Array.Empty<MetadataReference>());
        return MetadataReference.CreateFromImage(Emit(compilation));
    }

    private static GeneratorRun RunGenerator(
        string source,
        MetadataReference stableReference,
        string? assemblyName = null,
        IEnumerable<MetadataReference>? additionalReferences = null)
    {
        CSharpCompilation compilation = CreateCompilation(
            assemblyName ?? $"Game.ET.Code.HotfixView.Tests.{Guid.NewGuid():N}",
            source,
            new[] { stableReference }.Concat(additionalReferences ?? Array.Empty<MetadataReference>()));

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
            FrameworkReferences.Concat(RuntimeReferences).Concat(additionalReferences),
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
        Assembly assembly = context.LoadFromStream(stream);
        InitializeReactiveSystem(assembly);
        return assembly;
    }

    private static void InitializeReactiveSystem(Assembly assembly)
    {
        Type[] observerTypes = assembly.GetTypes()
            .Where(static type => type.GetCustomAttributes(typeof(global::ET.ETReactiveObserverAttribute), false).Length > 0)
            .ToArray();
        if (observerTypes.Length == 0)
        {
            return;
        }

        global::ET.CodeTypes.Instance.SetTypes(observerTypes);
        global::ET.World.Instance.AddSingleton<global::ET.ETReactiveSystem>();
    }

    private static Assembly LoadDefault(byte[] image)
    {
        using var stream = new MemoryStream(image, writable: false);
        return AssemblyLoadContext.Default.LoadFromStream(stream);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference ExecuteAndUnloadHotfix(
        Assembly stableAssembly,
        byte[] hotfixImage,
        object owner,
        string systemTypeName)
    {
        var loadContext = new DynamicAssemblyLoadContext(stableAssembly);
        Assembly hotfixAssembly = Load(loadContext, hotfixImage);
        Type systemType = hotfixAssembly.GetType(systemTypeName, throwOnError: true)!;
        object pooledOwnerA = Activator.CreateInstance(owner.GetType())!;
        object pooledOwnerB = Activator.CreateInstance(owner.GetType())!;
        Invoke(systemType, "ObserveChanges", owner.GetType(), pooledOwnerA);
        Invoke(systemType, "ObserveChanges", owner.GetType(), pooledOwnerB);
        Invoke(systemType, "ClearReactive", owner.GetType(), pooledOwnerA);
        Invoke(systemType, "ClearReactive", owner.GetType(), pooledOwnerB);
        Invoke(systemType, "ObserveChanges", owner.GetType(), owner);
        Invoke(systemType, "ClearReactive", owner.GetType(), owner);
        PropertyInfo observerProperty = owner.GetType().GetProperty("ReactiveObserver")!;
        Assert.That(observerProperty.GetValue(owner), Is.Null,
            "ClearReactive 必须释放稳定 owner 持有的生成 Observer");
        global::ET.World.Instance.Dispose();
        loadContext.Unload();

        return new WeakReference(loadContext);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference First, WeakReference Second) ExecuteReloadAndUnloadHotfix(
        Assembly stableAssembly,
        byte[] hotfixImage,
        object owner,
        string systemTypeName)
    {
        Type ownerType = owner.GetType();
        PropertyInfo observerProperty = ownerType.GetProperty("ReactiveObserver")!;

        var firstLoadContext = new DynamicAssemblyLoadContext(stableAssembly);
        Assembly firstHotfixAssembly = Load(firstLoadContext, hotfixImage);
        Type firstSystemType = firstHotfixAssembly.GetType(systemTypeName, throwOnError: true)!;
        Invoke(firstSystemType, "ObserveChanges", ownerType, owner);
        var firstObserver = (global::ET.IETReactiveObserver)observerProperty.GetValue(owner)!;
        int firstDllVersion = firstObserver.DllVersion;

        var secondLoadContext = new DynamicAssemblyLoadContext(stableAssembly);
        Assembly secondHotfixAssembly = Load(secondLoadContext, hotfixImage);
        Type secondSystemType = secondHotfixAssembly.GetType(systemTypeName, throwOnError: true)!;
        Invoke(secondSystemType, "ObserveChanges", ownerType, owner);
        var secondObserver = (global::ET.IETReactiveObserver)observerProperty.GetValue(owner)!;

        Assert.Multiple(() =>
        {
            Assert.That(secondObserver, Is.Not.SameAs(firstObserver));
            Assert.That(secondObserver.DllVersion, Is.GreaterThan(firstDllVersion));
            Assert.That(secondObserver.GetType().Assembly, Is.SameAs(secondHotfixAssembly),
                "版本不一致时必须按 ETReactiveSystem 中登记的当前 Type 创建 observer");
        });

        Invoke(secondSystemType, "ClearReactive", ownerType, owner);
        global::ET.World.Instance.Dispose();
        firstLoadContext.Unload();
        secondLoadContext.Unload();

        return (new WeakReference(firstLoadContext), new WeakReference(secondLoadContext));
    }

    private static void AssertContextCollected(WeakReference contextReference)
    {
        for (int attempt = 0; contextReference.IsAlive && attempt < 10; ++attempt)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        Assert.That(contextReference.IsAlive, Is.False,
            "Hotfix AssemblyLoadContext 仍被稳定 reactive 状态或生成代码引用");
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

    private sealed class RuntimeReactiveEntity: global::ET.Entity, global::ET.IETReactiveHost
    {
        public RuntimeReactiveEntity(long instanceId)
        {
            this.InstanceId = instanceId;
        }

        public IReactiveObserver ReactiveObserver { get; set; } = null!;

        public void Reuse(long instanceId)
        {
            this.InstanceId = instanceId;
        }
    }

    [global::ET.ETReactiveObserver(typeof(RuntimeReactiveEntity))]
    private sealed partial class RuntimeReactiveObserver: global::ET.IETReactiveObserver
    {
        public int DllVersion { get; private set; }

        public long OwnerInstanceId { get; private set; }

        public int RecycleCount { get; private set; }

        public void Initialize(global::ET.IETReactiveHost host, int dllVersion)
        {
            this.DllVersion = dllVersion;
            this.OwnerInstanceId = ((RuntimeReactiveEntity)host).InstanceId;
        }

        public void Recycle()
        {
            ++this.RecycleCount;
            this.DllVersion = 0;
            this.OwnerInstanceId = 0;
        }

    }

    private sealed class DynamicAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly Assembly? stableAssembly;

        public DynamicAssemblyLoadContext(Assembly? stableAssembly = null)
            : base(isCollectible: true)
        {
            this.stableAssembly = stableAssembly;
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (this.stableAssembly != null &&
                string.Equals(this.stableAssembly.GetName().Name, assemblyName.Name, StringComparison.Ordinal))
            {
                return this.stableAssembly;
            }

            return null;
        }
    }
}
