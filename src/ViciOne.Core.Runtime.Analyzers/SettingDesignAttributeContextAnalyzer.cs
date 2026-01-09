using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using ViciOne.Core.Runtime.Analyzers.Constants;

namespace ViciOne.Core.Runtime.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SettingDesignAttributeContextAnalyzer : FunctionBlockComponentDesignAttributeContextAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule = new(RuleIdentifiers.WrongSettingDesignAttributeContext,
        "SettingDesign attribute must be used in correct context",
        "Property '{0}' has SettingDesignAttribute assigned but it's parent class has no SettingDesignsAttribute assigned. Please use attribute within correct context.",
        RuleCategories.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private protected override string ComponentDesignAttributeTypeName => TypeNames.SettingDesignAttribute;
    private protected override string ExpectedParentAttributeTypeName => TypeNames.SettingDesignsAttribute;
    internal static DiagnosticDescriptor Rule => s_rule;

    private protected override DiagnosticDescriptor GetRule()
        => s_rule;
}
