using AppKit;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynTestApp {
	static class MainClass {
		static void Main (string [] args)
		{
			NSApplication.IgnoreMissingAssembliesDuringRegistration = true;
			NSApplication.Init ();

			SyntaxTree tree = CSharpSyntaxTree.ParseText (
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
}");

			var root = (CompilationUnitSyntax) tree.GetRoot ();
			System.Console.WriteLine (root);
		}
	}
}
