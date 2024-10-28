using System.Collections.Immutable;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class AttributesComparerTests {

	readonly AttributesComparer comparer;
	public AttributesComparerTests ()
	{
		comparer = new AttributesComparer ();
	}

	[Fact]
	public void NotEqualsDiffLength ()
	{
		ImmutableArray<AttributeCodeChange> x = [new ("name", []), new ("name1")];
		ImmutableArray<AttributeCodeChange> y = [new ("name", [])];
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void NotEqualsDiffAttributes ()
	{
		ImmutableArray<AttributeCodeChange> x = [new ("name", []), new ("name1")];
		ImmutableArray<AttributeCodeChange> y = [new ("name1", []), new ("name1")];
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsSameOrder ()
	{
		ImmutableArray<AttributeCodeChange> x = [new ("name", []), new ("name1")];
		ImmutableArray<AttributeCodeChange> y = [new ("name", []), new ("name1")];
		Assert.True (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsDiffOrder ()
	{
		ImmutableArray<AttributeCodeChange> x = [new ("name1", []), new ("name")];
		ImmutableArray<AttributeCodeChange> y = [new ("name", []), new ("name1")];
		Assert.True (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsSameOrderSameArguments ()
	{
		ImmutableArray<AttributeCodeChange> x = [new("SupportedOSPlatform", ["ios15.0"]), new("SupportedOSPlatform", ["maccatalyst15.0"])];
		ImmutableArray<AttributeCodeChange> y = [new("SupportedOSPlatform", ["maccatalyst15.0"]), new("SupportedOSPlatform", ["ios15.0"])];
		Assert.True (comparer.Equals (x, y));
	}
}
