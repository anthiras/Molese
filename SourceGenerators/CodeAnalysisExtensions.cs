using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerators;

public static class CodeAnalysisExtensions
{
    public static bool IsPartial(this ClassDeclarationSyntax classDeclaration) =>
        classDeclaration.Modifiers.Any(token => token.IsKind(SyntaxKind.PartialKeyword));

    public static bool IsSubclassOf(this ClassDeclarationSyntax classDeclaration, Predicate<TypeSyntax> predicate)
    {
        var baseType = classDeclaration.BaseList?.Types.FirstOrDefault()?.Type;
        return baseType != null && predicate(baseType);
    }

    public static string GetName(this TypeSyntax type) => type switch
    {
        GenericNameSyntax generic => generic.Identifier.ToString(),
        IdentifierNameSyntax name => name.Identifier.ToString(),
        _ => throw new NotImplementedException()
    };
}