using System.Collections.Generic;
using Xunit;

namespace Microsoft.Macios.Generator.Tests;

public class DictionaryComparerTests {
	readonly DictionaryComparer<string, (string Name, string Surname)> comparer = new ();
	Dictionary<string, (string Name, string Surname)> left = new ();
	Dictionary<string, (string Name, string Surname)> right = new ();

	[Fact]
	public void EqualBothNull ()
		=> Assert.True (comparer.Equals (null, null));

	[Fact]
	public void EqualLeftNull ()
		=> Assert.False (comparer.Equals (null, right));

	[Fact]
	public void EqualRightNull ()
		=> Assert.False (comparer.Equals (left, null));

	[Fact]
	public void EqualDiffCount ()
	{
		left.Add ("A", ("A", "A"));
		left.Add ("B", ("B", "B"));
		right.Add ("A", ("A", "A"));
		Assert.False (comparer.Equals (left, right));
	}

	[Fact]
	public void EqualMissingKeySameCount ()
	{
		left.Add ("A", ("A", "A"));
		left.Add ("B", ("B", "B"));
		right.Add ("A", ("A", "A"));
		right.Add ("C", ("C", "C"));
		Assert.False (comparer.Equals (left, right));
	}

	[Fact]
	public void EqualDiffValues ()
	{
		left.Add ("A", ("A", "A"));
		left.Add ("B", ("B", "B"));
		right.Add ("A", ("A", "A"));
		right.Add ("B", ("B", "C"));
		Assert.False (comparer.Equals (left, right));
	}

}
