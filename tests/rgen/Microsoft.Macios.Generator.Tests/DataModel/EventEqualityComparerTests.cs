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
					new (AccessorKind.Getter, [], [])
				]),
			new (
				name: "SecondEvent",
				type: "System.EventHandler",
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (AccessorKind.Getter, [], [])
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
					new (AccessorKind.Getter, [], [])
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
					new (AccessorKind.Getter, [], [])
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
					new (AccessorKind.Getter, [], [])
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
					new (AccessorKind.Getter, [], [])
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
					new (AccessorKind.Getter, [], [])
				]),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}
}
