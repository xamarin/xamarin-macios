// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.Attributes;
using ObjCRuntime;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer.Tests.Attributes;

public class ExportDataTests : BaseTransformerTestClass {


	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string selectorOnly = @"
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

	[Export (""prepare"")]
	void Prepare ();
}
";
			yield return [(Source: selectorOnly, Path: "/some/random/path.cs"), new ExportData (selector: "prepare")];

			const string selectorWithArgumentSemantic = @"
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

	[Export (""prepare"", ArgumentSemantic.Retain)]
	void Prepare ();
}
";
			yield return [(Source: selectorWithArgumentSemantic, Path: "/some/random/path.cs"), new ExportData (selector: "prepare", argumentSemantic: ArgumentSemantic.Retain)];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, ExportData expectedData)
	{
		// create a compilation used to create the transformer
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.FirstOrDefault ();
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
			.DescendantNodes ().OfType<MethodDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var exportData = symbol.GetAttribute<ExportData> (AttributesNames.ExportAttribute, ExportData.TryParse);
		Assert.Equal (expectedData, exportData);
	}
}
