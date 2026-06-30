using System.Linq;
using Microsoft.CodeAnalysis;
using ViciOne.Core.Runtime.Analyzers.Constants;

namespace ViciOne.Core.Runtime.Analyzers.Extensions;

internal static class TypeSymbolExtensions
{
    extension(ITypeSymbol propertyType)
    {
        public ITypeSymbol? GetFunctionBlockComponentValueType(Compilation compilation)
            => GetConnectorValueType(propertyType, compilation) ??
               GetPersistentVariableValueType(propertyType, compilation) ??
               GetSettingValueType(propertyType, compilation);

        public ITypeSymbol? GetConnectorValueType(Compilation compilation)
            => GetRuntimeElementValueType(propertyType, TypeNames.IRuntimeConnector, compilation);

        public ITypeSymbol? GetPersistentVariableValueType(Compilation compilation)
            => GetRuntimeElementValueType(propertyType, TypeNames.IRuntimePersistentVariable, compilation);

        public ITypeSymbol? GetSettingValueType(Compilation compilation)
            => GetRuntimeElementValueType(propertyType, TypeNames.IRuntimeSetting, compilation);
    }

    private static ITypeSymbol? GetRuntimeElementValueType(ITypeSymbol runtimeElementPropertyType, string runtimeElementInterfaceTypeName, Compilation compilation)
    {
        INamedTypeSymbol? runtimeElementInterfaceType = compilation.GetTypeByMetadataName(runtimeElementInterfaceTypeName);
        INamedTypeSymbol? interfaceType;
        if (SymbolEqualityComparer.Default.Equals(runtimeElementPropertyType.OriginalDefinition, runtimeElementInterfaceType))
            interfaceType = runtimeElementPropertyType as INamedTypeSymbol;
        else
            interfaceType = runtimeElementPropertyType.AllInterfaces.FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.OriginalDefinition, runtimeElementInterfaceType));

        return interfaceType?.TypeArguments.FirstOrDefault();
    }
}
