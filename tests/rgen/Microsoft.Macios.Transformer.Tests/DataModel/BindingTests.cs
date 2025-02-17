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

public class BindingTests : BaseTransformerTestClass {

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

			const string simpleNSObject = @"
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
				(Source: simpleNSObject, Path: path),
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

			const string internalSimpleNSObject = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[Internal]
[BaseType (typeof (NSObject))]
interface MyNSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {
}
";

			yield return [
				(Source: internalSimpleNSObject, Path: path),
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
						Token (SyntaxKind.InternalKeyword),
						Token (SyntaxKind.PartialKeyword)]
				}
			];

			const string staticNSObject = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[Static]
[BaseType (typeof (NSObject))]
interface MyNSArray { 
}
";

			yield return [
				(Source: staticNSObject, Path: path),
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
					Interfaces = [],
					Protocols = [],
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.StaticKeyword),
						Token (SyntaxKind.PartialKeyword)]
				}
			];

			const string nsObjectWithParent = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[BaseType (typeof (NSArray))]
interface MyNSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {
}
";

			yield return [
				(Source: nsObjectWithParent, Path: path),
				new Binding (
					symbolName: "MyNSArray",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyNSArray",
					info: new BindingInfo (new ("Foundation.NSArray"), BindingType.Class),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = "Foundation.NSArray",
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

			const string publicCategory = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[Category, BaseType (typeof (NSOrderedSet))]
partial interface MyNSKeyValueSorting_NSOrderedSet { }
";

			yield return [
				(Source: publicCategory, Path: path),
				new Binding (
					symbolName: "MyNSKeyValueSorting_NSOrderedSet",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyNSKeyValueSorting_NSOrderedSet",
					info: new BindingInfo (new ("Foundation.NSOrderedSet"), BindingType.Category),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = "object",
					UsingDirectives = new HashSet<string> {
						"System",
						"Foundation",
						"CloudKit",
						"ObjCRuntime"
					},
					Interfaces = [],
					Protocols = [],
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.StaticKeyword),
						Token (SyntaxKind.PartialKeyword)]
				}
			];

			const string internalCategory = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[Internal, Category, BaseType (typeof (NSOrderedSet))]
partial interface MyNSKeyValueSorting_NSOrderedSet { }
";

			yield return [
				(Source: internalCategory, Path: path),
				new Binding (
					symbolName: "MyNSKeyValueSorting_NSOrderedSet",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyNSKeyValueSorting_NSOrderedSet",
					info: new BindingInfo (new ("Foundation.NSOrderedSet"), BindingType.Category),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = "object",
					UsingDirectives = new HashSet<string> {
						"System",
						"Foundation",
						"CloudKit",
						"ObjCRuntime"
					},
					Interfaces = [],
					Protocols = [],
					Modifiers = [
						Token (SyntaxKind.InternalKeyword),
						Token (SyntaxKind.StaticKeyword),
						Token (SyntaxKind.PartialKeyword)]
				}
			];

			const string simpleProtocol = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[Protocol, BaseType (typeof (NSObject))]
interface MyNSCacheDelegate { }

";

			yield return [
				(Source: simpleProtocol, Path: path),
				new Binding (
					symbolName: "MyNSCacheDelegate",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyNSCacheDelegate",
					info: new BindingInfo (new ("Foundation.NSObject"), BindingType.Protocol),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = string.Empty,
					UsingDirectives = new HashSet<string> {
						"System",
						"Foundation",
						"CloudKit",
						"ObjCRuntime"
					},
					Interfaces = [],
					Protocols = [],
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.PartialKeyword)]
				}
			];

			const string protocolWithParent = @"
using System;
using Foundation;
using CloudKit;
using ObjCRuntime;

namespace Test;

[Protocol, BaseType (typeof (NSObject))]
interface MyNSCacheDelegate : NSSecureCoding { }

";

			yield return [
				(Source: protocolWithParent, Path: path),
				new Binding (
					symbolName: "MyNSCacheDelegate",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyNSCacheDelegate",
					info: new BindingInfo (new ("Foundation.NSObject"), BindingType.Protocol),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = string.Empty,
					UsingDirectives = new HashSet<string> {
						"System",
						"Foundation",
						"CloudKit",
						"ObjCRuntime"
					},
					Interfaces = [],
					Protocols = [
						"Foundation.NSSecureCoding",
					],
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.PartialKeyword)]
				}
			];

			const string coreImageFilter = @"
using System;
using Foundation;
using CoreImage;
using ObjCRuntime;

namespace Test;

[CoreImageFilter]
[BaseType (typeof (CIFilter))]
interface MyCIAccordionFoldTransition : CIAccordionFoldTransitionProtocol {
}
";

			yield return [
				(Source: coreImageFilter, Path: path),
				new Binding (
					symbolName: "MyCIAccordionFoldTransition",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyCIAccordionFoldTransition",
					info: new BindingInfo (new ("CoreImage.CIFilter"), BindingType.CoreImageFilter),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = "CoreImage.CIFilter",
					UsingDirectives = new HashSet<string> {
						"System",
						"Foundation",
						"CoreImage",
						"ObjCRuntime"
					},
					Interfaces = [],
					Protocols = ["CoreImage.CIAccordionFoldTransitionProtocol"],
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.PartialKeyword)]
				}
			];
			
			const string strongDictionary = @"
using System;
using Foundation;
using CoreImage;
using ObjCRuntime;

namespace Test;

[StrongDictionary (""CMHevcTemporalLevelInfoKeys"")]
interface MyCMHevcTemporalLevelInfoSettings {

	int TemporalLevel { get; set; }
}
";

			yield return [
				(Source: strongDictionary, Path: path),
				new Binding (
					symbolName: "MyCMHevcTemporalLevelInfoSettings",
					@namespace: ["Test"],
					fullyQualifiedSymbol: "Test.MyCMHevcTemporalLevelInfoSettings",
					info: new BindingInfo (null, BindingType.StrongDictionary),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new ()
				) {
					Base = "Foundation.DictionaryContainer",
					UsingDirectives = new HashSet<string> {
						"System",
						"Foundation",
						"CoreImage",
						"ObjCRuntime"
					},
					Interfaces = [],
					Protocols = [],
					Modifiers = [
						Token (SyntaxKind.PublicKeyword),
						Token (SyntaxKind.PartialKeyword)
					],
					Properties = [
						new (
							name: "TemporalLevel",
							returnType: ReturnTypeForInt (),
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
									attributes: new ())
							]
						) {
							Modifiers = [
								Token (SyntaxKind.PublicKeyword),
								Token (SyntaxKind.VirtualKeyword),
								Token (SyntaxKind.PartialKeyword)
							],
							ExportPropertyData = null, // we want to explicitly set this to null
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
