using System.Collections.Immutable;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class EnumMembersEqualityComparerTests {

	readonly EnumMembersEqualityComparer comparer;

	public EnumMembersEqualityComparerTests ()
	{
		comparer = new EnumMembersEqualityComparer ();
	}

	[Fact]
	public void NotEqualsDiffLength ()
	{
		ImmutableArray<EnumMember> x = [new ("name", new(),[]), new ("name1")];
		ImmutableArray<EnumMember> y = [new ("name",new(), [])];
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void NotEqualsDiffAttributes ()
	{
		ImmutableArray<EnumMember> x = [new ("name", new(),[]), new ("name1")];
		ImmutableArray<EnumMember> y = [new ("name1", new(),[]), new ("name1")];
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsSameOrder ()
	{
		ImmutableArray<EnumMember> x = [new ("name", new SymbolAvailability(),[]), new ("name1")];
		ImmutableArray<EnumMember> y = [new ("name", new SymbolAvailability(),[]), new ("name1")];
		Assert.True (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsDiffOrder ()
	{
		ImmutableArray<EnumMember> x = [new ("name1", new(),[]), new ("name")];
		ImmutableArray<EnumMember> y = [new ("name", new(),[]), new ("name1")];
		Assert.True (comparer.Equals (x, y));
	}
}
