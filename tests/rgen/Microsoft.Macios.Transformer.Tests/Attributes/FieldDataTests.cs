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

public class FieldDataTests : AttributeParsingTestClass {

	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string symbolOnly = @"
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

	[Field (""prepare"")]
	void Prepare { get; }
}
";
			yield return [(Source: symbolOnly, Path: "/some/random/path.cs"), new FieldData ("prepare")];

			const string symbolAndLibrary = @"
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

	[Field (""prepare"", ""LibraryName"")]
	void Prepare { get; }
}
";
			yield return [(Source: symbolAndLibrary, Path: "/some/random/path.cs"), new FieldData ("prepare", "LibraryName")];

			const string symbolAndLibraryNamed = @"
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

	[Field (""prepare"", LibraryName = ""LibraryName"")]
	void Prepare { get; }
}
";
			yield return [(Source: symbolAndLibraryNamed, Path: "/some/random/path.cs"), new FieldData ("prepare", "LibraryName")];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, FieldData expectedData)
		=> AssertTryCreate<FieldData, PropertyDeclarationSyntax> (platform, source, AttributesNames.FieldAttribute,
			expectedData, FieldData.TryParse, lastOrDefault: false);
}
