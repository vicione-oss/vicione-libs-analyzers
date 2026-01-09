using System.Threading.Tasks;
using ViciOne.Core.Runtime.Analyzers;
using Xunit;
using Verify = Verifiers.CSharpAnalyzerVerifier<ViciOne.Core.Runtime.Analyzers.ValidatorValueTypeAnalyzer>;

public class ValidatorValueTypeAnalyzerTests
{
    [Fact]
    public async Task DoesNotReportAnythingIfAttributeIsUsedCorrectly()
    {
        var code =
@"using ViciOne.Core.Contracts.Runtime;
using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;
using ViciOne.Engine.DefaultValidators;

[FunctionBlockDesign(""dcdb34a4-1602-42cb-9449-caaaaa30b190"", ""2795ad3c-eeda-46f4-a23b-fe22c463c1fd"", ""e6259ec1-21ad-4d64-bf43-0d897064e309"")]
public class Block : RuntimeFunctionBlock<Block>
{
    [InputConnectorDesigns]
    public class Inputs
    {
        [InputConnectorDesign(""899a6e54-ba16-442f-9580-4b5940ed76f1"")]
        public IRuntimeConnector<int> Input { get; set; }
    }

    [OutputConnectorDesigns]
    public class Outputs
    {
        [OutputConnectorDesign(""103ac877-299e-4980-b0e9-63df8cf2eb8f"")]
        [EnumValidator<Side>(Side.Top)]
        public IRuntimeConnector<Side> Output { get; set; }
    }

    [SettingDesigns]
    public class Settings
    {
        [SettingDesign(""67cecd95-bcb1-487e-ad30-00b25119dbfe"", ""8eb025f2-fab5-4716-9029-de7ab4168906"", ""7facec42-6d44-42bb-9f92-a25072d68c06"")]
        [RangeValidator<int>(10, 20)]
        public IRuntimeSetting<int> Setting { get; set; }
    }

    [PersistentVariableDesigns]
    public class PersistentVariables
    {
        [PersistentVariableDesign(""cdf197cb-6def-43ae-a066-5a8d2947bf30"")]
        [RangeValidator<float>(42.0f, 666.0f)]
        public IRuntimePersistentVariable<float> Variable { get; set; }
    }
}

public enum Side { Top, Bottom }";


        await Verify.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReportsDiagnosticIfWrongValueTypeIsUsedOnInput()
    {
        var code =
@"using ViciOne.Core.Contracts.Runtime;
using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;
using ViciOne.Engine.DefaultValidators;

[FunctionBlockDesign(""1a51a929-37fb-43ee-bbca-466cc159601f"", ""63eb35bc-5e8c-4a33-ad0f-407d10f53035"", ""e5e9c583-38ed-4304-b523-2cea1b242ae9"")]
public class Block : RuntimeFunctionBlock<Block>
{
    [InputConnectorDesigns]
    public class Inputs
    {
        [InputConnectorDesign(""0bc4ff63-bc84-4b4c-9492-7536c52a3e09"")]
        [EnumValidator<Side>(Side.Top)]
        public IRuntimeConnector<int> Input1 { get; set; }

        [InputConnectorDesign(""d8f2dc8f-8497-4e2b-8f23-bf494d36c7e4"")]
        [RangeValidator<int>(10, 20)]
        public IRuntimeConnector<double> Input2 { get; set; }

        [InputConnectorDesign(""8190f4ea-500f-4bb7-88e3-b41e468da55a"")]
        [EqualityValidator<double>(42.0, 666.0)]
        public IRuntimeConnector<int> Input3 { get; set; }

        [InputConnectorDesign(""8a137d0b-1a26-41fa-af31-324e5cbe48ba"")]
        [InequalityValidator<float>(42.0f, 666.0f)]
        public IRuntimeConnector<double> Input4 { get; set; }
    }
}

public enum Side { Top, Bottom }";

        var expected = new[]
        {
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Input1", "EnumValidatorAttribute", "Side", "int")
                .WithLocation(13, 24),
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Input2", "RangeValidatorAttribute", "int", "double")
                .WithLocation(17, 25),
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Input3", "EqualityValidatorAttribute", "double", "int")
                .WithLocation(21, 28),
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Input4", "InequalityValidatorAttribute", "float", "double")
                .WithLocation(25, 30),
        };

        await Verify.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task ReportsDiagnosticIfWrongValueTypeIsUsedOnOutput()
    {
        var code =
@"using ViciOne.Core.Contracts.Runtime;
using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;
using ViciOne.Engine.DefaultValidators;

[FunctionBlockDesign(""adf73273-4ffa-4a72-b802-0f923b291284"", ""96ce03ed-9b4a-4ded-bd19-1f7f3f83432b"", ""5de5f8b7-9bf7-43dc-ab81-09db98d03f7e"")]
public class Block : RuntimeFunctionBlock<Block>
{
    [OutputConnectorDesigns]
    public class Outputs
    {
        [OutputConnectorDesign(""aea6829a-ad7f-4a13-a94b-59501a1d5f7d"")]
        [EnumValidator<Side>(Side.Top)]
        public IRuntimeConnector<int> Output1 { get; set; }

        [OutputConnectorDesign(""8e66a89e-c0c2-4b3d-b9ef-f85b00398ec5"")]
        [RangeValidator<int>(10, 20)]
        public IRuntimeConnector<double> Output2 { get; set; }

        [OutputConnectorDesign(""7145d8bb-22ac-42c1-9aa9-96915033d2c8"")]
        [EqualityValidator<double>(42.0, 666.0)]
        public IRuntimeConnector<int> Output3 { get; set; }

        [OutputConnectorDesign(""48619ce9-ae62-4708-ba3c-9a903b89f5a8"")]
        [InequalityValidator<float>(42.0f, 666.0f)]
        public IRuntimeConnector<double> Output4 { get; set; }
    }
}

public enum Side { Top, Bottom }";

        var expected = new[]
        {
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Output1", "EnumValidatorAttribute", "Side", "int")
                .WithLocation(13, 24),
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Output2", "RangeValidatorAttribute", "int", "double")
                .WithLocation(17, 25),
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Output3", "EqualityValidatorAttribute", "double", "int")
                .WithLocation(21, 28),
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Output4", "InequalityValidatorAttribute", "float", "double")
                .WithLocation(25, 30),
        };

        await Verify.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task ReportsDiagnosticIfWrongValueTypeIsUsedOnSetting()
    {
        var code =
@"using ViciOne.Core.Contracts.Runtime;
using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;
using ViciOne.Engine.DefaultValidators;

[FunctionBlockDesign(""eaf194fd-3121-4286-ac96-0cfddc9f9f1f"", ""78079321-b05d-41f8-8051-22ee41e85897"", ""913356fe-3a68-40ec-94ef-227461151fbc"")]
public class Block : RuntimeFunctionBlock<Block>
{
    [SettingDesigns]
    public class Settings
    {
        [SettingDesign(""6bf2f058-528e-40b9-85c1-3c8c7c54d20f"", ""ea161bbf-a86c-4c43-986b-bc0480b0bfd0"", ""8313ca2b-74c2-4a0f-879b-b867b530ece0"")]
        [EnumValidator<Side>(Side.Top)]
        public IRuntimeSetting<int> Setting1 { get; set; }

        [SettingDesign(""c8e05884-6b5c-46f6-8336-66a81f094d07"", ""5e18c354-a74a-483d-9822-b2feb15d0451"", ""94e25679-e1ff-4282-851d-b8a5164d9f45"")]
        [RangeValidator<int>(10, 20)]
        public IRuntimeSetting<double> Setting2 { get; set; }

        [SettingDesign(""a6ebdf95-631f-4fa9-8957-0cd45e21f887"", ""6e266ba8-fcd6-41bf-a005-945758c117c9"", ""ac35593b-1b93-4a44-bdea-2de2122c227d"")]
        [EqualityValidator<double>(42.0, 666.0)]
        public IRuntimeSetting<int> Setting3 { get; set; }

        [SettingDesign(""8f50194b-a482-4eca-8c0b-fc4982d47f6a"", ""4b94d8d4-91e0-46d4-a644-f1fc13c74ea5"", ""a98764bd-7fe2-42b6-bcd8-e9b327a9f9d6"")]
        [InequalityValidator<float>(42.0f, 666.0f)]
        public IRuntimeSetting<double> Setting4 { get; set; }
    }
}

public enum Side { Top, Bottom }";

        var expected = new[]
        {
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Setting1", "EnumValidatorAttribute", "Side", "int")
                .WithLocation(13, 24),
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Setting2", "RangeValidatorAttribute", "int", "double")
                .WithLocation(17, 25),
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Setting3", "EqualityValidatorAttribute", "double", "int")
                .WithLocation(21, 28),
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Setting4", "InequalityValidatorAttribute", "float", "double")
                .WithLocation(25, 30),
        };

        await Verify.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task ReportsDiagnosticIfWrongValueTypeIsUsedOnPersistentVariable()
    {
        var code =
@"using ViciOne.Core.Contracts.Runtime;
using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;
using ViciOne.Engine.DefaultValidators;

[FunctionBlockDesign(""eaf194fd-3121-4286-ac96-0cfddc9f9f1f"", ""78079321-b05d-41f8-8051-22ee41e85897"", ""913356fe-3a68-40ec-94ef-227461151fbc"")]
public class Block : RuntimeFunctionBlock<Block>
{
    [PersistentVariableDesigns]
    public class PersistentVariables
    {
        [PersistentVariableDesign(""c03f12d2-e9e4-49e3-a9e3-a254dc7c1eef"")]
        [EnumValidator<Side>(Side.Top)]
        public IRuntimePersistentVariable<int> Variable1 { get; set; }

        [PersistentVariableDesign(""bc7a3dfa-4a6f-42f2-959d-212e4030d4e0"")]
        [RangeValidator<int>(10, 20)]
        public IRuntimePersistentVariable<double> Variable2 { get; set; }

        [PersistentVariableDesign(""52640b18-ff2d-4b11-90df-9f41e06380c9"")]
        [EqualityValidator<double>(42.0, 666.0)]
        public IRuntimePersistentVariable<int> Variable3 { get; set; }

        [PersistentVariableDesign(""07a4b43f-820f-4690-8f77-a43063816972"")]
        [InequalityValidator<float>(42.0f, 666.0f)]
        public IRuntimePersistentVariable<double> Variable4 { get; set; }
    }
}

public enum Side { Top, Bottom }";

        var expected = new[]
        {
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Variable1", "EnumValidatorAttribute", "Side", "int")
                .WithLocation(13, 24),
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Variable2", "RangeValidatorAttribute", "int", "double")
                .WithLocation(17, 25),
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Variable3", "EqualityValidatorAttribute", "double", "int")
                .WithLocation(21, 28),
            Verify.Diagnostic(ValidatorValueTypeAnalyzer.Rule)
                .WithArguments("Variable4", "InequalityValidatorAttribute", "float", "double")
                .WithLocation(25, 30),
        };

        await Verify.VerifyAnalyzerAsync(code, expected);
    }
}
