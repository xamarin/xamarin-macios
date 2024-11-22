using System.Collections.Immutable;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class PropertyAccessorsEqualityComparerTests {
	readonly PropertyAccessorsEqualityComparer equalityComparer = new();

	[Fact]
	public void CompareTwoEmptyAccessorsArray ()
	{
		var x = ImmutableArray<PropertyAccessor>.Empty;
		var y = ImmutableArray<PropertyAccessor>.Empty;
		Assert.True (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeDiffSizes ()
	{
		ImmutableArray<PropertyAccessor> x = [
			new (AccessorKind.Getter,  [], []),
			new (AccessorKind.Setter,  [], []),
		];
		ImmutableArray<PropertyAccessor> y = [
			new (AccessorKind.Getter,  [], []),
		];
		
		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeDiffAccessors ()
	{
		ImmutableArray<PropertyAccessor> x = [
			new (AccessorKind.Getter,  [], []),
			new (AccessorKind.Setter,  [], []),
		];
		ImmutableArray<PropertyAccessor> y = [
			new (AccessorKind.Add,  [], []),
			new (AccessorKind.Remove,  [], []),
		];
		
		Assert.False (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareTwoAccessorsDiffOrder ()
	{
		ImmutableArray<PropertyAccessor> x = [
			new (AccessorKind.Getter,  [], []),
			new (AccessorKind.Setter,  [], []),
		];
		ImmutableArray<PropertyAccessor> y = [
			new (AccessorKind.Setter,  [], []),
			new (AccessorKind.Getter,  [], []),
		];
		
		Assert.True (equalityComparer.Equals (x, y));
	}

	[Fact]
	public void CompareTwoAccessorsEqual ()
	{
		ImmutableArray<PropertyAccessor> x = [
			new (AccessorKind.Getter,  [], []),
			new (AccessorKind.Setter,  [], []),
		];
		
		ImmutableArray<PropertyAccessor> y = [
			new (AccessorKind.Getter,  [], []),
			new (AccessorKind.Setter,  [], []),
		];
		
		Assert.True (equalityComparer.Equals (x, y));
	}
}
