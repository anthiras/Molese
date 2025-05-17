using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerators;

[Generator]
public class AggregateRootApplyMethodGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (node, _) => IsAggregateRoot(node),
            transform: (syntaxContext, _) => (ClassDeclarationSyntax)syntaxContext.Node);
        
        var combined = context.CompilationProvider.Combine(classDeclarations.Collect());
        
        context.RegisterSourceOutput(combined, (productionContext, pair) => GenerateApplyBaseEventMethods(pair.Left, pair.Right, productionContext));
    }
    
    private static void GenerateApplyBaseEventMethods(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classDeclarations, SourceProductionContext context)
    {
        foreach (var classDeclaration in classDeclarations)
        {
            var classSymbol = compilation.GetSemanticModel(classDeclaration.SyntaxTree)
                .GetDeclaredSymbol(classDeclaration);
            
            if (classSymbol == null)
                continue;

            var applyMethods = classSymbol
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(IsApplyMethod);
            
            var applyBaseEventMethod = GenerateApplyBaseEventMethod(classSymbol, applyMethods);
            
            context.AddSource($"{classSymbol.Name}_Apply.g.cs", SourceText.From(applyBaseEventMethod, Encoding.UTF8));
        }
    }

    private static string GenerateApplyBaseEventMethod(INamedTypeSymbol classSymbol, IEnumerable<IMethodSymbol> applyMethods)
    {
        var cases = new StringBuilder();

        foreach (var applyMethod in applyMethods)
        {
            cases.AppendLine(
                $"""
                          case {applyMethod.Parameters.First().Type.Name} e:
                              Apply(e);
                              break;
                """);
        }
        
        return 
            $$"""
              using Framework;
              
              namespace {{classSymbol.ContainingNamespace}};

              public partial class {{classSymbol.Name}}
              {
                  protected override void Apply(Event<{{classSymbol.Name}}> @event)
                  {
                      switch (@event)
                      {
              {{cases}}
                          default:
                              throw new NotImplementedException($"Event type {@event.GetType()} not implemented.");
                      }
                  }
              }
              """;
    }

    private static bool IsAggregateRoot(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDeclaration &&
               classDeclaration.IsPartial() &&
               classDeclaration.IsSubclassOf(type => type.GetName() == "AggregateRoot");
    }

    private static bool IsApplyMethod(IMethodSymbol method)
    {
        return method is { Name: "Apply", Parameters.Length: 1 } &&
               method.Parameters[0].Type.BaseType?.Name == "Event";
    }
}