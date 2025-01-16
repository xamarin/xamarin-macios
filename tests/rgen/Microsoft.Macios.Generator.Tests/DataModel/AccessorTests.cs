// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
#pragma warning disable APL0003
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using ObjCRuntime;
using Xunit;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class AccessorTests {
	[Fact]
	public void CompareDiffKind ()
	{
		var x = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [],
			modifiers: []);
		var y = new Accessor (
			accessorKind: AccessorKind.Setter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [],
			modifiers: []);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareDiffExportData ()
	{
		var x = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: new ("name"),
			attributes: [],
			modifiers: []);
		var y = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: new ("surname"),
			attributes: [],
			modifiers: []);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareSameKindDiffAttrCount ()
	{
		var x = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [
				new ("First"),
				new ("Second"),
			],
			modifiers: []);
		var y = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [
				new ("First"),
			],
			modifiers: []);
		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareSameKindDiffAttr ()
	{
		var x = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [
				new ("First"),
				new ("Second"),
			],
			modifiers: []);
		var y = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [
				new ("Third"),
				new ("Fourth"),
			],
			modifiers: []);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareSameKindDiffAttrOrder ()
	{
		var x = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [
				new ("First"),
				new ("Second"),
			],
			modifiers: []);
		var y = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [
				new ("Second"),
				new ("First"),
			],
			modifiers: []);

		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}

	[Fact]
	public void CompareSameKindSameAttrDiffModifiersCount ()
	{
		var x = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [
				new ("First"),
				new ("Second"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
			]);
		var y = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [
				new ("Second"),
				new ("First"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
			]);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareSameKindSameAttrDiffModifiers ()
	{
		var x = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [
				new ("First"),
				new ("Second"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
			]);
		var y = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [
				new ("Second"),
				new ("First"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
				SyntaxFactory.Token (SyntaxKind.ProtectedKeyword)
			]);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareSameKindSameAttrDiffModifiersOrder ()
	{
		var x = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [
				new ("First"),
				new ("Second"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
			]);
		var y = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: null,
			attributes: [
				new ("Second"),
				new ("First"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			]);

		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}

	[Fact]
	public void CompareSameKindSameAttrDiffAvailability ()
	{
		var builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new SupportedOSPlatformData ("ios17.0"));
		var x = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: builder.ToImmutable (),
			exportPropertyData: null,
			attributes: [
				new ("First"),
				new ("Second"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
			]);
		builder.Clear ();
		builder.Add (new SupportedOSPlatformData ("tvos17.0"));
		var y = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: builder.ToImmutable (),
			exportPropertyData: null,
			attributes: [
				new ("Second"),
				new ("First"),
			], modifiers: [
				SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			]);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareSameKindSameAttrSameAvailability ()
	{
		var builder = SymbolAvailability.CreateBuilder ();
		builder.Add (new SupportedOSPlatformData ("ios17.0"));
		var x = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: builder.ToImmutable (),
			exportPropertyData: null,
			attributes: [
				new ("First"),
				new ("Second"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
			]);
		var y = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: builder.ToImmutable (),
			exportPropertyData: null,
			attributes: [
				new ("Second"),
				new ("First"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PrivateKeyword),
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
			]);

		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}

	[Theory]
	[InlineData (false, false, false)]
	[InlineData (false, true, true)]
	[InlineData (true, false, true)]
	[InlineData (true, true, true)]
	public void ShouldMarshalNativeExceptionsBothFalse (bool propertyHasFlag, bool accessorHasFalg, bool expectedResult)
	{
		var property = new Property (
			name: "MyProperty",
			returnType: ReturnTypeForString (),
			symbolAvailability: new (),
			attributes: [],
			modifiers: [],
			accessors: []
		) {
			ExportPropertyData = new (
				selector: "selector",
				argumentSemantic: ArgumentSemantic.None,
				flags: propertyHasFlag ? ObjCBindings.Property.MarshalNativeExceptions : ObjCBindings.Property.Default),
		};

		var accessor = new Accessor (
			accessorKind: AccessorKind.Getter,
			symbolAvailability: new (),
			exportPropertyData: new (
				selector: "selector",
				argumentSemantic: ArgumentSemantic.None,
				flags: accessorHasFalg ? ObjCBindings.Property.MarshalNativeExceptions : ObjCBindings.Property.Default),
			attributes: [
				new ("First"),
				new ("Second"),
			],
			modifiers: [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				SyntaxFactory.Token (SyntaxKind.PrivateKeyword)
			]);
		Assert.Equal (expectedResult, accessor.ShouldMarshalNativeExceptions (property));
	}


}
