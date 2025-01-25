// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.Attributes;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer.Tests.Attributes;

public class ErrorDomainDataTests : BaseTransformerTestClass {
	
	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";
			const string simpleErrorDomain = @"
using System;
using Foundation;
using ObjCRuntime;

namespace NS;

[Native]
[ErrorDomain (""AVKitErrorDomain"")]
public enum AVKitError : long {
        None = 0,
        Unknown = -1000,
        PictureInPictureStartFailed = -1001,
        ContentRatingUnknown = -1100,
        ContentDisallowedByPasscode = -1101,
        ContentDisallowedByProfile = -1102,
        RecordingFailed = -1200,
}
";
			yield return [(Sorunce: simpleErrorDomain, Path: path), new ErrorDomainData("AVKitErrorDomain")];
			
			const string errorWithLibDomain = @"
using System;
using Foundation;
using ObjCRuntime;

namespace NS;

[Native]
[ErrorDomain (""AVKitErrorDomain"", LibraryName = ""AVKit"")]
public enum AVKitError : long {
        None = 0,
        Unknown = -1000,
        PictureInPictureStartFailed = -1001,
        ContentRatingUnknown = -1100,
        ContentDisallowedByPasscode = -1101,
        ContentDisallowedByProfile = -1102,
        RecordingFailed = -1200,
}
";
			yield return [(Sorunce: errorWithLibDomain, Path: path), new ErrorDomainData("AVKitErrorDomain", "AVKit")];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source,
		ErrorDomainData expectedData)
	{
		// create a compilation used to create the transformer
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
			.DescendantNodes ().OfType<EnumDeclarationSyntax> ()
			.LastOrDefault ();
		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var exportData = symbol.GetAttribute<ErrorDomainData> (
			AttributesNames.ErrorDomainAttribute, ErrorDomainData.TryParse);
		Assert.Equal (expectedData, exportData);
	}
}
