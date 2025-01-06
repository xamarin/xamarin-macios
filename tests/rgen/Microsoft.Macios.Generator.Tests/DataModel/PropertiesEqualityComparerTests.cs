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
				isBlittable: false,
				isSmartEnum: false,
				isReferenceType: false,
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
				name: "SecondProperty",
				type: "string",
				isBlittable: false,
				isSmartEnum: false,
				isReferenceType: false,
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
		ImmutableArray<Property> y = [
			new (
				name: "FirstProperty",
				type: "string",
				isBlittable: false,
				isSmartEnum: false,
				isReferenceType: false,
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
		ImmutableArray<Property> x = [
			new (
				name: "FirstProperty",
				type: "string",
				isBlittable: false,
				isSmartEnum: false,
				isReferenceType: false,
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
		ImmutableArray<Property> y = [
			new (
				name: "FirstProperty",
				type: "AVFoundation.AVVideo",
				isBlittable: false,
				isSmartEnum: false,
				isReferenceType: false,
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
		ImmutableArray<Property> x = [
			new (
				name: "FirstProperty",
				type: "string",
				isBlittable: false,
				isSmartEnum: false,
				isReferenceType: false,
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
		ImmutableArray<Property> y = [
			new (
				name: "FirstProperty",
				isSmartEnum: false,
				type: "string",
				isBlittable: false,
				isReferenceType: false,
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

	[Fact]
	public void CompareDiffSmartEnum ()
	{
		ImmutableArray<Property> x = [
			new (
				name: "FirstProperty",
				type: "string",
				isBlittable: false,
				isSmartEnum: false,
				isReferenceType: false,
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
		ImmutableArray<Property> y = [
			new (
				name: "FirstProperty",
				type: "string",
				isBlittable: false,
				isSmartEnum: true,
				isReferenceType: false,
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
}
