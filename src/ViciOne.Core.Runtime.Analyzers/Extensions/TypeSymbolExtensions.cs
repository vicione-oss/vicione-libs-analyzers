using System.Linq;
using Microsoft.CodeAnalysis;
using ViciOne.Core.Runtime.Analyzers.Constants;

namespace ViciOne.Core.Runtime.Analyzers.Extensions;

internal static class TypeSymbolExtensions
{
    public static ITypeSymbol? GetFunctionBlockComponentValueType(this ITypeSymbol connectorPropertyType, Compilation compilation)
        => GetConnectorValueType(connectorPropertyType, compilation) ??
           GetPersistentVariableValueType(connectorPropertyType, compilation) ??
           GetSettingValueType(connectorPropertyType, compilation);

    public static ITypeSymbol? GetConnectorValueType(this ITypeSymbol connectorPropertyType, Compilation compilation)
        => GetRuntimeElementValueType(connectorPropertyType, TypeNames.IRuntimeConnector, compilation);

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

    public static ITypeSymbol? GetPersistentVariableValueType(this ITypeSymbol settingPropertyType, Compilation compilation)
        => GetRuntimeElementValueType(settingPropertyType, TypeNames.IRuntimePersistentVariable, compilation);

    public static ITypeSymbol? GetSettingValueType(this ITypeSymbol settingPropertyType, Compilation compilation)
        => GetRuntimeElementValueType(settingPropertyType, TypeNames.IRuntimeSetting, compilation);
}
