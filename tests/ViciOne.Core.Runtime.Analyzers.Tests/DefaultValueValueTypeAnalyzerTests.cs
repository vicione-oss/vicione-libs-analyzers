using System.Threading.Tasks;
using ViciOne.Core.Runtime.Analyzers;
using Xunit;
using Verify = Verifiers.CSharpAnalyzerVerifier<ViciOne.Core.Runtime.Analyzers.DefaultValueValueTypeAnalyzer>;

public class DefaultValueValueTypeAnalyzerTests
{
    [Fact]
    public async Task DoesNotReportAnythingIfCorrectValueTypeIsUsed()
    {
        var code =
@"using ViciOne.Core.Contracts.Runtime;
using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;

[FunctionBlockDesign(""cf8f2711-d03d-40d3-b6c6-ea94ba8a6006"", ""45d328f8-1d2c-4e9f-a17f-2895b02bcfb0"", ""e57c3565-5293-4b8b-a1c4-6197005ca6b1"")]
public class Block : RuntimeFunctionBlock<Block>
{
    [InputConnectorDesigns]
    public class Inputs
    {
        [InputConnectorDesign(""6a370459-0649-4795-aada-c787fdb36ab8"")]
        [DefaultValue(42.0)]
        public IRuntimeConnector<double> Input { get; set; }
    }

    [OutputConnectorDesigns]
    public class Outputs
    {
        [OutputConnectorDesign(""f7b0510a-11fb-410b-a2cc-f83e31e4c639"")]
        [DefaultValue(666L)]
        public IRuntimeConnector<long> Output { get; set; }
    }

    [SettingDesigns]
    public class Settings
    {
        [SettingDesign(""7f40313a-ab68-49fb-af6e-23461416c47b"", ""1c8e70e0-c879-454a-a849-23576a0bed8e"", ""68a2171f-42c6-466b-84fc-746f8d721cee"")]
        [DefaultValue(333)]
        public IRuntimeSetting<int> Setting { get; set; }
    }

    [PersistentVariableDesigns]
    public class PersistentVariables
    {
        [PersistentVariableDesign(""cdf197cb-6def-43ae-a066-5a8d2947bf30"")]
        [DefaultValue(666.0f)]
        public IRuntimePersistentVariable<float> Variable { get; set; }
    }
}";

        await Verify.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReportsDiagnosticIfWrongValueTypeIsUsedOnInput()
    {
        var code =
@"using ViciOne.Core.Contracts.Runtime;
using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;

[FunctionBlockDesign(""cf8f2711-d03d-40d3-b6c6-ea94ba8a6006"", ""45d328f8-1d2c-4e9f-a17f-2895b02bcfb0"", ""e57c3565-5293-4b8b-a1c4-6197005ca6b1"")]
public class Block : RuntimeFunctionBlock<Block>
{
    [InputConnectorDesigns]
    public class Inputs
    {
        [InputConnectorDesign(""6a370459-0649-4795-aada-c787fdb36ab8"")]
        [DefaultValue(42)]
        public IRuntimeConnector<double> Input { get; set; }
    }
}";

        var expected = Verify.Diagnostic(DefaultValueValueTypeAnalyzer.Rule)
            .WithArguments("Input", "int", "double")
            .WithLocation(12, 23);

        await Verify.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task ReportsDiagnosticIfWrongValueTypeIsUsedOnOutput()
    {
        var code =
@"using ViciOne.Core.Contracts.Runtime;
using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;

[FunctionBlockDesign(""cf8f2711-d03d-40d3-b6c6-ea94ba8a6006"", ""45d328f8-1d2c-4e9f-a17f-2895b02bcfb0"", ""e57c3565-5293-4b8b-a1c4-6197005ca6b1"")]
public class Block : RuntimeFunctionBlock<Block>
{
    [OutputConnectorDesigns]
    public class Outputs
    {
        [OutputConnectorDesign(""f7b0510a-11fb-410b-a2cc-f83e31e4c639"")]
        [DefaultValue(666.0)]
        public IRuntimeConnector<long> Output { get; set; }
    }
}";

        var expected = Verify.Diagnostic(DefaultValueValueTypeAnalyzer.Rule)
            .WithArguments("Output", "double", "long")
            .WithLocation(12, 23);

        await Verify.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task ReportsDiagnosticIfWrongValueTypeIsUsedOnSetting()
    {
        var code =
@"using ViciOne.Core.Contracts.Runtime;
using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;

[FunctionBlockDesign(""cf8f2711-d03d-40d3-b6c6-ea94ba8a6006"", ""45d328f8-1d2c-4e9f-a17f-2895b02bcfb0"", ""e57c3565-5293-4b8b-a1c4-6197005ca6b1"")]
public class Block : RuntimeFunctionBlock<Block>
{
    [SettingDesigns]
    public class Settings
    {
        [SettingDesign(""7f40313a-ab68-49fb-af6e-23461416c47b"", ""1c8e70e0-c879-454a-a849-23576a0bed8e"", ""68a2171f-42c6-466b-84fc-746f8d721cee"")]
        [DefaultValue(33.3f)]
        public IRuntimeSetting<int> Setting { get; set; }
    }
}";

        var expected = Verify.Diagnostic(DefaultValueValueTypeAnalyzer.Rule)
            .WithArguments("Setting", "float", "int")
            .WithLocation(12, 23);

        await Verify.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task ReportsDiagnosticIfWrongValueTypeIsUsedOnPersistentVariable()
    {
        var code =
@"using ViciOne.Core.Contracts.Runtime;
using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;

[FunctionBlockDesign(""cf8f2711-d03d-40d3-b6c6-ea94ba8a6006"", ""45d328f8-1d2c-4e9f-a17f-2895b02bcfb0"", ""e57c3565-5293-4b8b-a1c4-6197005ca6b1"")]
public class Block : RuntimeFunctionBlock<Block>
{
    [PersistentVariableDesigns]
    public class PersistentVariables
    {
        [PersistentVariableDesign(""cdf197cb-6def-43ae-a066-5a8d2947bf30"")]
        [DefaultValue(666L)]
        public IRuntimePersistentVariable<float> Variable { get; set; }
    }
}";

        var expected = Verify.Diagnostic(DefaultValueValueTypeAnalyzer.Rule)
            .WithArguments("Variable", "long", "float")
            .WithLocation(12, 23);

        await Verify.VerifyAnalyzerAsync(code, expected);
    }
}
