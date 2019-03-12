using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;

namespace SyntaxAnalysisDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string programText =
@"using System;
using System.Collections;
using System.Linq;
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

            // Create an AST
            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            WriteLine();

            // Read things about the root element of the tree
            WriteLine($"The tree is a {root.Kind()} node.");
            WriteLine($"The tree has {root.Members.Count} elements in it.");
            WriteLine($"The tree has {root.Usings.Count} using statements. They are:");
            foreach (UsingDirectiveSyntax element in root.Usings)
                WriteLine($"\t{element.Name}");

            ReadKey(); WriteLine();

            // Let's go deeper
            MemberDeclarationSyntax firstMember = root.Members[0];
            WriteLine($"The first member is a {firstMember.Kind()}.");
            var helloWorldDeclaration = (NamespaceDeclarationSyntax)firstMember;

            ReadKey(); WriteLine();

            // Deeper!
            WriteLine($"There are {helloWorldDeclaration.Members.Count} members declared in this namespace.");
            WriteLine($"The first member is a {helloWorldDeclaration.Members[0].Kind()}.");

            ReadKey(); WriteLine();

            // Even deeper!
            var programDeclaration = (ClassDeclarationSyntax)helloWorldDeclaration.Members[0];
            WriteLine($"There are {programDeclaration.Members.Count} members declared in the {programDeclaration.Identifier} class.");
            WriteLine($"The first member is a {programDeclaration.Members[0].Kind()}.");
            var mainDeclaration = (MethodDeclarationSyntax)programDeclaration.Members[0];

            ReadKey(); WriteLine();

            // Let's tear apart a node
            WriteLine($"The return type of the {mainDeclaration.Identifier} method is {mainDeclaration.ReturnType}.");
            WriteLine($"The method has {mainDeclaration.ParameterList.Parameters.Count} parameters.");
            foreach (ParameterSyntax item in mainDeclaration.ParameterList.Parameters)
                WriteLine($"The type of the {item.Identifier} parameter is {item.Type}.");
            WriteLine($"The body text of the {mainDeclaration.Identifier} method follows:");
            WriteLine(mainDeclaration.Body.ToFullString());

            ReadKey(); WriteLine();

            // We can also query things
            var argsParameter = mainDeclaration.ParameterList.Parameters[0];

            var firstParameters = root.DescendantNodes()
                                      .OfType<MethodDeclarationSyntax>()
                                      .Where(methodDeclaration => methodDeclaration.Identifier.ValueText == "Main")
                                      .Select(methodDeclaration => methodDeclaration.ParameterList.Parameters.First());

            var argsParameter2 = firstParameters.Single();

            WriteLine(argsParameter == argsParameter2);

            ReadKey();
        }
    }
}
