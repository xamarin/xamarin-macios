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

public class ExportDataTests : AttributeParsingTestClass {


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
		=> AssertTryCreate<ExportData, MethodDeclarationSyntax> (platform, source, AttributesNames.ExportAttribute,
			expectedData, ExportData.TryParse, lastOrDefault: false);
}
