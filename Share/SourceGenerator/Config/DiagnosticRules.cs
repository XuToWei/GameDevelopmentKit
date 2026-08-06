using Microsoft.CodeAnalysis;

namespace ET.Generator;


public static class ETSystemMethodIsInStaticPartialClassRule
{
    private const string Title = "ETSystem函数必须声明在静态分部类中";

    private const string MessageFormat = "ETSystem函数所在的类:{0} 不是静态分部类";

    private const string Description = "ETSystem函数必须声明在静态类中.";

    public static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticIds.ETSystemMethodIsInStaticPartialClassRuleId,
                Title,
                MessageFormat,
                DiagnosticCategories.Generator,
                DiagnosticSeverity.Error,
                true,
                Description);
}

public static class ETReactiveDiagnosticRules
{
    public static readonly DiagnosticDescriptor SystemDeclaration = Create(
        DiagnosticIds.ETReactiveSystemDeclarationRuleId,
        "ET Reactive System 声明无效",
        "ET Reactive System '{0}' 必须是顶层、非泛型的 static partial class");

    public static readonly DiagnosticDescriptor Owner = Create(
        DiagnosticIds.ETReactiveOwnerRuleId,
        "ET Reactive owner 无效",
        "ET Reactive System '{0}' 必须与合法的 EntitySystemOfAttribute 声明在同一个类上");

    public static readonly DiagnosticDescriptor OwnerInterface = Create(
        DiagnosticIds.ETReactiveOwnerInterfaceRuleId,
        "ET Reactive owner 接口无效",
        "owner '{0}' 必须继承 Entity 并实现 IETReactive，以保存生成的响应式缓存字段");

    public static readonly DiagnosticDescriptor HostDeclaration = Create(
        DiagnosticIds.ETReactiveHostDeclarationRuleId,
        "ET Reactive host 声明无效",
        "IETReactive '{0}' 及其外层类型必须声明为 partial；响应式缓存字段由源码生成器生成");

    public static readonly DiagnosticDescriptor SourceSignature = Create(
        DiagnosticIds.ETReactiveSourceSignatureRuleId,
        "ET Reactive source 签名无效",
        "Reactive source '{0}' 必须是 Entity 的 public 实例字段、可读非索引属性，或无参非泛型且有返回值的方法");

    public static readonly DiagnosticDescriptor DuplicateSource = Create(
        DiagnosticIds.ETReactiveDuplicateSourceRuleId,
        "ET Reactive source ID 重复",
        "Reactive source ID '{0}' 在 owner '{1}' 中重复");

    public static readonly DiagnosticDescriptor MissingSource = Create(
        DiagnosticIds.ETReactiveMissingSourceRuleId,
        "ET Reactive bind 引用了无效 source",
        "Reactive bind '{0}' 必须显式声明 source ID；以下 ID 不存在或为空: {1}");

    public static readonly DiagnosticDescriptor DuplicateBindSource = Create(
        DiagnosticIds.ETReactiveDuplicateBindSourceRuleId,
        "ET Reactive bind source ID 重复",
        "Reactive bind '{0}' 包含重复的 source ID: {1}");

    public static readonly DiagnosticDescriptor BindNameof = Create(
        DiagnosticIds.ETReactiveBindNameofRuleId,
        "ET Reactive bind 必须使用 nameof",
        "Reactive bind '{0}' 的 source ID 必须直接使用 nameof(...)，不能使用字符串字面量或字符串常量");

    public static readonly DiagnosticDescriptor BindSignature = Create(
        DiagnosticIds.ETReactiveBindSignatureRuleId,
        "ET Reactive bind 签名无效",
        "Reactive bind '{0}' 必须是 static void，且 owner 后的参数必须匹配 source: {1}");

    public static readonly DiagnosticDescriptor GeneratedMethodCollision = Create(
        DiagnosticIds.ETReactiveGeneratedMethodCollisionRuleId,
        "ET Reactive 生成方法冲突",
        "System '{0}' 已声明会与生成方法 '{1}({2})' 冲突的成员");

    public static readonly DiagnosticDescriptor DuplicateOwner = Create(
        DiagnosticIds.ETReactiveDuplicateOwnerRuleId,
        "ET Reactive owner 重复",
        "owner '{0}' 已由另一个 ET Reactive System 使用；每个 owner 只能声明一个 Reactive System，以保持 ObserveChanges 扩展调用唯一");

    public static readonly DiagnosticDescriptor UnsupportedSourceType = Create(
        DiagnosticIds.ETReactiveUnsupportedSourceTypeRuleId,
        "ET Reactive source 类型不受支持",
        "Reactive source '{0}' 的成员类型 '{1}' 必须是值类型、string 或 ReactiveBinding.IVersion；object、dynamic 和普通引用类型不受支持");

    public static readonly DiagnosticDescriptor StructEquality = Create(
        DiagnosticIds.ETReactiveStructEqualityRuleId,
        "ET Reactive source 结构体缺少相等运算符",
        "Reactive source '{0}' 的结构体类型 '{1}' 必须声明 ==/!= 运算符");

    public static readonly DiagnosticDescriptor UnusedSource = Create(
        DiagnosticIds.ETReactiveUnusedSourceRuleId,
        "ET Reactive source 未使用",
        "Reactive source '{0}' 没有被任何 ETReactiveBind 引用，将不会在 ObserveChanges 中求值",
        DiagnosticSeverity.Warning);

    private static DiagnosticDescriptor Create(
        string id,
        string title,
        string messageFormat,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        return new DiagnosticDescriptor(
            id,
            title,
            messageFormat,
            DiagnosticCategories.Generator,
            severity,
            true);
    }
}

