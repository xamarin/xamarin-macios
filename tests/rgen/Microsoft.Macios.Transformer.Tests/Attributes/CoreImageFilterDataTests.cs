// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.Attributes;
using Xamarin.Tests;
using Xamarin.Utils;
using MethodAttributes = Mono.Cecil.MethodAttributes;

namespace Microsoft.Macios.Transformer.Tests.Attributes;

public class CoreImageFilterDataTests : AttributeParsingTestClass {
	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string path = "/some/random/path.cs";

			const string simpleCoreImageFilter = @"
using System;
using CoreImage;
using Foundation;
using ObjCRuntime;

namespace NS;

[CoreImageFilter]
[BaseType (typeof (CIFilter))]
interface CIAccordionFoldTransition : CIAccordionFoldTransitionProtocol {
	[CoreImageFilterProperty (""inputNumberOfFolds"")]
	int NumberOfFolds { get; set; }
}
";

			yield return [(Source: simpleCoreImageFilter, Path: path), new CoreImageFilterData () { }];

			const string coreImageFilterWithProperties = @"
using CoreImage;
using Foundation;
using ObjCRuntime;
using Mono.Cecil;

namespace NS;

[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
[BaseType (typeof (CIFilter))]
interface CICompositingFilter : CIAccordionFoldTransitionProtocol { 

	[CoreImageFilterProperty (""inputImage"")]
	CIImage InputImage { get; set; }

	[CoreImageFilterProperty (""inputBackgroundImage"")]
	CIImage BackgroundImage { get; set; }
}
";

			yield return [(Source: coreImageFilterWithProperties, Path: path), new CoreImageFilterData () { IntPtrCtorVisibility = MethodAttributes.Family }];

			const string allProperties = @"
using CoreImage;
using Foundation;
using ObjCRuntime;
using Mono.Cecil;

namespace NS;

[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Private, IntPtrCtorVisibility = MethodAttributes.Family, StringCtorVisibility = MethodAttributes.Family)] // was already protected in classic
[BaseType (typeof (CIFilter))]
interface CICompositingFilter : CIAccordionFoldTransitionProtocol { 

	[CoreImageFilterProperty (""inputImage"")]
	CIImage InputImage { get; set; }

	[CoreImageFilterProperty (""inputBackgroundImage"")]
	CIImage BackgroundImage { get; set; }
}
";

			yield return [(Source: allProperties, Path: path),
				new CoreImageFilterData () {
					IntPtrCtorVisibility = MethodAttributes.Family,
					DefaultCtorVisibility = MethodAttributes.Private,
					StringCtorVisibility = MethodAttributes.Family,
				}];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, CoreImageFilterData expectedData)
		=> AssertTryCreate<CoreImageFilterData, BaseTypeDeclarationSyntax> (platform, source, AttributesNames.CoreImageFilterAttribute,
			expectedData, CoreImageFilterData.TryParse, lastOrDefault: true);
}
