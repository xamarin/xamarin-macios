using System.Collections.Immutable;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class PropertyAccessorsComparerTests {
	readonly PropertyAccessorsComparer comparer = new();

	[Fact]
	public void CompareTwoEmptyAccessorsArray ()
	{
		var x = ImmutableArray<PropertyAccessorCodeChange>.Empty;
		var y = ImmutableArray<PropertyAccessorCodeChange>.Empty;
		Assert.True (comparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeDiffSizes ()
	{
		ImmutableArray<PropertyAccessorCodeChange> x = [
			new (AccessorKind.Getter,  [], []),
			new (AccessorKind.Setter,  [], []),
		];
		ImmutableArray<PropertyAccessorCodeChange> y = [
			new (AccessorKind.Getter,  [], []),
		];
		
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void CompareSameSizeDiffAccessors ()
	{
		ImmutableArray<PropertyAccessorCodeChange> x = [
			new (AccessorKind.Getter,  [], []),
			new (AccessorKind.Setter,  [], []),
		];
		ImmutableArray<PropertyAccessorCodeChange> y = [
			new (AccessorKind.Add,  [], []),
			new (AccessorKind.Remove,  [], []),
		];
		
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void CompareTwoAccessorsDiffOrder ()
	{
		ImmutableArray<PropertyAccessorCodeChange> x = [
			new (AccessorKind.Getter,  [], []),
			new (AccessorKind.Setter,  [], []),
		];
		ImmutableArray<PropertyAccessorCodeChange> y = [
			new (AccessorKind.Setter,  [], []),
			new (AccessorKind.Getter,  [], []),
		];
		
		Assert.True (comparer.Equals (x, y));
	}

	[Fact]
	public void CompareTwoAccessorsEqual ()
	{
		ImmutableArray<PropertyAccessorCodeChange> x = [
			new (AccessorKind.Getter,  [], []),
			new (AccessorKind.Setter,  [], []),
		];
		
		ImmutableArray<PropertyAccessorCodeChange> y = [
			new (AccessorKind.Getter,  [], []),
			new (AccessorKind.Setter,  [], []),
		];
		
		Assert.True (comparer.Equals (x, y));
	}
}
