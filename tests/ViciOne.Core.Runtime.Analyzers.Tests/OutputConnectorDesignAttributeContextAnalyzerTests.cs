using System.Threading.Tasks;
using ViciOne.Core.Runtime.Analyzers;
using Xunit;
using Verify = Verifiers.CSharpAnalyzerVerifier<ViciOne.Core.Runtime.Analyzers.OutputConnectorDesignAttributeContextAnalyzer>;

public class OutputConnectorDesignAttributeContextAnalyzerTests
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
    [OutputConnectorDesigns]
    public class Outputs
    {
        [OutputConnectorDesign(""f7b0510a-11fb-410b-a2cc-f83e31e4c639"")]
        public IRuntimeConnector<long> Output { get; set; }
    }
}";

        await Verify.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReportsDiagnosticIfAttributeIsUsedOnInput()
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
        [OutputConnectorDesign(""475f7045-1035-4741-b2c0-a18a524e9ef2"")]
        public IRuntimeConnector<int> Input { get; set; }
    }
}";

        var expected = Verify.Diagnostic(OutputConnectorDesignAttributeContextAnalyzer.Rule)
            .WithArguments("Input")
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
        [OutputConnectorDesign(""1daec1bc-87c2-4d60-aec1-f31a9537af71"")]
        public IRuntimeSetting<int> Setting { get; set; }
    }
}";

        var expected = Verify.Diagnostic(OutputConnectorDesignAttributeContextAnalyzer.Rule)
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
        [OutputConnectorDesign(""1daec1bc-87c2-4d60-aec1-f31a9537af71"")]
        public IRuntimePersistentVariable<float> Variable { get; set; }
    }
}";

        var expected = Verify.Diagnostic(OutputConnectorDesignAttributeContextAnalyzer.Rule)
            .WithArguments("Variable")
            .WithLocation(11, 10);

        await Verify.VerifyAnalyzerAsync(code, expected);
    }
}
