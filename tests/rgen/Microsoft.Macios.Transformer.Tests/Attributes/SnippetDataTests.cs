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

public class SnippetDataTests : BaseTransformerTestClass {
	class TestDataTryCreate : IEnumerable<object []> {

		public IEnumerator<object []> GetEnumerator ()
		{
			const string path = "/some/random/path.cs";

			const string disposeAttribute = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[BaseType (typeof (NSControl))]
[Dispose (""dispatcher = null;"", Optimizable = true)]
interface NSButton { }
";

			yield return [(Source: disposeAttribute, Path: path), new SnippetData ("dispatcher = null;", true)];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, SnippetData expectedData)
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
		var attribute = symbol.GetAttribute<SnippetData> (AttributesNames.DisposeAttribute, SnippetData.TryParse);
		Assert.Equal (expectedData, attribute);
	}
}
