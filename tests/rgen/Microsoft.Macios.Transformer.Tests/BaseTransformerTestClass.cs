// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

	protected Compilation CreateCompilation (ApplePlatform platform, [CallerMemberName] string name = "", params (string Source, string Path)[] sources)
	{
		// get the dotnet bcl and fully load it for the test.
		var references = Directory.GetFiles (Configuration.DotNetBclDir, "*.dll")
			.Select (assembly => MetadataReference.CreateFromFile (assembly)).ToList ();
		
		// get the dll for the current platform, this is needed because that way we will get the attributes that
		// are used in the old dlls that are needed to test the transformer.
		var targetFramework = TargetFramework.GetTargetFramework (platform, isDotNet: true);
		var platformDll = Configuration.GetBaseLibrary (targetFramework);
		if (!string.IsNullOrEmpty (platformDll)) {
			references.Add (MetadataReference.CreateFromFile (platformDll));
		} else {
			throw new InvalidOperationException ($"Could not find platform dll for {platform}");
		}
		// include the bgen attributes to the compilation, otherwise the transformer will not work.
		var sourcesList = sources.ToList ();
		if (Configuration.TryGetRootPath (out var rootPath)) {
			var oldVersionAttrs = Path.Combine (rootPath, "src", "ObjCRuntime", "PlatformAvailability.cs");
			sourcesList.Add ((File.ReadAllText(oldVersionAttrs), oldVersionAttrs));
			
			var oldBgenAttrs = Path.Combine (rootPath, "src", "bgen", "Attributes.cs");
			sourcesList.Add ((File.ReadAllText(oldBgenAttrs), oldBgenAttrs));
		}

		var parseOptions = new CSharpParseOptions (LanguageVersion.Latest, DocumentationMode.None, preprocessorSymbols: ["COREBUILD"]);
		var trees = sourcesList.Select (
			s => CSharpSyntaxTree.ParseText (s.Source, parseOptions, s.Path))
			.ToImmutableArray ();

		var options = new CSharpCompilationOptions (OutputKind.NetModule)
			.WithAllowUnsafe (true);

		return CSharpCompilation.Create (name, trees, references, options);
	}

}
