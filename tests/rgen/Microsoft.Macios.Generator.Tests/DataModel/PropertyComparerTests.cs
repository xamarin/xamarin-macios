using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class PropertyComparerTests {
	readonly PropertyComparer comparer = new ();

	[Fact]
	public void CompareEmptyArrays ()
	{
		ImmutableArray<PropertyCodeChange> x = [];
		ImmutableArray<PropertyCodeChange> y = [];

		Assert.True (comparer.Equals (x, y));
	}

	[Fact]
	public void CompareDifferentSize ()
	{
		ImmutableArray<PropertyCodeChange> x = [
			new (
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
		ImmutableArray<PropertyCodeChange> y = [
			new (
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

		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeDiffProperties ()
	{
		ImmutableArray<PropertyCodeChange> x = [
			new (
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
		ImmutableArray<PropertyCodeChange> y = [
			new (
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

		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeSameProperties ()
	{
		ImmutableArray<PropertyCodeChange> x = [
			new (
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
		ImmutableArray<PropertyCodeChange> y = [
			new (
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

		Assert.True (comparer.Equals (x, y));
	}
}
