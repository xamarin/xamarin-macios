// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Transformer.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Transformer.Tests.DataModel;

public class SmartEnumTests : BaseTransformerTestClass {

	readonly BindingEqualityComparer comparer = new ();
	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";
			var availabilityBuilder = SymbolAvailability.CreateBuilder ();
			availabilityBuilder.Add (new SupportedOSPlatformData ("ios"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("tvos"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("macos"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("maccatalyst"));

			const string smartEnum = @"
using System;
using Foundation;
using ObjCRutime;

namespace AVFoundation;

public enum AVCaptureDeviceType {

	[Field (""AVCaptureDeviceTypeBuiltInMicrophone"")]
	BuiltInMicrophone,

	[Field (""AVCaptureDeviceTypeBuiltInWideAngleCamera"")]
	BuiltInWideAngleCamera,

	[Field (""AVCaptureDeviceTypeBuiltInTelephotoCamera"")]
	BuiltInTelephotoCamera,
}
";

			yield return [
				(Source: smartEnum, Path: path),
				new Binding (
					symbolName: "AVCaptureDeviceType",
					@namespace: ["AVFoundation"],
					fullyQualifiedSymbol: "AVFoundation.AVCaptureDeviceType",
					info: new BindingInfo (null, BindingType.SmartEnum),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()) {
					Base = "System.Enum",
					Modifiers = [Token (SyntaxKind.PublicKeyword)],
					EnumMembers = [
						new EnumMember (
							name: "BuiltInMicrophone",
							libraryName: "AVFoundation",
							libraryPath: null,
							fieldData: new ("AVCaptureDeviceTypeBuiltInMicrophone"),
							symbolAvailability: availabilityBuilder.ToImmutable ()),
						new EnumMember (
							name: "BuiltInWideAngleCamera",
							libraryName: "AVFoundation",
							libraryPath: null,
							fieldData: new ("AVCaptureDeviceTypeBuiltInWideAngleCamera"),
							symbolAvailability: availabilityBuilder.ToImmutable ()),
						new EnumMember (
							name: "BuiltInTelephotoCamera",
							libraryName: "AVFoundation",
							libraryPath: null,
							fieldData: new ("AVCaptureDeviceTypeBuiltInTelephotoCamera"),
							symbolAvailability: availabilityBuilder.ToImmutable ()),
					],
					UsingDirectives = new HashSet<string> { "System", "Foundation", "ObjCRutime" },
				}
			];

			const string missingField = @"
using System;
using Foundation;
using ObjCRutime;

namespace AVFoundation;

public enum AVCaptureDeviceType {

	[Field (""AVCaptureDeviceTypeBuiltInMicrophone"")]
	BuiltInMicrophone,

	[Field (""AVCaptureDeviceTypeBuiltInWideAngleCamera"")]
	BuiltInWideAngleCamera,

	// missing attr, this should be ignored
	BuiltInTelephotoCamera,
}
";

			yield return [
				(Source: missingField, Path: path),

				new Binding (
					symbolName: "AVCaptureDeviceType",
					@namespace: ["AVFoundation"],
					fullyQualifiedSymbol: "AVFoundation.AVCaptureDeviceType",
					info: new BindingInfo (null, BindingType.SmartEnum),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()) {
					Base = "System.Enum",
					Modifiers = [Token (SyntaxKind.PublicKeyword)],
					EnumMembers = [
						new EnumMember (
							name: "BuiltInMicrophone",
							libraryName: "AVFoundation",
							libraryPath: null,
							fieldData: new ("AVCaptureDeviceTypeBuiltInMicrophone"),
							symbolAvailability: availabilityBuilder.ToImmutable ()),
						new EnumMember (
							name: "BuiltInWideAngleCamera",
							libraryName: "AVFoundation",
							libraryPath: null,
							fieldData: new ("AVCaptureDeviceTypeBuiltInWideAngleCamera"),
							symbolAvailability: availabilityBuilder.ToImmutable ()),
					],
					UsingDirectives = new HashSet<string> { "System", "Foundation", "ObjCRutime" },
				}
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, Binding expectedData)
	{
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		var trees = compilation.SyntaxTrees.Where (s => s.FilePath == source.Path).ToArray ();
		Assert.Single (trees);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
				.DescendantNodes ().OfType<EnumDeclarationSyntax> ()
				.LastOrDefault ();
		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);

		var binding = new Binding (declaration, symbol, new (semanticModel));
		Assert.Equal (expectedData, binding, comparer);
	}
}
