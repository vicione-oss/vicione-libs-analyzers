using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using ViciOne.Core.Runtime.Analyzers.Constants;
using ViciOne.Core.Runtime.Analyzers.Extensions;

namespace ViciOne.Core.Runtime.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FunctionBlockDesignBaseTypeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule = new(RuleIdentifiers.WrongFunctionBlockDesignBaseType,
        "FunctionBlocks must be derived from correct base class",
        "Class '{0}' has FunctionBlockDesignAttribute assigned but is not derived from '{1}'",
        RuleCategories.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true);

    internal static DiagnosticDescriptor Rule => s_rule;
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        var type = (INamedTypeSymbol)context.Symbol;
        if (type.TypeKind != TypeKind.Class ||
            SymbolExtensions.HasFunctionBlockDesignAttribute(type, context.Compilation) == false ||
            IsDerivedFromRuntimeFunctionBlock(type, context.Compilation))
            return;

        context.ReportDiagnostic(Diagnostic.Create(s_rule, type.Locations[0], type.Name, TypeNames.RuntimeFunctionBlock));
    }

    private static bool IsDerivedFromRuntimeFunctionBlock(ITypeSymbol type, Compilation compilation)
    {
        var runtimeFunctionBlockType = compilation.GetTypeByMetadataName(TypeNames.RuntimeFunctionBlock);
        var baseType = type.BaseType;
        while (baseType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(baseType.OriginalDefinition, runtimeFunctionBlockType))
                return true;
            baseType = baseType.BaseType;
        }
        return false;
    }
}
