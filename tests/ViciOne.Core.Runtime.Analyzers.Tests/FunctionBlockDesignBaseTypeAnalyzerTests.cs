using System.Threading.Tasks;
using ViciOne.Core.Runtime.Analyzers;
using ViciOne.Core.Runtime.Analyzers.Constants;
using Xunit;
using Verify = Verifiers.CSharpAnalyzerVerifier<ViciOne.Core.Runtime.Analyzers.FunctionBlockDesignBaseTypeAnalyzer>;

public class FunctionBlockDesignBaseTypeAnalyzerTests
{
    [Fact]
    public async Task DoesNotReportAnythingIfBlockIsDirectlyDerivedFromCorrectBaseType()
    {
        var code =
@"using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;

[FunctionBlockDesign(""cf8f2711-d03d-40d3-b6c6-ea94ba8a6006"", ""45d328f8-1d2c-4e9f-a17f-2895b02bcfb0"", ""e57c3565-5293-4b8b-a1c4-6197005ca6b1"")]
public class Block : RuntimeFunctionBlock<Block> { }";

        await Verify.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task DoesNotReportAnythingIfBlockIsIndirectlyDerivedFromCorrectBaseType()
    {
        var code =
@"using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;

[FunctionBlockDesign(""cf8f2711-d03d-40d3-b6c6-ea94ba8a6006"", ""45d328f8-1d2c-4e9f-a17f-2895b02bcfb0"", ""e57c3565-5293-4b8b-a1c4-6197005ca6b1"")]
public class Block : BaseBlock<Block> { }

public abstract class BaseBlock<TBlock> : RuntimeFunctionBlock<TBlock> where TBlock : RuntimeFunctionBlock<TBlock>, new() { }";

        await Verify.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReportsDiagnosticIfBlockIsNotDerivedFromCorrectBaseType()
    {
        var code =
@"using ViciOne.Core.Runtime;
using ViciOne.Core.Runtime.Attributes;

[FunctionBlockDesign(""cf8f2711-d03d-40d3-b6c6-ea94ba8a6006"", ""45d328f8-1d2c-4e9f-a17f-2895b02bcfb0"", ""e57c3565-5293-4b8b-a1c4-6197005ca6b1"")]
public class Block : BaseBlock<Block> { }

public abstract class BaseBlock<TBlock> { }";

        var expected = Verify.Diagnostic(FunctionBlockDesignBaseTypeAnalyzer.Rule)
            .WithArguments("Block", TypeNames.RuntimeFunctionBlock)
            .WithLocation(5, 14);

        await Verify.VerifyAnalyzerAsync(code, expected);
    }
}
