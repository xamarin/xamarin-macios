using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class PropertiesEqualityComparerTests {
	readonly PropertiesEqualityComparer equalityComparer = new();

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
			new(
				name: "FirstProperty",
				type: "string",
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, [], [])
				]),
			new (
				name: "SecondProperty",
				type: "string",
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, [], [])
				]),
		];
		ImmutableArray<Property> y = [
			new(
				name: "FirstProperty",
				type: "string",
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, [], [])
				]),
		];

		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeDiffProperties ()
	{
		ImmutableArray<Property> x = [
			new(
				name: "FirstProperty",
				type: "string",
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, [], [])
				]),
		];
		ImmutableArray<Property> y = [
			new(
				name: "FirstProperty",
				type: "AVFoundation.AVVideo",
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, [], [])
				]),
		];

		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeSameProperties ()
	{
		ImmutableArray<Property> x = [
			new(
				name: "FirstProperty",
				type: "string",
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, [], [])
				]),
		];
		ImmutableArray<Property> y = [
			new(
				name: "FirstProperty",
				type: "string",
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, [], [])
				]),
		];
		
		Assert.True (equalityComparer.Equals (x, y));
	}
}
