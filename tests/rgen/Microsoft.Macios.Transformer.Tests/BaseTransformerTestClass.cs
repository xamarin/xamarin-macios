// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Transformer.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer.Tests;

/// <summary>
/// Base class that allows to test the transformer.
/// </summary>
public class BaseTransformerTestClass {

	// list of the defines for each platform, this is passed to the parser to ensure that
	// we are testing the platforms as if they were being compiled.
	readonly Dictionary<ApplePlatform, string []> platformDefines = new () {
		{ ApplePlatform.iOS, new [] { "__IOS__" } },
		{ ApplePlatform.TVOS, new [] { "__TVOS__" } },
		{ ApplePlatform.MacOSX, new [] { "__MACOS__" } },
		{ ApplePlatform.MacCatalyst, new [] { "__MACCATALYST__" } },
	};

	protected Compilation CreateCompilation (ApplePlatform platform, [CallerMemberName] string name = "", params (string Source, string Path) [] sources)
	{
		// get the dll for the current platform, this is needed because that way we will get the attributes that
		// are used in the old dlls that are needed to test the transformer.
		var targetFramework = TargetFramework.GetTargetFramework (platform, isDotNet: true);
		var workingDirectory = Path.Combine (Configuration.SourceRoot, "src");
		if (!Configuration.TryGetApiDefinitionRsp (targetFramework, out var rspFile)) {
			Assert.Fail ($"Could not find rsp file for {targetFramework}");
		}

		var parseResult = CSharpCommandLineParser.Default.ParseRsp (
			rspFile, workingDirectory, Configuration.DotNetBclDir);

		// add NET to the preprocessor directives
		var preprocessorDirectives = parseResult.ParseOptions.PreprocessorSymbolNames.ToList ();
		preprocessorDirectives.Add ("NET");

		// fixing the parsing options, we must have an issue in the rsp
		var updatedParseOptions = parseResult.ParseOptions
			.WithLanguageVersion (LanguageVersion.Latest)
			.WithPreprocessorSymbols (preprocessorDirectives)
			.WithDocumentationMode (DocumentationMode.None);

		var references = parseResult.GetReferences (workingDirectory, Configuration.DotNetBclDir).ToList ();
		// add the mono cecil assembly, which we are missing in the api compilation rsp
		references.Add (MetadataReference.CreateFromFile (typeof (Mono.Cecil.Cil.OpCode).Assembly.Location));
		var parsedSource = parseResult.GetSourceFiles (updatedParseOptions).ToList ();
		foreach (var (source, path) in sources) {
			parsedSource.Add (CSharpSyntaxTree.ParseText (source, updatedParseOptions, path));
		}

		return CSharpCompilation.Create (
			assemblyName: name,
			syntaxTrees: parsedSource,
			references: references,
			options: parseResult.CompilationOptions);
	}

}
