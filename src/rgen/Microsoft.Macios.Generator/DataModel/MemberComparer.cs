using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

class MemberComparer : IEqualityComparer<ImmutableArray<MemberCodeChange>> {
	
	public bool Equals (ImmutableArray<MemberCodeChange> x, ImmutableArray<MemberCodeChange> y)
	{
		if (x.Length != y.Length)
			return false;
		var xOrdered = x.OrderBy (x => x.Name).ToArray ();
		var yOrdered = y.OrderBy (x => x.Name).ToArray ();
		for (int i = 0; i < x.Length; i++) {
			if (xOrdered[i] != yOrdered[i])
				return false;
		}
		return true;
	}

	public int GetHashCode (ImmutableArray<MemberCodeChange> obj)
	{
		var hash = new HashCode ();
		foreach (var change in obj) {
			hash.Add (change.GetHashCode ());	
		}
		return hash.ToHashCode ();
	}
}
