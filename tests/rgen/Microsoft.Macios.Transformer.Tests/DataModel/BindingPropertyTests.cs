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

public class BindingPropertyTests : BaseTransformerTestClass {
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

			const string noProperties = @"
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
				(Source: noProperties, Path: path),
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

			const string noExportedProperties = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[BaseType (typeof (NSObject))]
interface MyNSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {
	public string Name { get; set; }
}
";

			yield return [
				(Source: noExportedProperties, Path: path),
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

			const string fieldProperty = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[BaseType (typeof (NSObject))]
interface MyNSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {
	[Field (""NSTextCheckingNameKey"")]
	NSString NameKey { get; }
}
";

			yield return [
				(Source: fieldProperty, Path: path),
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
					Properties = [
						new (
							name: "NameKey",
							returnType: ReturnTypeForNSString (),
							symbolAvailability: availabilityBuilder.ToImmutable (),
							attributes: new (),
							accessors: [
								new Accessor (
									accessorKind: AccessorKind.Getter,
									symbolAvailability: availabilityBuilder.ToImmutable (),
									attributes: new ())
							]
						) {
							Modifiers = [
								Token (SyntaxKind.PublicKeyword),
								Token (SyntaxKind.StaticKeyword),
								Token (SyntaxKind.PartialKeyword)
							],
							ExportFieldData = new ("NSTextCheckingNameKey")
						}
					]
				}
			];

			const string exportPropertyGetter = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[BaseType (typeof (NSObject))]
interface MyNSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {
	[Export (""name"")]
	string Name { get; }
}
";

			yield return [
				(Source: exportPropertyGetter, Path: path),
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
					Properties = [
						new (
							name: "Name",
							returnType: ReturnTypeForString (),
							symbolAvailability: availabilityBuilder.ToImmutable (),
							attributes: new (),
							accessors: [
								new Accessor (
									accessorKind: AccessorKind.Getter,
									symbolAvailability: availabilityBuilder.ToImmutable (),
									attributes: new ())
							]
						) {
							Modifiers = [
								Token (SyntaxKind.PublicKeyword),
								Token (SyntaxKind.VirtualKeyword),
								Token (SyntaxKind.PartialKeyword)
							],
							ExportPropertyData = new ("name")
						}
					]
				}
			];

			const string exportPropertyGetterSetter = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[BaseType (typeof (NSObject))]
interface MyNSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {
	[Export (""name"")]
	string Name { get; set;}
}
";

			yield return [
				(Source: exportPropertyGetterSetter, Path: path),
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
					Properties = [
						new (
							name: "Name",
							returnType: ReturnTypeForString (),
							symbolAvailability: availabilityBuilder.ToImmutable (),
							attributes: new (),
							accessors: [
								new Accessor (
									accessorKind: AccessorKind.Getter,
									symbolAvailability: availabilityBuilder.ToImmutable (),
									attributes: new ()),
								new Accessor (
									accessorKind: AccessorKind.Setter,
									symbolAvailability: availabilityBuilder.ToImmutable (),
									attributes: new ()),
							]
						) {
							Modifiers = [
								Token (SyntaxKind.PublicKeyword),
								Token (SyntaxKind.VirtualKeyword),
								Token (SyntaxKind.PartialKeyword)
							],
							ExportPropertyData = new ("name")
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
