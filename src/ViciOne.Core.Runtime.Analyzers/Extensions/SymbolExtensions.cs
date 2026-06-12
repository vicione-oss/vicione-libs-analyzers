using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using ViciOne.Core.Runtime.Analyzers.Constants;

namespace ViciOne.Core.Runtime.Analyzers.Extensions;

internal static class SymbolExtensions
{
    extension(ISymbol symbol)
    {
        public IEnumerable<AttributeData> GetAttributesOfType(INamedTypeSymbol attributeType)
            => symbol.GetAttributes().Where(x =>
                x.AttributeClass is not null &&
                    (SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeType) ||
                    (x.AttributeClass.IsGenericType && SymbolEqualityComparer.Default.Equals(x.AttributeClass.ConstructedFrom, attributeType))));

        public IEnumerable<AttributeData> GetAttributesWithClassName(string className)
            => symbol.GetAttributes().Where(x => (x.AttributeClass?.Name ?? string.Empty).Equals(className, StringComparison.Ordinal));

        public bool HasAttribute(INamedTypeSymbol? attributeType)
            => attributeType is not null && symbol.GetAttributesOfType(attributeType).Any();

        public bool HasAttribute(Compilation compilation, string attributeTypeName)
            => symbol.HasAttribute(compilation.GetTypeByMetadataName(attributeTypeName));

        public bool HasFunctionBlockComponentDesignAttribute(Compilation compilation)
            => symbol.HasAttribute(compilation, TypeNames.InputConnectorDesignAttribute) ||
               symbol.HasAttribute(compilation, TypeNames.OutputConnectorDesignAttribute) ||
               symbol.HasAttribute(compilation, TypeNames.PersistentVariableDesignAttribute) ||
               symbol.HasAttribute(compilation, TypeNames.SettingDesignAttribute);

        public bool HasFunctionBlockComponentDesignsAttribute(Compilation compilation)
            => symbol.HasAttribute(compilation, TypeNames.InputConnectorDesignsAttribute) ||
               symbol.HasAttribute(compilation, TypeNames.OutputConnectorDesignsAttribute) ||
               symbol.HasAttribute(compilation, TypeNames.PersistentVariableDesignsAttribute) ||
               symbol.HasAttribute(compilation, TypeNames.SettingDesignsAttribute);

        public bool HasFunctionBlockDesignAttribute(Compilation compilation)
            => symbol.HasAttribute(compilation, TypeNames.FunctionBlockDesignAttribute);

        public bool IsFunctionBlockComponentDesign(Compilation compilation)
        {
            if (HasFunctionBlockComponentDesignParentTypeStructure(symbol, out INamedTypeSymbol? parentType, out INamedTypeSymbol? parentsParentType) == false)
                return false;

            return
                symbol.HasFunctionBlockComponentDesignAttribute(compilation) &&
                parentType!.HasFunctionBlockComponentDesignsAttribute(compilation) &&
                parentsParentType!.HasFunctionBlockDesignAttribute(compilation);
        }

        public bool HasFunctionBlockComponentDesignParentTypeStructure(out INamedTypeSymbol? parentType, out INamedTypeSymbol? parentsParentType)
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
}
