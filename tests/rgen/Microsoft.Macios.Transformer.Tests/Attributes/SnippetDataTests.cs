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

public class SnippetDataTests : AttributeParsingTestClass {
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
		=> AssertTryCreate<SnippetData, BaseTypeDeclarationSyntax> (platform, source, AttributesNames.DisposeAttribute,
			expectedData, SnippetData.TryParse, lastOrDefault: true);
}
