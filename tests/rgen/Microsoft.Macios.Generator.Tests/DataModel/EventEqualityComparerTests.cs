using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class EventEqualityComparerTests {
	readonly EventEqualityComparer equalityComparer = new ();

	[Fact]
	public void CompareEmptyArrays ()
	{
		ImmutableArray<Event> x = [];
		ImmutableArray<Event> y = [];

		Assert.True (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareDifferentSize ()
	{
		ImmutableArray<Event> x = [
			new (
				name: "FirstEvent",
				type: "System.EventHandler",
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
			new (
				name: "SecondEvent",
				type: "System.EventHandler",
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
		];
		ImmutableArray<Event> y = [
			new (
				name: "FirstEvent",
				type: "System.EventHandler",
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
		ImmutableArray<Event> x = [
			new (
				name: "FirstEvent",
				type: "System.EventHandler",
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
		];
		ImmutableArray<Event> y = [
			new (
				name: "FirstEvent",
				type: "AVFoundation.AVVideo",
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
		ImmutableArray<Event> x = [
			new (
				name: "FirstEvent",
				type: "System.EventHandler",
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, new (), [], [])
				]),
		];
		ImmutableArray<Event> y = [
			new (
				name: "FirstEvent",
				type: "System.EventHandler",
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
}
