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
			new (AccessorKind.Getter, [], []),
			new (AccessorKind.Setter, [], []),
		];
		ImmutableArray<Accessor> y = [
			new (AccessorKind.Getter, [], []),
		];

		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeDiffAccessors ()
	{
		ImmutableArray<Accessor> x = [
			new (AccessorKind.Getter, [], []),
			new (AccessorKind.Setter, [], []),
		];
		ImmutableArray<Accessor> y = [
			new (AccessorKind.Add, [], []),
			new (AccessorKind.Remove, [], []),
		];

		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareTwoAccessorsDiffOrder ()
	{
		ImmutableArray<Accessor> x = [
			new (AccessorKind.Getter, [], []),
			new (AccessorKind.Setter, [], []),
		];
		ImmutableArray<Accessor> y = [
			new (AccessorKind.Setter, [], []),
			new (AccessorKind.Getter, [], []),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareTwoAccessorsEqual ()
	{
		ImmutableArray<Accessor> x = [
			new (AccessorKind.Getter, [], []),
			new (AccessorKind.Setter, [], []),
		];

		ImmutableArray<Accessor> y = [
			new (AccessorKind.Getter, [], []),
			new (AccessorKind.Setter, [], []),
		];

		Assert.True (equalityComparer.Equals (x, y));
	}
}
