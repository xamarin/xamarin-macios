using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Macios.Generator.DataModel;

class PropertyAccessorsComparer : IEqualityComparer<ImmutableArray<PropertyAccessorCodeChange>> {
	public bool Equals (ImmutableArray<PropertyAccessorCodeChange> x, ImmutableArray<PropertyAccessorCodeChange> y)
	{
		// property accessor kinds cannot be duplicated due to the definition of the language, create two dictionaries
		// using the kind as a key and then re-use our dictionary comparer
		var xDictionary = Enumerable.ToDictionary (x, accessor => accessor.Kind);
		var yDictionary = Enumerable.ToDictionary (y, accessor => accessor.Kind);
		var comparer = new DictionaryComparer<AccessorKind, PropertyAccessorCodeChange> ();
		return comparer.Equals (xDictionary, yDictionary);
	}

	public int GetHashCode (ImmutableArray<PropertyAccessorCodeChange> obj)
	{
		var hashCode = new HashCode ();
		foreach (var accessor in obj) {
			hashCode.Add (accessor);
		}

		return hashCode.ToHashCode ();
	}
}
