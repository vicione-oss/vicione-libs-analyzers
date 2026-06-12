using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ViciOne.Core.Runtime.Analyzers.Extensions;

internal static class AttributeDataExtensions
{
    extension(AttributeData attribute)
    {
        public Location? GetLocation()
            => attribute.ApplicationSyntaxReference?.SyntaxTree.GetLocation(attribute.ApplicationSyntaxReference.Span);

        public Location? GetArgumentLocation(int index)
            => (attribute.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax)?.ArgumentList?.Arguments[index].GetLocation();

        public Location? GetTypeArgumentLocation(int index)
            => ((attribute.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax)?.Name as GenericNameSyntax)?.TypeArgumentList?.Arguments[index].GetLocation();
    }
}
