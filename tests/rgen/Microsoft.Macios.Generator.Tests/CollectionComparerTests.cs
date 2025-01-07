using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Macios.Generator.Tests;

public class CollectionComparerTests {
	class MyComparer : IComparer<string> {
		public int Compare (string? x, string? y)
			=> String.Compare (x, y, StringComparison.Ordinal);
	}

	readonly CollectionComparer<string> comparer = new (new MyComparer ());

	[Fact]
	public void CompareBothNull ()
		=> Assert.True (comparer.Equals (null, null));

	[Fact]
	public void CompareOneNull ()
	{
		var list = new List<string> ();
		Assert.False (comparer.Equals (null, list));
		Assert.False (comparer.Equals (list, null));
	}

	[Fact]
	public void CompareDiffSize ()
	{
		List<string> x = ["first", "second"];
		List<string> y = ["second"];
		Assert.False (comparer.Equals (x, y));
		Assert.False (comparer.Equals (y, x));
	}

	[Fact]
	public void CompareDiffLists ()
	{
		List<string> x = ["first", "second"];
		List<string> y = ["second", "third"];
		Assert.False (comparer.Equals (x, y));
		Assert.False (comparer.Equals (y, x));
	}

	[Fact]
	public void CompareDiffOrder ()
	{
		List<string> x = ["first", "second"];
		List<string> y = ["second", "first"];
		Assert.True (comparer.Equals (x, y));
		Assert.True (comparer.Equals (y, x));
	}
}
