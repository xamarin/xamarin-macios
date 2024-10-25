using System.Collections.Immutable;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class MemberComparerTests {
	
	readonly MemberComparer comparer;

	public MemberComparerTests ()
	{
		comparer = new MemberComparer ();
	}
	
	[Fact]
	public void NotEqualsDiffLength ()
	{
		ImmutableArray<MemberCodeChange> x = [new("name", []), new("name1")];
		ImmutableArray<MemberCodeChange> y = [new("name", [])];
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void NotEqualsDiffAttributes ()
	{
		ImmutableArray<MemberCodeChange> x = [new("name", []), new("name1")];
		ImmutableArray<MemberCodeChange> y = [new("name1", []), new("name1")];
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsSameOrder ()
	{
		ImmutableArray<MemberCodeChange> x = [new("name", []), new("name1")];
		ImmutableArray<MemberCodeChange> y = [new("name", []), new("name1")];
		Assert.True (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsDiffOrder ()
	{
		ImmutableArray<MemberCodeChange> x = [new("name1", []), new("name")];
		ImmutableArray<MemberCodeChange> y = [new("name", []), new("name1")];
		Assert.True (comparer.Equals (x, y));
	}
}
