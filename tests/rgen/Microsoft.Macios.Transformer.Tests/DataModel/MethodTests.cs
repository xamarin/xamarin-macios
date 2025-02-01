// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Transformer.Tests.DataModel;

public class MethodTests : BaseTransformerTestClass {

	[Theory]
	[InlineData ("Hello", false)]
	[InlineData ("Constructor", true)]
	public void IsConstructorTests (string methodName, bool expectedResult)
	{
		var method = new Method (
			type: "TestClass",
			name: methodName,
			returnType: ReturnTypeForVoid (),
			symbolAvailability: new(),
			attributes: new(),
			parameters: []);
		Assert.Equal (expectedResult, method.IsConstructor);
	}
	
	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";
			var availabilityBuilder = SymbolAvailability.CreateBuilder ();
			availabilityBuilder.Add (new SupportedOSPlatformData ("ios"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("tvos"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("macos"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("maccatalyst"));

			const string simpleMethod = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Export (""play"")]
	public void Play ();
}
";
			yield return [
				(Source: simpleMethod, Path: path),
				new Method (
					type: "AVPlayer",
					name: "Play",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					parameters: []) {
					ExportMethodData = new ("play"),
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.VirtualKeyword),
						Token (SyntaxKind.PartialKeyword),
					]
				}
			];

			const string abstractMethod = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Abstract, Export (""play"")]
	public void Play ();
}
";

			yield return [
				(Source: abstractMethod, Path: path),
				new Method (
					type: "AVPlayer",
					name: "Play",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					parameters: []) {
					ExportMethodData = new ("play"),
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.AbstractKeyword),
					]
				}
			];

			const string internalAbstractMethod = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Internal, Abstract, Export (""play"")]
	public void Play ();
}
";

			yield return [
				(Source: internalAbstractMethod, Path: path),
				new Method (
					type: "AVPlayer",
					name: "Play",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					parameters: []) {
					ExportMethodData = new ("play"),
					Modifiers = [
						Token (SyntaxKind.InternalKeyword),
						Token (SyntaxKind.AbstractKeyword),
					]
				}
			];

			const string newMethod = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[New, Export (""play"")]
	public void Play ();
}
";
			yield return [
				(Source: newMethod, Path: path),
				new Method (
					type: "AVPlayer",
					name: "Play",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					parameters: []) {
					ExportMethodData = new ("play"),
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.NewKeyword),
						Token (SyntaxKind.VirtualKeyword),
						Token (SyntaxKind.PartialKeyword),
					]
				}
			];

			const string overrideMethod = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Override, Export (""play"")]
	public void Play ();
}
";

			yield return [
				(Source: overrideMethod, Path: path),
				new Method (
					type: "AVPlayer",
					name: "Play",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					parameters: []) {
					ExportMethodData = new ("play"),
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.OverrideKeyword),
						Token (SyntaxKind.PartialKeyword),
					]
				}
			];

			const string intReturnMethod = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Export (""play"")]
	public int Play ();
}
";

			yield return [
				(Source: intReturnMethod, Path: path),
				new Method (
					type: "AVPlayer",
					name: "Play",
					returnType: ReturnTypeForInt (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					parameters: []) {
					ExportMethodData = new ("play"),
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.VirtualKeyword),
						Token (SyntaxKind.PartialKeyword),
					]
				}
			];

			const string nullableIntReturnMethod = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Export (""play"")]
	[return: NullAllowed]
	public int Play ();
}
";

			yield return [
				(Source: nullableIntReturnMethod, Path: path),
				new Method (
					type: "AVPlayer",
					name: "Play",
					returnType: ReturnTypeForInt (isNullable: true, keepInterfaces: true),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					parameters: []) {
					ExportMethodData = new ("play"),
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.VirtualKeyword),
						Token (SyntaxKind.PartialKeyword),
					]
				}
			];

			const string stringParameter = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Override, Export (""play:"")]
	public void Play (string name);
}
";

			yield return [
				(Source: stringParameter, Path: path),
				new Method (
					type: "AVPlayer",
					name: "Play",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					parameters: [
						new (0, ReturnTypeForString (), "name"),
					]) {
					ExportMethodData = new ("play:"),
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.OverrideKeyword),
						Token (SyntaxKind.PartialKeyword),
					]
				}
			];

			const string nullableStringParameter = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Override, Export (""play:"")]
	public void Play ([NullAllowed] string name);
}
";

			yield return [
				(Source: nullableStringParameter, Path: path),
				new Method (
					type: "AVPlayer",
					name: "Play",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					parameters: [
						new (0, ReturnTypeForString (isNullable: true), "name") {
							Attributes = [
								new ("NullAllowedAttribute"),
							]
						},
					]) {
					ExportMethodData = new ("play:"),
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.OverrideKeyword),
						Token (SyntaxKind.PartialKeyword),
					]
				}
			];

			const string severalParameter = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Override, Export (""play:"")]
	public void Play (string name, int age);
}
";

			yield return [
				(Source: severalParameter, Path: path),
				new Method (
					type: "AVPlayer",
					name: "Play",
					returnType: ReturnTypeForVoid (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					parameters: [
						new (0, ReturnTypeForString (), "name"),
						new (1, ReturnTypeForInt (), "age"),
					]) {
					ExportMethodData = new ("play:"),
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.OverrideKeyword),
						Token (SyntaxKind.PartialKeyword),
					]
				}
			];
		}


		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, Method expectedData)
	{
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		var trees = compilation.SyntaxTrees.Where (s => s.FilePath == source.Path).ToArray ();
		Assert.Single (trees);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
				.DescendantNodes ().OfType<MethodDeclarationSyntax> ()
				.LastOrDefault ();
		Assert.NotNull (declaration);
		Assert.True (Method.TryCreate (declaration, semanticModel, out var parameter));
		Assert.Equal (expectedData, parameter);
	}
}
