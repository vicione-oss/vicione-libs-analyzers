using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using ViciOne.Core.Runtime.Analyzers.Extensions;

namespace ViciOne.Core.Runtime.Analyzers;

public abstract class FunctionBlockComponentDesignAttributeContextAnalyzer : DiagnosticAnalyzer
{
    private protected abstract string ComponentDesignAttributeTypeName { get; }
    private protected abstract string ExpectedParentAttributeTypeName { get; }
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [GetRule()];

    private protected abstract DiagnosticDescriptor GetRule();

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
    }

    private void AnalyzeProperty(SymbolAnalysisContext context)
    {
        var propertySymbol = (IPropertySymbol)context.Symbol;
        if (propertySymbol.HasFunctionBlockComponentDesignParentTypeStructure(out INamedTypeSymbol? parentType, out INamedTypeSymbol? parentsParentType) == false ||
            parentsParentType!.HasFunctionBlockDesignAttribute(context.Compilation) == false)
            return;

        INamedTypeSymbol? componentDesignAttributeType = context.Compilation.GetTypeByMetadataName(ComponentDesignAttributeTypeName);
        if (componentDesignAttributeType is null)
            return;

        AttributeData? componentDesignAttribute = propertySymbol.GetAttributesOfType(componentDesignAttributeType).FirstOrDefault();
        if (componentDesignAttribute is null)
            return;

        if (parentType!.HasAttribute(context.Compilation, ExpectedParentAttributeTypeName) == false)
            context.ReportDiagnostic(Diagnostic.Create(GetRule(), componentDesignAttribute.GetLocation() ?? propertySymbol.Locations[0], propertySymbol.Name));
    }
}
