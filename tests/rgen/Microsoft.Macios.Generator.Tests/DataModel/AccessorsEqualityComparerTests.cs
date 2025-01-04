using System.Collections.Immutable;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class AccessorsEqualityComparerTests {
	readonly AccessorsEqualityComparer equalityComparer = new ();

	[Fact]
	public void CompareTwoEmptyAccessorsArray ()
	{
		var x = ImmutableArray<Accessor>.Empty;
		var y = ImmutableArray<Accessor>.Empty;
		Assert.True (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeDiffSizes ()
	{
		ImmutableArray<Accessor> x = [
			new (
				accessorKind: AccessorKind.Getter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
			new (
				accessorKind: AccessorKind.Setter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
		];
		ImmutableArray<Accessor> y = [
			new (
				accessorKind: AccessorKind.Getter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
		];

		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeDiffAccessors ()
	{
		ImmutableArray<Accessor> x = [
			new (
				accessorKind: AccessorKind.Getter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
			new (
				accessorKind: AccessorKind.Setter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
		];
		ImmutableArray<Accessor> y = [
			new (
				accessorKind: AccessorKind.Add,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
			new (
				accessorKind: AccessorKind.Remove,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
		];

		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareTwoAccessorsDiffOrder ()
	{
		ImmutableArray<Accessor> x = [
			new (
				accessorKind: AccessorKind.Getter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
			new (
				accessorKind: AccessorKind.Setter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
		];
		ImmutableArray<Accessor> y = [
			new (
				accessorKind: AccessorKind.Setter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
			new (
				accessorKind: AccessorKind.Getter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareTwoAccessorsEqual ()
	{
		ImmutableArray<Accessor> x = [
			new (
				accessorKind: AccessorKind.Getter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
			new (
				accessorKind: AccessorKind.Setter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
		];

		ImmutableArray<Accessor> y = [
			new (
				accessorKind: AccessorKind.Getter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
			new (
				accessorKind: AccessorKind.Setter,
				symbolAvailability: new (),
				exportPropertyData: null,
				attributes: [],
				modifiers: []),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}
}
