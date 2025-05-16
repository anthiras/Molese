using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerators;

[Generator]
public class EventHandlerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (node, _) => IsEventHandler(node),
            transform: (syntaxContext, _) => (ClassDeclarationSyntax)syntaxContext.Node);
        
        var combined = context.CompilationProvider.Combine(classDeclarations.Collect());
        
        context.RegisterSourceOutput(combined, (productionContext, pair) => GenerateBaseEventHandlerMethods(pair.Left, pair.Right, productionContext));
    }
    
    private static void GenerateBaseEventHandlerMethods(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classDeclarations, SourceProductionContext context)
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
                .Where(IsHandleMethod);
            
            var applyBaseEventMethod = GenerateBaseEventHandlerMethod(classSymbol, applyMethods.ToList());
            
            context.AddSource($"{classSymbol.Name}_Apply.g.cs", SourceText.From(applyBaseEventMethod, Encoding.UTF8));
        }
    }

    private static string GenerateBaseEventHandlerMethod(INamedTypeSymbol classSymbol, List<IMethodSymbol> applyMethods)
    {
        var namespaces = new StringBuilder();
        var cases = new StringBuilder();
        
        foreach (var ns in applyMethods.Select(x => x.Parameters.First().Type.ContainingNamespace).Distinct(SymbolEqualityComparer.Default))
        {
            namespaces.AppendLine($"using {ns};");
        }

        foreach (var handleMethod in applyMethods)
        {
            cases.AppendLine(
                $"""
                          case {handleMethod.Parameters.First().Type.Name} e:
                              await HandleAsync(e, ct);
                              break;
                """);
        }
        
        return 
            $$"""
              {{namespaces}}
              using Framework;
              
              namespace {{classSymbol.ContainingNamespace}};

              public partial class {{classSymbol.Name}}
              {
                  public async Task HandleAsync(Event @event, CancellationToken ct = default)
                  {
                      switch (@event)
                      {
              {{cases}}
                          default:
                              // Ignore
                              break;
                      }
                  }
              }
              """;
    }

    private static bool IsEventHandler(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDeclaration &&
               classDeclaration.IsPartial() &&
               classDeclaration.IsSubclassOf(type => type.GetName() == "IEventHandler");
    }

    private static bool IsHandleMethod(IMethodSymbol method)
    {
        return method is { Name: "HandleAsync", Parameters.Length: 2 }; // &&
               //method.Parameters[0].Type.BaseType?.Name == "Event";
    }
}