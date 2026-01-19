using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using ViciOne.Core.Runtime.Analyzers.Constants;
using ViciOne.Core.Runtime.Analyzers.Extensions;

namespace ViciOne.Core.Runtime.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ValidatorValueTypeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule = new(RuleIdentifiers.WrongValidatorValueType,
        "Validator attribute must be used with correct value type argument",
        "Property '{0}' has {1} with type '{2}' as type argument assigned but was expected to have type '{3}' as type argument. Type argument needs to be changed to '{3}'.",
        RuleCategories.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true);
    private static readonly string[] s_validatorAttributeNamesWithTypeArgument =
    [
        TypeNames.EnumValidatorAttribute,
        TypeNames.EqualityValidatorAttribute,
        TypeNames.InequalityValidatorAttribute,
        TypeNames.RangeValidatorAttribute,
    ];

    internal static DiagnosticDescriptor Rule => s_rule;
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
    }

    private void AnalyzeProperty(SymbolAnalysisContext context)
    {
        var propertySymbol = (IPropertySymbol)context.Symbol;
        if (propertySymbol.IsFunctionBlockComponentDesign(context.Compilation) == false)
            return;

        var runtimeElementValueType = propertySymbol.Type.GetFunctionBlockComponentValueType(context.Compilation);
        if (runtimeElementValueType is null)
            return;

        foreach (var attributeTypeName in s_validatorAttributeNamesWithTypeArgument)
        {
            var attributeType = context.Compilation.GetTypeByMetadataName(attributeTypeName);
            if (attributeType is null)
                continue;

            var attributes = propertySymbol.GetAttributesOfType(attributeType);
            foreach (var attribute in attributes)
            {
                if (attribute.AttributeClass is null)
                    continue;
                var attributeValueType = attribute.AttributeClass.TypeArguments[0];
                if (SymbolEqualityComparer.Default.Equals(attributeValueType, runtimeElementValueType) == false)
                    context.ReportDiagnostic(Diagnostic.Create(s_rule, attribute.GetTypeArgumentLocation(0) ?? propertySymbol.Locations[0], propertySymbol.Name, attributeType.Name, attributeValueType, runtimeElementValueType));
            }
        }
    }
}
