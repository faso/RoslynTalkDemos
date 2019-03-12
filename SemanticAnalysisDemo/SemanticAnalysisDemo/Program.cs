using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;

namespace SemanticAnalysisDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string programText =
@"using System;
using System.Collections.Generic;
using System.Text;
 
namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }
    }
}";
            // Build a tree
            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            // Analyze the tree, build the symbols
            var compilation = CSharpCompilation.Create("HelloWorld")
                .AddReferences(MetadataReference.CreateFromFile(
                    typeof(string).Assembly.Location))
                .AddSyntaxTrees(tree);

            // Get the model
            SemanticModel model = compilation.GetSemanticModel(tree);

            // Find something
            UsingDirectiveSyntax usingSystem = root.Usings[0];
            NameSyntax systemName = usingSystem.Name;

            // Ask the model about our thing
            SymbolInfo nameInfo = model.GetSymbolInfo(systemName);

            // Let's see what exists in the same namespace
            var systemSymbol = (INamespaceSymbol)nameInfo.Symbol;
            foreach (INamespaceSymbol ns in systemSymbol.GetNamespaceMembers())
            {
                WriteLine(ns);
            }

            ReadKey(); WriteLine();

            // Use the syntax model to find the literal string:
            LiteralExpressionSyntax helloWorldString = root.DescendantNodes()
                .OfType<LiteralExpressionSyntax>()
                .Single();

            // Use the semantic model for type information:
            TypeInfo literalInfo = model.GetTypeInfo(helloWorldString);

            var stringTypeSymbol = (INamedTypeSymbol)literalInfo.Type;
            var allMembers = stringTypeSymbol.GetMembers();
            var methods = allMembers.OfType<IMethodSymbol>();
            var publicStringReturningMethods = methods
                .Where(m => m.ReturnType.Equals(stringTypeSymbol) &&
                    m.DeclaredAccessibility == Accessibility.Public);

            var distinctMethods = publicStringReturningMethods.Select(m => m.Name).Distinct();

            // Query the same information
            foreach (string name in (from method in stringTypeSymbol
                .GetMembers().OfType<IMethodSymbol>()
                                     where method.ReturnType.Equals(stringTypeSymbol) &&
                                           method.DeclaredAccessibility == Accessibility.Public
                                     select method.Name).Distinct())
            {
                WriteLine(name);
            }

            ReadKey();
        }
    }
}
