using System.Threading.Tasks;
using ViciOne.Core.Runtime.Analyzers;
using Xunit;
using Verify = Verifiers.CSharpAnalyzerVerifier<ViciOne.Core.Runtime.Analyzers.SettingDesignAttributeContextAnalyzer>;

public class SettingDesignAttributeContextAnalyzerTests
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
    [SettingDesigns]
    public class Settings
    {
        [SettingDesign(""7f40313a-ab68-49fb-af6e-23461416c47b"", ""1c8e70e0-c879-454a-a849-23576a0bed8e"", ""68a2171f-42c6-466b-84fc-746f8d721cee"")]
        public IRuntimeSetting<int> Setting { get; set; }
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
        [SettingDesign(""75d3517a-bfad-44b3-961e-95dc59d3865c"", ""7ad5b3d8-813a-41aa-b54b-51c3552ba733"", ""7fc0bf81-bf8a-48a2-b75e-37b2570c5980"")]
        public IRuntimeConnector<int> Input { get; set; }
    }
}";

        var expected = Verify.Diagnostic(SettingDesignAttributeContextAnalyzer.Rule)
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
        [SettingDesign(""08a9cf6a-a6e5-4852-a490-d1dcbffb2ecf"", ""c6cf3075-4a9b-4554-816a-c5c129dfb77c"", ""5268df5b-23f0-4d83-89f2-5a3a2bd6d555"")]
        public IRuntimeConnector<long> Output { get; set; }
    }
}";

        var expected = Verify.Diagnostic(SettingDesignAttributeContextAnalyzer.Rule)
            .WithArguments("Output")
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
        [SettingDesign(""08a9cf6a-a6e5-4852-a490-d1dcbffb2ecf"", ""c6cf3075-4a9b-4554-816a-c5c129dfb77c"", ""5268df5b-23f0-4d83-89f2-5a3a2bd6d555"")]
        public IRuntimePersistentVariable<float> Variable { get; set; }
    }
}";

        var expected = Verify.Diagnostic(SettingDesignAttributeContextAnalyzer.Rule)
            .WithArguments("Variable")
            .WithLocation(11, 10);

        await Verify.VerifyAnalyzerAsync(code, expected);
    }
}
