using System.Threading.Tasks;
using ViciOne.Core.Runtime.Analyzers;
using Xunit;
using Verify = Verifiers.CSharpAnalyzerVerifier<ViciOne.Core.Runtime.Analyzers.PersistentVariableDesignAttributeContextAnalyzer>;

public class PersistentVariableDesignAttributeContextAnalyzerTests
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
    [PersistentVariableDesigns]
    public class PersistentVariables
    {
        [PersistentVariableDesign(""cdf197cb-6def-43ae-a066-5a8d2947bf30"")]
        public IRuntimePersistentVariable<float> Variable { get; set; }
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
        [PersistentVariableDesign(""cdf197cb-6def-43ae-a066-5a8d2947bf30"")]
        public IRuntimeConnector<int> Input { get; set; }
    }
}";

        var expected = Verify.Diagnostic(PersistentVariableDesignAttributeContextAnalyzer.Rule)
            .WithArguments("Input")
            .WithLocation(11, 10);

        await Verify.VerifyAnalyzerAsync(code, expected);
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
        [PersistentVariableDesign(""cdf197cb-6def-43ae-a066-5a8d2947bf30"")]
        public IRuntimeConnector<long> Output { get; set; }
    }
}";

        var expected = Verify.Diagnostic(PersistentVariableDesignAttributeContextAnalyzer.Rule)
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
        [PersistentVariableDesign(""cdf197cb-6def-43ae-a066-5a8d2947bf30"")]
        public IRuntimeSetting<int> Setting { get; set; }
    }
}";

        var expected = Verify.Diagnostic(PersistentVariableDesignAttributeContextAnalyzer.Rule)
            .WithArguments("Setting")
            .WithLocation(11, 10);

        await Verify.VerifyAnalyzerAsync(code, expected);
    }
}
