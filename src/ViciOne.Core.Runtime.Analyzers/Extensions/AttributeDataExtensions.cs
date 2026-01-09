using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ViciOne.Core.Runtime.Analyzers.Extensions;

internal static class AttributeDataExtensions
{
    public static Location? GetLocation(this AttributeData attribute)
        => attribute.ApplicationSyntaxReference?.SyntaxTree.GetLocation(attribute.ApplicationSyntaxReference.Span);

    public static Location? GetArgumentLocation(this AttributeData attribute, int index)
        => (attribute.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax)?.ArgumentList?.Arguments[index].GetLocation();

    public static Location? GetTypeArgumentLocation(this AttributeData attribute, int index)
        => ((attribute.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax)?.Name as GenericNameSyntax)?.TypeArgumentList?.Arguments[index].GetLocation();
}
