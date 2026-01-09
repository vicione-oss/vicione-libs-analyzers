namespace ViciOne.Core.Runtime.Analyzers.Constants;

internal static class TypeNames
{
    public const string DefaultValueAttribute = $"{Namespaces.Attributes}.DefaultValueAttribute";
    public const string FunctionBlockDesignAttribute = $"{Namespaces.Attributes}.FunctionBlockDesignAttribute";
    public const string InputConnectorDesignAttribute = $"{Namespaces.Attributes}.InputConnectorDesignAttribute";
    public const string InputConnectorDesignsAttribute = $"{Namespaces.Attributes}.InputConnectorDesignsAttribute";
    public const string OutputConnectorDesignAttribute = $"{Namespaces.Attributes}.OutputConnectorDesignAttribute";
    public const string OutputConnectorDesignsAttribute = $"{Namespaces.Attributes}.OutputConnectorDesignsAttribute";
    public const string PersistentVariableDesignAttribute = $"{Namespaces.Attributes}.PersistentVariableDesignAttribute";
    public const string PersistentVariableDesignsAttribute = $"{Namespaces.Attributes}.PersistentVariableDesignsAttribute";
    public const string SettingDesignAttribute = $"{Namespaces.Attributes}.SettingDesignAttribute";
    public const string SettingDesignsAttribute = $"{Namespaces.Attributes}.SettingDesignsAttribute";

    public const string IRuntimeConnector = $"{Namespaces.RuntimeContracts}.IRuntimeConnector`1";
    public const string IRuntimePersistentVariable = $"{Namespaces.RuntimeContracts}.IRuntimePersistentVariable`1";
    public const string IRuntimeSetting = $"{Namespaces.RuntimeContracts}.IRuntimeSetting`1";

    public const string RuntimeFunctionBlock = $"{Namespaces.RuntimeElements}.RuntimeFunctionBlock`1";

    public const string RangeValidatorAttribute = $"{Namespaces.Validators}.RangeValidatorAttribute`1";
    public const string EnumValidatorAttribute = $"{Namespaces.Validators}.EnumValidatorAttribute`1";
    public const string EqualityValidatorAttribute = $"{Namespaces.Validators}.EqualityValidatorAttribute`1";
    public const string InequalityValidatorAttribute = $"{Namespaces.Validators}.InequalityValidatorAttribute`1";
}
