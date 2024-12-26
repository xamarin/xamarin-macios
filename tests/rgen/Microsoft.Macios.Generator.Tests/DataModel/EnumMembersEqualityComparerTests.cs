#pragma warning disable APL0003
using System.Collections.Immutable;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using ObjCBindings;
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
		ImmutableArray<EnumMember> x = [new ("name", new (), new (), []), new ("name1")];
		ImmutableArray<EnumMember> y = [new ("name", new (), new (), [])];
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void NotEqualsDiffAttributes ()
	{
		ImmutableArray<EnumMember> x = [new ("name", new (), new (), []), new ("name1")];
		ImmutableArray<EnumMember> y = [new ("name", new (), new (), [
			new ("AttrName")
		]), new ("name1")];
		Assert.False (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsSameOrder ()
	{
		ImmutableArray<EnumMember> x = [new ("name", new (), new SymbolAvailability (), []), new ("name1")];
		ImmutableArray<EnumMember> y = [new ("name", new (), new SymbolAvailability (), []), new ("name1")];
		Assert.True (comparer.Equals (x, y));
	}

	[Fact]
	public void EqualsDiffOrder ()
	{
		ImmutableArray<EnumMember> x = [new ("name1"), new ("name")];
		ImmutableArray<EnumMember> y = [new ("name"), new ("name1")];
		Assert.True (comparer.Equals (x, y));
	}

	[Fact]
	public void NotEqualsDiffFieldData ()
	{
		ImmutableArray<EnumMember> x = [new ("name", new ("x", "xLib", EnumValue.None), new SymbolAvailability (), []), new ("name1")];
		ImmutableArray<EnumMember> y = [new ("name", new ("y", "xLib", EnumValue.None), new SymbolAvailability (), []), new ("name1")];
		Assert.False (comparer.Equals (x, y));
	} 
}
