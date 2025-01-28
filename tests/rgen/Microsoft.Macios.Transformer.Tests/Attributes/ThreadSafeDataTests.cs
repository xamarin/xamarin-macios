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

public class ThreadSafeDataTests : AttributeParsingTestClass {

	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";

			const string threadSageMethod = @"
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

	[Export (""prepare""), ThreadSafe]
	void Prepare ();
}
";

			yield return [(Source: threadSageMethod, Path: path), new ThreadSafeData ()];

			const string notThreadSafeMethod = @"
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

	[Export (""prepare""), ThreadSafe (false)]
	void Prepare ();
}
";

			yield return [(Source: notThreadSafeMethod, Path: path), new ThreadSafeData (false)];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, ThreadSafeData expectedData)
		=> AssertTryCreate<ThreadSafeData, MethodDeclarationSyntax> (platform, source, AttributesNames.ThreadSafeAttribute,
			expectedData, ThreadSafeData.TryParse, lastOrDefault: true);
}
