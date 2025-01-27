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

public class BindDataTests : BaseTransformerTestClass {

	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";
			var simpleBind = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[NoTV]
[MacCatalyst (13, 1)]
[DisableDefaultCtor]
[Abstract]
[BaseType (typeof (NSObject))]
interface UIFeedbackGenerator : UIInteraction {

	[Bind (""someSelector"")]
	void Prepare ();
}
";

			yield return [(Sorunce: simpleBind, Path: path), new BindData ("someSelector")];

			var virtualBind = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[NoTV]
[MacCatalyst (13, 1)]
[DisableDefaultCtor]
[Abstract]
[BaseType (typeof (NSObject))]
interface UIFeedbackGenerator : UIInteraction {

	[Bind (""someSelector"", Virtual = true)]
	void Prepare ();
}
";

			yield return [(Sorunce: virtualBind, Path: path), new BindData ("someSelector", true)];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source,
		BindData expectedData)
	{
		// create a compilation used to create the transformer
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
			.DescendantNodes ().OfType<MethodDeclarationSyntax> ()
			.LastOrDefault ();
		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var exportData = symbol.GetAttribute<BindData> (
			AttributesNames.BindAttribute, BindData.TryParse);
		Assert.Equal (expectedData, exportData);
	}
}
