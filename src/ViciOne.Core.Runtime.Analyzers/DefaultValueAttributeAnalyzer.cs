using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using ViciOne.Core.Runtime.Analyzers.Constants;
using ViciOne.Core.Runtime.Analyzers.Extensions;

namespace ViciOne.Core.Runtime.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DefaultValueAttributeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule = new(RuleIdentifiers.WrongDefaultValueAttribute,
        "Correct DefaultValue attribute must be used",
        "Property '{0}' has wrong DefaultValueAttribute assigned. Please use attribute within namespace '{1}'.",
        RuleCategories.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true);

    internal static DiagnosticDescriptor Rule => s_rule;
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(s_rule);

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

        var defaultValueAttributes = propertySymbol.GetAttributesWithClassName(correctDefaultValueAttributeType.Name);
        foreach (var defaultValueAttribute in defaultValueAttributes)
            if (SymbolEqualityComparer.Default.Equals(defaultValueAttribute.AttributeClass, correctDefaultValueAttributeType) == false)
                context.ReportDiagnostic(Diagnostic.Create(s_rule, defaultValueAttribute.GetLocation() ?? propertySymbol.Locations[0], propertySymbol.Name, Namespaces.Attributes));
    }
}
