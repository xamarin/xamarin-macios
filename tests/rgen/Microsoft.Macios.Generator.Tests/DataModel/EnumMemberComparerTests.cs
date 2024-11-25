using System.Collections.Immutable;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class EnumMemberComparerTests {

	readonly EnumMemberComparer comparer;

	public EnumMemberComparerTests ()
	{
		comparer = new EnumMemberComparer ();
	}

	[Fact]
	public void NotEqualsDiffLength ()
	{
		ImmutableArray<EnumMemberCodeChange> x = [new ("name", []), new ("name1")];
		ImmutableArray<EnumMemberCodeChange> y = [new ("name", [])];
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void NotEqualsDiffAttributes ()
	{
		ImmutableArray<EnumMemberCodeChange> x = [new ("name", []), new ("name1")];
		ImmutableArray<EnumMemberCodeChange> y = [new ("name1", []), new ("name1")];
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsSameOrder ()
	{
		ImmutableArray<EnumMemberCodeChange> x = [new ("name", []), new ("name1")];
		ImmutableArray<EnumMemberCodeChange> y = [new ("name", []), new ("name1")];
		Assert.True (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsDiffOrder ()
	{
		ImmutableArray<EnumMemberCodeChange> x = [new ("name1", []), new ("name")];
		ImmutableArray<EnumMemberCodeChange> y = [new ("name", []), new ("name1")];
		Assert.True (comparer.Equals (x, y));
	}
}
