using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests;

/// <summary>
/// Base class that allows to test the generator.
/// </summary>
public class BaseGeneratorTestClass {
	protected BindingSourceGeneratorGenerator GeneratorGenerator;
	protected CSharpGeneratorDriver _driver;

	public BaseGeneratorTestClass ()
	{
		GeneratorGenerator = new BindingSourceGeneratorGenerator ();
		_driver = CSharpGeneratorDriver.Create (GeneratorGenerator);
	}

	protected Compilation RunGeneratorsAndUpdateCompilation (Compilation compilation, out ImmutableArray<Diagnostic> diagnostics)
	{
		_driver.RunGeneratorsAndUpdateCompilation (compilation, out var updatedCompilation, out diagnostics);
		return updatedCompilation;
	}

	protected GeneratorDriverRunResult RunGenerators (Compilation compilation)
		=> _driver.RunGenerators (compilation).GetRunResult ();

	protected Compilation CreateCompilation (string name, ApplePlatform platform, params string [] sources)
	{
		// get the dotnet bcl and fully load it for the test.
		var references = Directory.GetFiles (Configuration.DotNetBclDir, "*.dll")
			.Select (assembly => MetadataReference.CreateFromFile (assembly)).ToList ();
		// get the dll for the current platform
		var targetFramework = TargetFramework.GetTargetFramework (platform, isDotNet: true);
		var platformDll = Configuration.GetBaseLibrary (targetFramework);
		if (!string.IsNullOrEmpty (platformDll)) {
			references.Add (MetadataReference.CreateFromFile (platformDll));
		} else {
			throw new InvalidOperationException ($"Could not find platform dll for {platform}");
		}
		var trees = sources.Select (s => CSharpSyntaxTree.ParseText (s));
		var options = new CSharpCompilationOptions (OutputKind.NetModule);
		return CSharpCompilation.Create (name, trees, references, options);
	}

	protected void CompareGeneratedCode (ApplePlatform platform, string className, string inputFileName, string inputText, string outputFileName, string expectedOutputText)
	{
		// We need to create a compilation with the required source code.
		var compilation = CreateCompilation (nameof (CompareGeneratedCode), platform, inputText);

		// Run generators and retrieve all results.
		var runResult = RunGenerators (compilation);

		// All generated files can be found in 'RunResults.GeneratedTrees'.
		var generatedFileSyntax = runResult.GeneratedTrees.Single (t => t.FilePath.EndsWith ($"{className}.g.cs"));

		// Complex generators should be tested using text comparison.
		Assert.Equal (expectedOutputText, generatedFileSyntax.GetText ().ToString (),
			ignoreLineEndingDifferences: true);

	}
}
