// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer.Tests.Attributes;

public class AttributeParsingTestClass : BaseTransformerTestClass {

	internal void AssertTryCreate<T, TR> (ApplePlatform platform, (string Source, string Path) source,
		string attributeName, T expectedData, TypeSymbolExtensions.TryParse<T> tryParse, bool lastOrDefault = false)
		where T : struct
		where TR : MemberDeclarationSyntax
	{
		// create a compilation used to create the transformer
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		var trees = compilation.SyntaxTrees.Where (s => s.FilePath == source.Path).ToArray ();
		Assert.Single (trees);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		TR? declaration = null;
		if (lastOrDefault) {
			declaration = syntaxTree.GetRoot ()
				.DescendantNodes ().OfType<TR> ()
				.LastOrDefault ();
		} else {
			declaration = syntaxTree.GetRoot ()
				.DescendantNodes ().OfType<TR> ()
				.FirstOrDefault ();
		}

		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var attribute = symbol.GetAttribute (attributeName, tryParse);
		Assert.NotNull (attribute);
		Assert.Equal (expectedData, attribute.Value);
	}
}
