using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace Verifiers;

internal static class VerifierOptions
{
    public static ImmutableArray<PackageIdentity> ReferencedPackages { get; } =
        [
            new PackageIdentity("ViciOne.Core.Runtime", "0.47.0"),
            new PackageIdentity("ViciOne.Engine.DefaultValidators", "0.47.0"),
        ];
    public static LanguageVersion LanguageVersion => LanguageVersion.Latest;
}
