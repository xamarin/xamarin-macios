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

public class StrongDictionaryDataTests : BaseTransformerTestClass {
	
	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string path = "/some/random/path.cs";

			const string strongDictionary = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[StrongDictionary (""AVCapturePhotoSettingsThumbnailFormatKeys"")]
interface AVCapturePhotoSettingsThumbnailFormat {
	NSString Codec { get; set; }
	NSNumber Width { get; set; }
	NSNumber Height { get; set; }
}
";
			
			yield return [(Source: strongDictionary, Path: "/some/random/path.cs"), new StrongDictionaryData("AVCapturePhotoSettingsThumbnailFormatKeys")];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, StrongDictionaryData expectedData)
	{
		// create a compilation used to create the transformer
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
			.DescendantNodes ().OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var attribute = symbol.GetAttribute<StrongDictionaryData> (AttributesNames.StrongDictionaryAttribute, StrongDictionaryData.TryParse);
		Assert.Equal (expectedData, attribute);
	}
}
