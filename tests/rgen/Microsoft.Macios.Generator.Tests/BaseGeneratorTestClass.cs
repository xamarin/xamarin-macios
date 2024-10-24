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
	protected CSharpGeneratorDriver Driver;

	// list of the defines for each platform, this is passed to the parser to ensure that
	// we are testing the platforms as if they were being compiled.
	readonly Dictionary<ApplePlatform, string []> platformDefines = new () {
		{ ApplePlatform.iOS, new [] { "__IOS__" } },
		{ ApplePlatform.TVOS, new [] { "__TVOS__" } },
		{ ApplePlatform.MacOSX, new [] { "__MACOS__" } },
		{ ApplePlatform.MacCatalyst, new [] { "__MACCATALYST__" } },
	};

	public BaseGeneratorTestClass ()
	{
		GeneratorGenerator = new BindingSourceGeneratorGenerator ();
		Driver = CSharpGeneratorDriver.Create (GeneratorGenerator);
	}

	protected Compilation RunGeneratorsAndUpdateCompilation (Compilation compilation, out ImmutableArray<Diagnostic> diagnostics)
	{
		Driver.RunGeneratorsAndUpdateCompilation (compilation, out var updatedCompilation, out diagnostics);
		return updatedCompilation;
	}

	protected GeneratorDriverRunResult RunGenerators (Compilation compilation)
		=> Driver.RunGenerators (compilation).GetRunResult ();

	protected (Compilation Compilation, ImmutableArray<SyntaxTree> SyntaxTrees) CreateCompilation (string name, ApplePlatform platform, params string [] sources)
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

		var parseOptions = new CSharpParseOptions (LanguageVersion.Latest, DocumentationMode.None, preprocessorSymbols: platformDefines [platform]);
		var trees = sources.Select (s => CSharpSyntaxTree.ParseText (s, parseOptions)).ToImmutableArray ();

		var options = new CSharpCompilationOptions (OutputKind.NetModule)
			.WithAllowUnsafe (true);

		return (CSharpCompilation.Create (name, trees, references, options), trees);
	}

	protected void CompareGeneratedCode (ApplePlatform platform, string className, string inputFileName, string inputText, string outputFileName, string expectedOutputText, string? expectedLibraryText)
	{
		// We need to create a compilation with the required source code.
		var (compilation, _) = CreateCompilation (nameof (CompareGeneratedCode), platform, inputText);

		// Run generators and retrieve all results.
		var runResult = RunGenerators (compilation);

		// All generated files can be found in 'RunResults.GeneratedTrees'.
		var generatedFileSyntax = runResult.GeneratedTrees.Single (t => t.FilePath.EndsWith ($"{className}.g.cs"));

		// Complex generators should be tested using text comparison.
		Assert.Equal (expectedOutputText, generatedFileSyntax.GetText ().ToString (),
			ignoreLineEndingDifferences: true);

		if (expectedLibraryText is not null) {
			// validate that Library.g.cs was created by the LibraryEmitter and matches the expectation
			var generatedLibSyntax = runResult.GeneratedTrees.Single (t => t.FilePath.EndsWith ("Libraries.g.cs"));
			Assert.Equal (expectedLibraryText, generatedLibSyntax.GetText ().ToString ());
		}

	}
}
