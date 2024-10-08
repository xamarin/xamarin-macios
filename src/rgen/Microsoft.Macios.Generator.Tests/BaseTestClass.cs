using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Microsoft.Macios.Generator.Tests;

/// <summary>
/// Base class that allows to test the generator.
/// </summary>
public class BaseTestClass {
	protected BindingSourceGenerator Generator;
	protected CSharpGeneratorDriver _driver;
	protected PortableExecutableReference [] _references;
	// HACK: this is a hack to get the runtime dll for the attributes
	const string RuntimeDll = "/Users/mandel/Xamarin/xamarin-macios/xamarin-macios/src/build/dotnet/ios/64/Microsoft.iOS.dll";

	public BaseTestClass ()
	{
		Generator = new BindingSourceGenerator ();
		_driver = CSharpGeneratorDriver.Create (Generator);

		var dotNetAssemblyPath = Path.GetDirectoryName (typeof (object).Assembly.Location)!;
		_references =
		[
			MetadataReference.CreateFromFile (typeof (object).Assembly.Location),
			MetadataReference.CreateFromFile (Path.Combine (dotNetAssemblyPath, "mscorlib.dll")),
			MetadataReference.CreateFromFile (Path.Combine (dotNetAssemblyPath, "System.dll")),
			MetadataReference.CreateFromFile (Path.Combine (dotNetAssemblyPath, "System.Core.dll")),
			MetadataReference.CreateFromFile (Path.Combine (dotNetAssemblyPath, "System.Drawing.dll")),
			MetadataReference.CreateFromFile (Path.Combine (dotNetAssemblyPath, "System.Drawing.Primitives.dll")),
			MetadataReference.CreateFromFile (Path.Combine (dotNetAssemblyPath, "System.Runtime.InteropServices.dll")),
			MetadataReference.CreateFromFile (Path.Combine (dotNetAssemblyPath, "System.Private.CoreLib.dll")),
			MetadataReference.CreateFromFile (Path.Combine (dotNetAssemblyPath, "System.Runtime.dll")),

			// needed for the attrs Export etc
			MetadataReference.CreateFromFile (RuntimeDll),
		];
	}

	protected void CompareGeneratedCode (string className, string inputFileName, string inputText, string outputFileName, string expectedOutputText)
	{

		// We need to create a compilation with the required source code.
		var compilation = CSharpCompilation.Create (nameof (CompareGeneratedCode),
			new [] { CSharpSyntaxTree.ParseText (inputText) },
			_references);

		// Run generators and retrieve all results.
		var runResult = _driver.RunGenerators (compilation).GetRunResult ();

		// All generated files can be found in 'RunResults.GeneratedTrees'.
		var generatedFileSyntax = runResult.GeneratedTrees.Single (t => t.FilePath.EndsWith ($"{className}.g.cs"));

		// Complex generators should be tested using text comparison.
		Assert.Equal (expectedOutputText, generatedFileSyntax.GetText ().ToString (),
			ignoreLineEndingDifferences: true);

	}
}
