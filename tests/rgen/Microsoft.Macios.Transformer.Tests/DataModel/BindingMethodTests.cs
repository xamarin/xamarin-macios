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
using static Microsoft.Macios.Generator.Tests.TestDataFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Transformer.Tests.DataModel;

public class BindingMethodTests : BaseTransformerTestClass {

	readonly TransformerBindingEqualityComparer comparer = new ();

	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";
			var availabilityBuilder = SymbolAvailability.CreateBuilder ();
			availabilityBuilder.Add (new SupportedOSPlatformData ("ios"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("tvos"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("macos"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("maccatalyst"));

			const string noMethods = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[BaseType (typeof (NSObject))]
interface MyNSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {
}
";

			yield return [
				(Source: noMethods, Path: path),
				new Binding (
					symbolName: "MyNSArray",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyNSArray",
					info: new BindingInfo (new ("Foundation.NSObject"), BindingType.Class),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = "Foundation.NSObject",
					UsingDirectives = new HashSet<string> {
						"System",
						"Foundation",
						"CloudKit",
						"ObjCRuntime"
					},
					Interfaces = ["Foundation.INSFastEnumeration"],
					Protocols = [
						"Foundation.NSSecureCoding",
						"Foundation.NSMutableCopying",
						"CloudKit.CKRecordValue"],
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.PartialKeyword)]
				}
			];

			const string noExportedMethods = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[BaseType (typeof (NSObject))]
interface MyNSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {

	public void TestMethod ();
}
";

			yield return [
				(Source: noExportedMethods, Path: path),
				new Binding (
					symbolName: "MyNSArray",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyNSArray",
					info: new BindingInfo (new ("Foundation.NSObject"), BindingType.Class),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = "Foundation.NSObject",
					UsingDirectives = new HashSet<string> {
						"System",
						"Foundation",
						"CloudKit",
						"ObjCRuntime"
					},
					Interfaces = ["Foundation.INSFastEnumeration"],
					Protocols = [
						"Foundation.NSSecureCoding",
						"Foundation.NSMutableCopying",
						"CloudKit.CKRecordValue"],
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.PartialKeyword)]
				}
			];

			const string simpleExportMethod = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[BaseType (typeof (NSObject))]
interface MyNSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {

	[Export (""testMethod"")]
	public void TestMethod ();
}
";

			yield return [
				(Source: simpleExportMethod, Path: path),
				new Binding (
					symbolName: "MyNSArray",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyNSArray",
					info: new BindingInfo (new ("Foundation.NSObject"), BindingType.Class),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = "Foundation.NSObject",
					UsingDirectives = new HashSet<string> {
						"System",
						"Foundation",
						"CloudKit",
						"ObjCRuntime"
					},
					Interfaces = ["Foundation.INSFastEnumeration"],
					Protocols = [
						"Foundation.NSSecureCoding",
						"Foundation.NSMutableCopying",
						"CloudKit.CKRecordValue"],
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.PartialKeyword)],
					Methods = [
						new Method (
							type: "Test.MyNSArray",
							name: "TestMethod",
							returnType: ReturnTypeForVoid (),
							symbolAvailability: availabilityBuilder.ToImmutable (),
							attributes: new (),
							parameters: []) {
							ExportMethodData = new ("testMethod"),
							Modifiers = [
								Token (SyntaxKind.PublicKeyword),
								Token (SyntaxKind.VirtualKeyword),
								Token (SyntaxKind.PartialKeyword),
							]
						}
					]
				}
			];

			const string methodWithParameters = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[BaseType (typeof (NSObject))]
interface MyNSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {
	[Override, Export (""play:"")]
	public void Play ([NullAllowed] string name);
}
";

			yield return [
				(Source: methodWithParameters, Path: path),
				new Binding (
					symbolName: "MyNSArray",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyNSArray",
					info: new BindingInfo (new ("Foundation.NSObject"), BindingType.Class),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = "Foundation.NSObject",
					UsingDirectives = new HashSet<string> {
						"System",
						"Foundation",
						"CloudKit",
						"ObjCRuntime"
					},
					Interfaces = ["Foundation.INSFastEnumeration"],
					Protocols = [
						"Foundation.NSSecureCoding",
						"Foundation.NSMutableCopying",
						"CloudKit.CKRecordValue"],
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.PartialKeyword)],
					Methods = [
						new Method (
							type: "Test.MyNSArray",
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
					]
				}
			];

			const string constructorNoParams = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[BaseType (typeof (NSObject))]
interface MyNSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {

	[Export (""initExample"")]
	public void Constructor ();
}
";

			yield return [
				(Source: constructorNoParams, Path: path),
				new Binding (
					symbolName: "MyNSArray",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyNSArray",
					info: new BindingInfo (new ("Foundation.NSObject"), BindingType.Class),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = "Foundation.NSObject",
					UsingDirectives = new HashSet<string> {
						"System",
						"Foundation",
						"CloudKit",
						"ObjCRuntime"
					},
					Interfaces = ["Foundation.INSFastEnumeration"],
					Protocols = [
						"Foundation.NSSecureCoding",
						"Foundation.NSMutableCopying",
						"CloudKit.CKRecordValue"],
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.PartialKeyword)],
					Constructors = [
						new Constructor (
							type: "Test.MyNSArray",
							symbolAvailability: availabilityBuilder.ToImmutable (),
							attributes: new (),
							parameters: []) {
							ExportMethodData = new ("initExample"),
							Modifiers = [
								Token (SyntaxKind.PublicKeyword),
								Token (SyntaxKind.PartialKeyword),
							]
						}
					]
				}
			];

			const string constructorWithParams = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[BaseType (typeof (NSObject))]
interface MyNSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {

	[Export (""initExample"")]
	public void Constructor ([NullAllowed] string name);
}
";

			yield return [
				(Source: constructorWithParams, Path: path),
				new Binding (
					symbolName: "MyNSArray",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyNSArray",
					info: new BindingInfo (new ("Foundation.NSObject"), BindingType.Class),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = "Foundation.NSObject",
					UsingDirectives = new HashSet<string> {
						"System",
						"Foundation",
						"CloudKit",
						"ObjCRuntime"
					},
					Interfaces = ["Foundation.INSFastEnumeration"],
					Protocols = [
						"Foundation.NSSecureCoding",
						"Foundation.NSMutableCopying",
						"CloudKit.CKRecordValue"],
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.PartialKeyword)],
					Constructors = [
						new Constructor (
							type: "Test.MyNSArray",
							symbolAvailability: availabilityBuilder.ToImmutable (),
							attributes: new (),
							parameters: [
								new (0, ReturnTypeForString (isNullable: true), "name") {
									Attributes = [
										new ("NullAllowedAttribute"),
									]
								},
							]) {
							ExportMethodData = new ("initExample"),
							Modifiers = [
								Token (SyntaxKind.PublicKeyword),
								Token (SyntaxKind.PartialKeyword),
							]
						}
					]
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
				.DescendantNodes ().OfType<InterfaceDeclarationSyntax> ()
				.LastOrDefault ();
		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);

		var binding = new Binding (declaration, symbol, new (semanticModel));
		Assert.Equal (expectedData, binding, comparer);
	}
}
