using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using ViciOne.Core.Runtime.Analyzers.Constants;

namespace ViciOne.Core.Runtime.Analyzers.Extensions;

internal static class SymbolExtensions
{
    public static IEnumerable<AttributeData> GetAttributesOfType(this ISymbol symbol, INamedTypeSymbol attributeType)
        => symbol.GetAttributes().Where(x =>
            x.AttributeClass is not null &&
                (SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeType) ||
                (x.AttributeClass.IsGenericType && SymbolEqualityComparer.Default.Equals(x.AttributeClass.ConstructedFrom, attributeType))));

    public static IEnumerable<AttributeData> GetAttributesWithClassName(this ISymbol symbol, string className)
        => symbol.GetAttributes().Where(x => (x.AttributeClass?.Name ?? string.Empty).Equals(className, StringComparison.Ordinal));

    public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol? attributeType)
        => attributeType is not null && symbol.GetAttributesOfType(attributeType).Any();

    public static bool HasAttribute(this ISymbol symbol, Compilation compilation, string attributeTypeName)
        => symbol.HasAttribute(compilation.GetTypeByMetadataName(attributeTypeName));

    public static bool HasFunctionBlockComponentDesignAttribute(this ISymbol symbol, Compilation compilation)
        => symbol.HasAttribute(compilation, TypeNames.InputConnectorDesignAttribute) ||
           symbol.HasAttribute(compilation, TypeNames.OutputConnectorDesignAttribute) ||
           symbol.HasAttribute(compilation, TypeNames.PersistentVariableDesignAttribute) ||
           symbol.HasAttribute(compilation, TypeNames.SettingDesignAttribute);

    public static bool HasFunctionBlockComponentDesignsAttribute(this ISymbol symbol, Compilation compilation)
        => symbol.HasAttribute(compilation, TypeNames.InputConnectorDesignsAttribute) ||
           symbol.HasAttribute(compilation, TypeNames.OutputConnectorDesignsAttribute) ||
           symbol.HasAttribute(compilation, TypeNames.PersistentVariableDesignsAttribute) ||
           symbol.HasAttribute(compilation, TypeNames.SettingDesignsAttribute);

    public static bool HasFunctionBlockDesignAttribute(this ISymbol symbol, Compilation compilation)
        => symbol.HasAttribute(compilation, TypeNames.FunctionBlockDesignAttribute);

    public static bool IsFunctionBlockComponentDesign(this ISymbol symbol, Compilation compilation)
    {
        if (HasFunctionBlockComponentDesignParentTypeStructure(symbol, out INamedTypeSymbol? parentType, out INamedTypeSymbol? parentsParentType) == false)
            return false;

        return
            symbol.HasFunctionBlockComponentDesignAttribute(compilation) &&
            parentType!.HasFunctionBlockComponentDesignsAttribute(compilation) &&
            parentsParentType!.HasFunctionBlockDesignAttribute(compilation);
    }

    public static bool HasFunctionBlockComponentDesignParentTypeStructure(this ISymbol symbol,
        out INamedTypeSymbol? parentType, out INamedTypeSymbol? parentsParentType)
    {
        parentType = symbol.ContainingType;
        parentsParentType = parentType?.ContainingType;

        if (parentType is null ||
            parentsParentType is null ||
            parentType.TypeKind != TypeKind.Class ||
            parentsParentType.TypeKind != TypeKind.Class)
            return false;

        return true;
    }
}
