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

public class StrongDictionaryDataTests : AttributeParsingTestClass {

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

			yield return [(Source: strongDictionary, Path: path), new StrongDictionaryData ("AVCapturePhotoSettingsThumbnailFormatKeys")];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, StrongDictionaryData expectedData)
		=> AssertTryCreate<StrongDictionaryData, BaseTypeDeclarationSyntax> (platform, source, AttributesNames.StrongDictionaryAttribute,
			expectedData, StrongDictionaryData.TryParse, lastOrDefault: true);
}
