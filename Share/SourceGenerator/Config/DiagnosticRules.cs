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
        "ET Reactive System '{0}' 的 owner 类型或状态成员名无效");

    public static readonly DiagnosticDescriptor StateMember = Create(
        DiagnosticIds.ETReactiveStateMemberRuleId,
        "ET Reactive 状态成员无效",
        "owner '{0}' 上的成员 '{1}' 必须是当前 System 可读的 ETReactiveState 实例字段或属性");

    public static readonly DiagnosticDescriptor SourceSignature = Create(
        DiagnosticIds.ETReactiveSourceSignatureRuleId,
        "ET Reactive source 签名无效",
        "Reactive source '{0}' 必须是 static、非泛型、返回可存入 ETReactiveSlot<T> 的非 void 值，并且只接收一个 owner 参数的方法");

    public static readonly DiagnosticDescriptor DuplicateSource = Create(
        DiagnosticIds.ETReactiveDuplicateSourceRuleId,
        "ET Reactive source ID 重复",
        "Reactive source ID '{0}' 在 System '{1}' 中重复");

    public static readonly DiagnosticDescriptor HotfixSourceType = Create(
        DiagnosticIds.ETReactiveHotfixSourceTypeRuleId,
        "ET Reactive source 不能返回 Hotfix 类型",
        "Reactive source '{0}' 的返回类型 '{1}' 包含当前 Hotfix 程序集声明的类型，不能存入稳定状态");

    public static readonly DiagnosticDescriptor MissingSource = Create(
        DiagnosticIds.ETReactiveMissingSourceRuleId,
        "ET Reactive bind 引用了无效 source",
        "Reactive bind '{0}' 必须显式声明 source ID；以下 ID 不存在或为空: {1}");

    public static readonly DiagnosticDescriptor DuplicateBindSource = Create(
        DiagnosticIds.ETReactiveDuplicateBindSourceRuleId,
        "ET Reactive bind source ID 重复",
        "Reactive bind '{0}' 包含重复的 source ID: {1}");

    public static readonly DiagnosticDescriptor BindSignature = Create(
        DiagnosticIds.ETReactiveBindSignatureRuleId,
        "ET Reactive bind 签名无效",
        "Reactive bind '{0}' 必须是 static void，且 owner 后的参数必须匹配 source: {1}");

    public static readonly DiagnosticDescriptor GeneratedMethodCollision = Create(
        DiagnosticIds.ETReactiveGeneratedMethodCollisionRuleId,
        "ET Reactive 生成方法冲突",
        "System '{0}' 已声明会与生成方法 '{1}({2})' 冲突的成员");

    private static DiagnosticDescriptor Create(string id, string title, string messageFormat)
    {
        return new DiagnosticDescriptor(
            id,
            title,
            messageFormat,
            DiagnosticCategories.Generator,
            DiagnosticSeverity.Error,
            true);
    }
}

