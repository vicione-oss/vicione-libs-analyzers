using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using ViciOne.Core.Runtime.Analyzers.Constants;
using ViciOne.Core.Runtime.Analyzers.Extensions;

namespace ViciOne.Core.Runtime.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DefaultValueValueTypeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule = new(RuleIdentifiers.WrongDefaultValueValueType,
        "DefaultValue attribute must provide value of correct type",
        "Property '{0}' has DefaultValueAttribute with value of type '{1}' assigned but was expected to have value of type '{2}'. Type of value needs to be changed to '{2}'.",
        RuleCategories.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true);

    internal static DiagnosticDescriptor Rule => s_rule;
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
    }

    private static void AnalyzeProperty(SymbolAnalysisContext context)
    {
        var propertySymbol = (IPropertySymbol)context.Symbol;
        if (propertySymbol.IsFunctionBlockComponentDesign(context.Compilation) == false)
            return;

        var correctDefaultValueAttributeType = context.Compilation.GetTypeByMetadataName(TypeNames.DefaultValueAttribute);
        if (correctDefaultValueAttributeType is null)
            return;

        var correctDefaultValueAttributes = propertySymbol.GetAttributesOfType(correctDefaultValueAttributeType);
        if (correctDefaultValueAttributes.Any() == false)
            return;

        var runtimeElementValueType = propertySymbol.Type.GetFunctionBlockComponentValueType(context.Compilation);
        if (runtimeElementValueType is null)
            return;

        foreach (var correctDefaultValueAttribute in correctDefaultValueAttributes)
        {
            var attributeValueType = correctDefaultValueAttribute.ConstructorArguments[0].Type;
            if (SymbolEqualityComparer.Default.Equals(attributeValueType, runtimeElementValueType) == false)
                context.ReportDiagnostic(Diagnostic.Create(s_rule, correctDefaultValueAttribute.GetArgumentLocation(0) ?? propertySymbol.Locations[0], propertySymbol.Name, attributeValueType, runtimeElementValueType));
        }
    }
}
