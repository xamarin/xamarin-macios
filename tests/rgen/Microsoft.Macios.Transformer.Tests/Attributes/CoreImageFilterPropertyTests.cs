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

public class CoreImageFilterPropertyTests : BaseTransformerTestClass {
	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string path = "/some/random/path.cs";

			const string coreImageFilterWithProperties = @"
using CoreImage;
using Foundation;
using ObjCRuntime;
using Mono.Cecil;

namespace NS;

[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
[BaseType (typeof (CIFilter))]
interface CICompositingFilter : CIAccordionFoldTransitionProtocol { 

	[CoreImageFilterProperty (""inputBackgroundImage"")]
	CIImage BackgroundImage { get; set; }
}
";
			yield return [
				(Source: coreImageFilterWithProperties, Path: path),
				new CoreImageFilterPropertyData ("inputBackgroundImage")
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source,
		CoreImageFilterPropertyData expectedData)
	{
		// create a compilation used to create the transformer
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
			.DescendantNodes ().OfType<PropertyDeclarationSyntax> ()
			.LastOrDefault ();
		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var exportData =
			symbol.GetAttribute<CoreImageFilterPropertyData> (AttributesNames.CoreImageFilterPropertyAttribute,
				CoreImageFilterPropertyData.TryParse);
		Assert.Equal (expectedData, exportData);
	}
}
