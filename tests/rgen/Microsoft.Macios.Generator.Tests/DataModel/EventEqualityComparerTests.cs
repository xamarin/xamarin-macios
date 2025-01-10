// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (
						accessorKind: AccessorKind.Getter,
						symbolAvailability: new (),
						exportPropertyData: null,
						attributes: [],
						modifiers: []
					)
				]),
			new (
				name: "SecondEvent",
				type: "System.EventHandler",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (
						accessorKind: AccessorKind.Getter,
						symbolAvailability: new (),
						exportPropertyData: null,
						attributes: [],
						modifiers: []
					)
				]),
		];
		ImmutableArray<Event> y = [
			new (
				name: "FirstEvent",
				type: "System.EventHandler",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (
						accessorKind: AccessorKind.Getter,
						symbolAvailability: new (),
						exportPropertyData: null,
						attributes: [],
						modifiers: []
					)
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
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (
						accessorKind: AccessorKind.Getter,
						symbolAvailability: new (),
						exportPropertyData: null,
						attributes: [],
						modifiers: []
					)
				]),
		];
		ImmutableArray<Event> y = [
			new (
				name: "FirstEvent",
				type: "AVFoundation.AVVideo",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (
						accessorKind: AccessorKind.Getter,
						symbolAvailability: new (),
						exportPropertyData: null,
						attributes: [],
						modifiers: []
					)
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
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (
						accessorKind: AccessorKind.Getter,
						symbolAvailability: new (),
						exportPropertyData: null,
						attributes: [],
						modifiers: []
					)
				]),
		];
		ImmutableArray<Event> y = [
			new (
				name: "FirstEvent",
				type: "System.EventHandler",
				symbolAvailability: new (),
				attributes: [],
				modifiers: [
					SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				],
				accessors: [
					new (
						accessorKind: AccessorKind.Getter,
						symbolAvailability: new (),
						exportPropertyData: null,
						attributes: [],
						modifiers: []
					)
				]),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}
}
