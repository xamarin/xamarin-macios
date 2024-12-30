using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class PropertiesEqualityComparerTests {
	readonly PropertiesEqualityComparer equalityComparer = new ();

	[Fact]
	public void CompareEmptyArrays ()
	{
		ImmutableArray<Property> x = [];
		ImmutableArray<Property> y = [];

		Assert.True (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareDifferentSize ()
	{
		ImmutableArray<Property> x = [
			new (
				name: "FirstProperty",
				type: "string",
				isSmartEnum: false,
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
			new (
				name: "SecondProperty",
				type: "string",
				isSmartEnum: false,
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
		];
		ImmutableArray<Property> y = [
			new (
				name: "FirstProperty",
				type: "string",
				isSmartEnum: false,
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
		];

		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeDiffProperties ()
	{
		ImmutableArray<Property> x = [
			new (
				name: "FirstProperty",
				type: "string",
				isSmartEnum: false,
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
		];
		ImmutableArray<Property> y = [
			new (
				name: "FirstProperty",
				type: "AVFoundation.AVVideo",
				isSmartEnum: false,
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
		];

		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeSameProperties ()
	{
		ImmutableArray<Property> x = [
			new (
				name: "FirstProperty",
				type: "string",
				isSmartEnum: false,
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
		];
		ImmutableArray<Property> y = [
			new (
				name: "FirstProperty",
				isSmartEnum: false,
				type: "string",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}
	
	[Fact]
	public void CompareDiffSmartEnum ()
	{
		ImmutableArray<Property> x = [
			new (
				name: "FirstProperty",
				type: "string",
				isSmartEnum: false,
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
		];
		ImmutableArray<Property> y = [
			new (
				name: "FirstProperty",
				isSmartEnum: true,
				type: "string",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
		];

		Assert.False (equalityComparer.Equals (x, y));
	}
}
