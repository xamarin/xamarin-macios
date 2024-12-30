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
			new (AccessorKind.Getter, new (), [], []),
			new (AccessorKind.Setter, new (), [], []),
		];
		ImmutableArray<Accessor> y = [
			new (AccessorKind.Getter, new (), [], []),
		];

		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeDiffAccessors ()
	{
		ImmutableArray<Accessor> x = [
			new (AccessorKind.Getter, new (), [], []),
			new (AccessorKind.Setter, new (), [], []),
		];
		ImmutableArray<Accessor> y = [
			new (AccessorKind.Add, new (), [], []),
			new (AccessorKind.Remove, new (), [], []),
		];

		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareTwoAccessorsDiffOrder ()
	{
		ImmutableArray<Accessor> x = [
			new (AccessorKind.Getter, new (), [], []),
			new (AccessorKind.Setter, new (), [], []),
		];
		ImmutableArray<Accessor> y = [
			new (AccessorKind.Setter, new (), [], []),
			new (AccessorKind.Getter, new (), [], []),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareTwoAccessorsEqual ()
	{
		ImmutableArray<Accessor> x = [
			new (AccessorKind.Getter, new (), [], []),
			new (AccessorKind.Setter, new (), [], []),
		];

		ImmutableArray<Accessor> y = [
			new (AccessorKind.Getter, new (), [], []),
			new (AccessorKind.Setter, new (), [], []),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}
}
