// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Formatters;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Formatters;

public class SmartEnumFormatterTests : BaseGeneratorTestClass {

	class TestDataToExtensionDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string publicSmartEnum = @"
using System;
using Foundation;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureDeviceType {

	[Field<EnumValue> (""AVCaptureDeviceTypeBuiltInMicrophone"")]
	BuiltInMicrophone,

	[Field<EnumValue> (""AVCaptureDeviceTypeBuiltInWideAngleCamera"")]
	BuiltInWideAngleCamera,
}
";
			yield return [publicSmartEnum, "AVCaptureDeviceTypeExtensions", "public static partial class AVCaptureDeviceTypeExtensions"];

			const string internalSmartEnum = @"
using System;
using Foundation;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
internal enum AVCaptureDeviceType {

	[Field<EnumValue> (""AVCaptureDeviceTypeBuiltInMicrophone"")]
	BuiltInMicrophone,

	[Field<EnumValue> (""AVCaptureDeviceTypeBuiltInWideAngleCamera"")]
	BuiltInWideAngleCamera,
}
";

			yield return [internalSmartEnum, "AVCaptureDeviceTypeExtensions", "internal static partial class AVCaptureDeviceTypeExtensions"];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataToExtensionDeclaration>]
	public void ToDeclarationTests (ApplePlatform platform, string inputText, string className, string expectedDeclaration)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<EnumDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		Assert.NotNull (semanticModel);
		var changes = Binding.FromDeclaration (declaration, semanticModel);
		Assert.NotNull (changes);
		var classDeclaration = changes.ToSmartEnumExtensionDeclaration (className);
		Assert.NotNull (classDeclaration);
		var str = classDeclaration.ToString ();
		Assert.Equal (expectedDeclaration, classDeclaration.ToString ());
	}
}
