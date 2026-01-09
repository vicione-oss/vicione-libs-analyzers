using System.Threading.Tasks;
using ViciOne.Core.Runtime.Analyzers;
using Xunit;
using Verify = Verifiers.CSharpAnalyzerVerifier<ViciOne.Core.Runtime.Analyzers.InputConnectorDesignAttributeContextAnalyzer>;

public class InputConnectorDesignAttributeContextAnalyzerTests
{
    [Fact]
    public async Task DoesNotReportAnythingIfAttributeIsUsedCorrectly()
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
        public IRuntimeConnector<double> Input { get; set; }
    }
}";

        await Verify.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReportsDiagnosticIfAttributeIsUsedOnOutput()
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
        [InputConnectorDesign(""2545c587-1b47-4ec0-a7b2-0f4684b099f9"")]
        public IRuntimeConnector<long> Output { get; set; }
    }
}";

        var expected = Verify.Diagnostic(InputConnectorDesignAttributeContextAnalyzer.Rule)
            .WithArguments("Output")
            .WithLocation(11, 10);

        await Verify.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task ReportsDiagnosticIfAttributeIsUsedOnSetting()
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
        [InputConnectorDesign(""eee7134f-65b5-4ff6-b2d9-96e1073fa5e2"")]
        public IRuntimeSetting<int> Setting { get; set; }
    }
}";

        var expected = Verify.Diagnostic(InputConnectorDesignAttributeContextAnalyzer.Rule)
            .WithArguments("Setting")
            .WithLocation(11, 10);

        await Verify.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task ReportsDiagnosticIfAttributeIsUsedOnPersistentVariable()
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
        [InputConnectorDesign(""eee7134f-65b5-4ff6-b2d9-96e1073fa5e2"")]
        public IRuntimePersistentVariable<float> Variable { get; set; }
    }
}";

        var expected = Verify.Diagnostic(InputConnectorDesignAttributeContextAnalyzer.Rule)
            .WithArguments("Variable")
            .WithLocation(11, 10);

        await Verify.VerifyAnalyzerAsync(code, expected);
    }
}
