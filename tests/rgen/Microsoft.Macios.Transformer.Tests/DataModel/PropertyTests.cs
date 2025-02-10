// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Transformer.Attributes;
using Xamarin.Tests;
using Xamarin.Utils;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Transformer.Tests.DataModel;

public class PropertyTests : BaseTransformerTestClass {

	class TestDataTryCreateProperties : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";
			var availabilityBuilder = SymbolAvailability.CreateBuilder ();
			availabilityBuilder.Add (new SupportedOSPlatformData ("ios"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("tvos"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("macos"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("maccatalyst"));

			const string simpleGetter = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Export (""name"")]
	string Name { get; }
}
";
			yield return [
				(Source: simpleGetter, Path: path),
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					accessors: [
						new Accessor (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: availabilityBuilder.ToImmutable (),
							attributes: new ())
					]) {
					ExportPropertyData = new ("name"),
				}
			];

			const string simpleGetterSetter = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Export (""name"")]
	string Name { get; set; }
}
";
			yield return [
				(Source: simpleGetterSetter, Path: path),
				new Property (
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
							attributes: new ())
					]) {
					ExportPropertyData = new ("name"),
				}
			];

			const string staticSimpleGetterSetter = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Static, Export (""name"")]
	string Name { get; set; }
}
";
			yield return [
				(Source: staticSimpleGetterSetter, Path: path),
				new Property (
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
							attributes: new ())
					]) {
					Modifiers = [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword)],
					ExportPropertyData = new ("name"),
				}
			];

			const string abstractSimpleGetterSetter = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Abstract, Export (""name"")]
	string Name { get; set; }
}
";
			yield return [
				(Source: abstractSimpleGetterSetter, Path: path),
				new Property (
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
							attributes: new ())
					]) {
					Modifiers = [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.AbstractKeyword)],
					ExportPropertyData = new ("name"),
				}
			];

			const string accessorsWithExport = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Export (""name"")]
	string Name { 
		[Export (""getName"")]
		get; 
		[Export (""setName"")]
		set; 
	}
}
";
			yield return [
				(Source: accessorsWithExport, Path: path),
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					accessors: [
						new Accessor (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: availabilityBuilder.ToImmutable (),
							attributes: new ()) {
							ExportPropertyData = new ("getName")
						},
						new Accessor (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: availabilityBuilder.ToImmutable (),
							attributes: new ()) {
							ExportPropertyData = new ExportData ("setName")
						}
					]) {
					Modifiers = [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.VirtualKeyword), Token (SyntaxKind.PartialKeyword)],
					ExportPropertyData = new ("name"),
				}
			];

			const string nullableSimpleGetterSetter = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[NullAllowed, Export (""name"")]
	string Name { get; set; }
}
";
			yield return [
				(Source: nullableSimpleGetterSetter, Path: path),
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (isNullable: true),
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
					]) {
					ExportPropertyData = new ("name"),
				}
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	class TestDataTryCreateFields : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";
			var availabilityBuilder = SymbolAvailability.CreateBuilder ();
			availabilityBuilder.Add (new SupportedOSPlatformData ("ios"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("tvos"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("macos"));
			availabilityBuilder.Add (new SupportedOSPlatformData ("maccatalyst"));

			const string simpleField = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Field (""ConstantField"")]
	NSString Name { get; }
}
";
			yield return [
				(Source: simpleField, Path: path),
				new Property (
					name: "Name",
					returnType: ReturnTypeForNSString (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					accessors: [
						new Accessor (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: availabilityBuilder.ToImmutable (),
							attributes: new ())
					]) {
					Modifiers = [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword)],
					ExportFieldData = new ("ConstantField"),
				}
			];

			const string fieldWithLibrary = @"
using System;
using Foundation;
using ObjCRuntime;

interface AVPlayer {
	[Field (""ConstantField"", ""LibraryName"")]
	NSString Name { get; }
}
";
			yield return [
				(Source: fieldWithLibrary, Path: path),
				new Property (
					name: "Name",
					returnType: ReturnTypeForNSString (),
					symbolAvailability: availabilityBuilder.ToImmutable (),
					attributes: new (),
					accessors: [
						new Accessor (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: availabilityBuilder.ToImmutable (),
							attributes: new ())
					]) {
					Modifiers = [Token (SyntaxKind.PublicKeyword), Token (SyntaxKind.StaticKeyword), Token (SyntaxKind.PartialKeyword)],
					ExportFieldData = new ("ConstantField", "LibraryName"),
				}
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreateProperties>]
	[AllSupportedPlatformsClassData<TestDataTryCreateFields>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, Property expectedData)
	{
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		var trees = compilation.SyntaxTrees.Where (s => s.FilePath == source.Path).ToArray ();
		Assert.Single (trees);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
				.DescendantNodes ().OfType<PropertyDeclarationSyntax> ()
				.LastOrDefault ();
		Assert.NotNull (declaration);
		Assert.True (Property.TryCreate (declaration, semanticModel, out var property));
		Assert.Equal (expectedData, property);
	}
}
