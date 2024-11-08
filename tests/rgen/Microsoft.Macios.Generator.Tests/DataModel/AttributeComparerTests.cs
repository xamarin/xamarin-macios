using System;
using Microsoft.Macios.Generator.DataModel;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class AttributeComparerTests {
	AttributeComparer comparer;
	public AttributeComparerTests ()
	{
		comparer = new ();
	}

	[Fact]
	public void CompareBasedOnNames ()
	{
		var x = new AttributeCodeChange ("First");
		var y = new AttributeCodeChange ("Second");
		Assert.Equal (String.Compare (x.Name, y.Name, StringComparison.Ordinal), comparer.Compare (x, y));
		y = new AttributeCodeChange (x.Name);
		Assert.Equal (0, comparer.Compare (x, y));
	}

	[Fact]
	public void CompareBasedOnAttributeLength ()
	{
		var x = new AttributeCodeChange ("First", ["1", "2", "2"]);
		var y = new AttributeCodeChange ("First", ["1", "2"]);
		Assert.Equal (3.CompareTo (2), comparer.Compare (x, y));
	}

	[Fact]
	public void CompareBasedOnAttributeArguments ()
	{
		var x = new AttributeCodeChange ("First", ["1", "2", "2"]);
		var y = new AttributeCodeChange ("First", ["2", "1", "3"]);
		Assert.Equal (String.Compare (x.Arguments [0], y.Arguments [0], StringComparison.Ordinal), comparer.Compare (x, y));
	}
}
