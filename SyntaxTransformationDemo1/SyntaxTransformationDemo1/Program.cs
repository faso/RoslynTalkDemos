using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.Console;

namespace SyntaxTransformationDemo1
{
    class Program
    {
        private const string sampleCode =
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

        static async Task Main(string[] args)
        {
            // Create a syntax node from scratch
            NameSyntax name = IdentifierName("System");
            WriteLine(name.ToString()); ReadKey();

            name = QualifiedName(name, IdentifierName("Collections"));
            WriteLine(name.ToString()); ReadKey();

            name = QualifiedName(name, IdentifierName("Generic"));
            WriteLine(name.ToString()); ReadKey();

            // Make a syntax tree to put our new node inside of
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sampleCode);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            WriteLine(root.ToString());
            ReadKey(); WriteLine();

            // Make a new using node
            var oldUsing = root.Usings[1];
            var newUsing = oldUsing.WithName(name);

            // Make a new tree
            root = root.ReplaceNode(oldUsing, newUsing);
            WriteLine(root.ToString());

            ReadKey();
        }
    }
}
