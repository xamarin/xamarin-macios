using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

class AttributesComparer : IEqualityComparer<ImmutableArray<AttributeCodeChange>> {

	public bool Equals (ImmutableArray<AttributeCodeChange> x, ImmutableArray<AttributeCodeChange> y)
	{
		if (x.Length != y.Length)
			return false;
		var xOrdered = x.OrderBy (a => a.Name).ToArray ();
		var yOrdered = y.OrderBy (a => a.Name).ToArray ();
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
