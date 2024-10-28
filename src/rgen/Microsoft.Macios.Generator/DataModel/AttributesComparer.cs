using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

class AttributeComparer : IComparer<AttributeCodeChange> {
	public int Compare (AttributeCodeChange x, AttributeCodeChange y)
	{
		// return the order based on the following
		// 1. Attribute name
		// 2. Attribute param count
		// 3. Attribute values
		if (x.Name != y.Name)
			return String.Compare (x.Name, y.Name, StringComparison.Ordinal);
		if (x.Arguments.Length != y.Arguments.Length)
			return x.Arguments.Length.CompareTo (y.Arguments.Length);
		// argument order is important, we do know that we already have the same length, loop and return if diff
		for (int index = 0; index < x.Arguments.Length; index++) {
			var xArgument = x.Arguments [index];
			var yArgument = y.Arguments [index];
			var compare = String.Compare (xArgument, yArgument, StringComparison.Ordinal);
			if (compare != 0)
				return compare;
		}

		return 0;
	}
}
class AttributesComparer : IEqualityComparer<ImmutableArray<AttributeCodeChange>> {

	public bool Equals (ImmutableArray<AttributeCodeChange> x, ImmutableArray<AttributeCodeChange> y)
	{
		if (x.Length != y.Length)
			return false;
		var comparer = new AttributeComparer ();
		var xOrdered = x.Sort (comparer).ToArray ();
		var yOrdered = y.Sort (comparer).ToArray ();
		for (var i = 0; i < x.Length; i++) {
			if (xOrdered [i] != yOrdered [i])
				return false;
		}
		return true;
	}

	public int GetHashCode (ImmutableArray<AttributeCodeChange> obj)
	{
		var hash = new HashCode ();
		foreach (var change in obj) {
			hash.Add (change.GetHashCode ());
		}
		return hash.ToHashCode ();
	}
}
